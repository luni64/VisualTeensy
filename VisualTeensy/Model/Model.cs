using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
//using System.Web.Script.Serialization;

namespace VisualTeensy.Model
{
    public class Model
    {
        public ProjectData project { get; private set; }
        public SetupData setup { get; private set; }

        //public LibManager libManager { get; private set; }

        // files ------------------------------------
        public string makefile { get; set; }
        public string tasks_json { get; set; }
        public string props_json { get; set; }
        public string vsSetup_json { get; set; }


        public void newFile()
        {
            log.Info("new file");
            project.parseBoardsTxt();
            project.selectedBoard = project.boards.FirstOrDefault();
            project = ProjectData.getDefault(setup);
            generateFiles();
        }

        public void openFile(string filename)
        {
            log.Info("open file");
            project = ProjectData.open(filename, setup);
            if (project == null)
            {
                project = ProjectData.getDefault(setup);
                project.path = filename;
            }
            generateFiles();
        }

        public Model(ProjectData project, SetupData setup)
        {
            this.setup = setup;
            this.project = project;


            generateFiles();

            //libManager = new LibManager(this.project, this.setup);


            sharedLibraries = new Repository("Shared Libraries", this.setup.libBase);
        }

        public Repository sharedLibraries { get; }


        public void generateFiles()
        {
            log.Info("enter");
            makefile = tasks_json = props_json = null;

            bool ok = project.selectedBoard != null && setup.uplTyBaseError == null && project.pathError == null;
            if (project.setupType == SetupTypes.quick)
            {
                ok = ok && setup.arduinoBaseError == null;
            }
            else
            {
                ok = ok && project.corePathError == null && project.compilerPathError == null;
            }

            if (ok)
            {
                log.Debug("OK (makefile, props_json, vsSetup_json)");

                var options = project.selectedBoard.getAllOptions();

                makefile = generateMakefile(options);
                props_json = generatePropertiesFile(options);
                vsSetup_json = generateVisualTeensySetup();
            }
            else
            {
                log.Debug("NOK (makefile, props_json, vsSetup_json)");
                project.logProject();
            }

            tasks_json = generateTasksFile();
        }

        private string generateVisualTeensySetup()
        {
            log.Debug("enter");
            return JsonConvert.SerializeObject(new vtTransferData(project), Formatting.Indented);
        }
        private string generateTasksFile()
        {
            log.Debug("enter");
            if (setup.makeExePathError != null)
            {
                return null;
            }

            string makePath = setup.makeExePath;

            var tasks = new tasksJson()
            {
                presentation = new Presentation(),
                tasks = new List<Task>()
                {
                    new Task()
                    {
                        label = "Build",
                        group = new Group(),
                        command = makePath.Replace('\\','/'),
                        args = new List<string>{"all","-j","-Otarget"},
                    },
                    new Task()
                    {
                        label = "Rebuild User Code",
                        group = new Group(),
                        command = makePath.Replace('\\','/'),
                        args = new List<string>{"rebuild" ,"-j","-Otarget"},
                    },
                    new Task()
                    {
                        label = "Clean",
                        group = new Group(),
                        command = makePath.Replace('\\','/'),
                        args = new List<string>{"clean"},
                    },
                    new Task()
                    {
                        label = "Upload (Teensy Uploader)",
                        group = new Group(),
                        command = makePath.Replace('\\','/'),
                        args = new List<string>{"upload" ,"-j","-Otarget"},
                    },
                    new Task()
                    {
                        label = "Upload (TyCommander)",
                        group = new Group(),
                        command = makePath.Replace('\\','/'),
                        args = new List<string>{"uploadTy" ,"-j","-Otarget"},
                    },
                    new Task()
                    {
                        label = "Upload (CLI)",
                        group = new Group(),
                        command = makePath.Replace('\\','/'),
                        args = new List<string>{"uploadCLI" ,"-j","-Otarget"},
                    }
                }
            };

            return JsonConvert.SerializeObject(tasks, Formatting.Indented);
        }
        private string generatePropertiesFile(Dictionary<string, string> options)
        {
            log.Debug("enter");
            if (project.compilerPathError != null)
            {
                return null;
            }

            var props = new PropertiesJson()
            {
                configurations = new List<Configuration>()
                {
                    new Configuration()
                    {
                        name = "VisualTeensy",
                        compilerPath =  Path.Combine(project.compilerBase ,"bin","arm-none-eabi-gcc.exe").Replace('\\','/'),
                        intelliSenseMode = "gcc-x64",
                        includePath = new List<string>()
                        {
                            "src/**",
                            "lib/**",
                            project.coreBase?.Replace('\\','/') + "/**",
                            setup.libBase?.Replace('\\','/') + "/**"
                        },
                        defines = new List<string>()
                    }
                }
            };
            
            if (options.ContainsKey("build.flags.defs"))
            {
                foreach (var define in options["build.flags.defs"].Split(new string[] { "-D" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    props.configurations[0].defines.Add(define.Trim());
                }
            }
            addConfigOption(options, props, "F_CPU=", "build.fcpu");
            addConfigOption(options, props, "", "build.usbtype");
            addConfigOption(options, props, "LAYOUT_", "build.keylayout");
            props.configurations[0].defines.Add("ARDUINO");

            //props.configurations[0].defines.Add("F_CPU=" + options["build.fcpu"]);
            //props.configurations[0].defines.Add(options["build.usbtype"]);
            //props.configurations[0].defines.Add("LAYOUT_" + options["build.keylayout"]);

            return JsonConvert.SerializeObject(props, Formatting.Indented);

        }
        private string generateMakefile(Dictionary<string, string> options)
        {
            log.Debug("enter");
            StringBuilder mf = new StringBuilder();

            mf.Append("#******************************************************************************\n");
            mf.Append("# Generated by VisualTeensy (https://github.com/luni64/VisualTeensy)\n");
            mf.Append("#\n");
            mf.Append($"# {"Board",-18} {project.selectedBoard.name}\n");
            project.selectedBoard.optionSets.ForEach(o => mf.Append($"# {o.name,-18} {o.selectedOption?.name}\n"));
            mf.Append("#\n");
            mf.Append($"# {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}\n");
            mf.Append("#******************************************************************************\n");

            mf.Append($"SHELL            := cmd.exe\nexport SHELL\n\n");

            mf.Append($"TARGET_NAME      := {project.name?.Replace(" ", "_")}\n");

            mf.Append(makeEntry("BOARD_ID         := ", "build.board", options) + "\n\n");

            mf.Append($"LIBS_SHARED_BASE := {Helpers.getShortPath(project.sharedLibs.path)}\n");
            mf.Append($"LIBS_SHARED      := ");
            foreach (var lib in project.sharedLibs.libraries.Where(l => l.isSelected))
            {
                mf.Append($"{lib.name} ");
            }
            mf.Append("\n\n");

            mf.Append($"LIBS_LOCAL_BASE  := {Helpers.getShortPath(project.localLibs.path)}\n");
            mf.Append($"LIBS_LOCAL       := ");
            foreach (var lib in project.localLibs.libraries.Where(l => l.isSelected))
            {
                mf.Append($"{lib.name} ");
            }
            mf.Append("\n\n");

            if (project.setupType == SetupTypes.quick)
            {
                mf.Append($"CORE_BASE    := {Helpers.getShortPath(setup.arduinoCore)}\n");
                mf.Append($"GCC_BASE     := {Helpers.getShortPath(setup.arduinoCompiler)}\n");
                mf.Append($"UPL_PJRC_B   := {Helpers.getShortPath(setup.arduinoTools)}\n");
            }
            else
            {
                mf.Append($"CORE_BASE    := {((project.copyCore || (Path.GetDirectoryName(project.coreBase) == project.path)) ? "core" : Helpers.getShortPath(project.coreBase))}\n");
                mf.Append($"GCC_BASE     := {Helpers.getShortPath(project.compilerBase)}\n");
                mf.Append($"UPL_PJRC_B   := {Helpers.getShortPath(setup.uplPjrcBase)}\n");
            }
            mf.Append($"UPL_TYCMD_B  := {Helpers.getShortPath(setup.uplTyBase)}\n");
            mf.Append($"UPL_CLICMD_B := {Helpers.getShortPath(setup.uplCLIBase)}\n\n");

            mf.Append(makeEntry("MCU   := ", "build.mcu", options) + "\n\n");

            mf.Append(makeEntry("FLAGS_CPU   := ", "build.flags.cpu", options) + "\n");
            mf.Append(makeEntry("FLAGS_OPT   := ", "build.flags.optimize", options) + "\n");
            mf.Append(makeEntry("FLAGS_COM   := ", "build.flags.common", options) + makeEntry(" ", "build.flags.dep", options) + "\n");
            mf.Append(makeEntry("FLAGS_LSP   := ", "build.flags.ldspecs", options) + "\n");

            mf.Append("\n");
            mf.Append(makeEntry("FLAGS_CPP   := ", "build.flags.cpp", options) + "\n");
            mf.Append(makeEntry("FLAGS_C     := ", "build.flags.c", options) + "\n");
            mf.Append(makeEntry("FLAGS_S     := ", "build.flags.S", options) + "\n");
            mf.Append(makeEntry("FLAGS_LD    := ", "build.flags.ld", options) + "\n");

            mf.Append("\n");
            mf.Append(makeEntry("LIBS        := ", "build.flags.libs", options) + "\n");
            mf.Append(makeEntry("LD_SCRIPT   := ", "build.mcu", options) + ".ld\n");

            mf.Append("\n");
            mf.Append(makeEntry("DEFINES     := ", "build.flags.defs", options) + " -DARDUINO=10807\n");
            mf.Append("DEFINES     += ");
            mf.Append(makeEntry("-DF_CPU=", "build.fcpu", options) + " " + makeEntry("-D", "build.usbtype", options) + " " + makeEntry("-DLAYOUT_", "build.keylayout", options) + "\n");

            mf.Append($"\n");
            mf.Append("CPP_FLAGS   := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_COM) $(DEFINES) $(FLAGS_CPP)\n");
            mf.Append("C_FLAGS     := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_COM) $(DEFINES) $(FLAGS_C)\n");
            mf.Append("S_FLAGS     := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_COM) $(DEFINES) $(FLAGS_S)\n");
            mf.Append("LD_FLAGS    := $(FLAGS_CPU) $(FLAGS_OPT) $(FLAGS_LSP) $(FLAGS_LD)\n");
            mf.Append("AR_FLAGS    := rcs\n");

            if (project.setupType == SetupTypes.expert && !String.IsNullOrWhiteSpace(project.makefileExtension))
            {
                mf.Append("\n");
                mf.Append(project.makefileExtension);
                mf.Append("\n");
            }

            mf.Append(setup.makefile_fixed);

            return mf.ToString();
        }

        private string makeEntry(String txt, String key, Dictionary<String, String> options)
        {
            if (options.ContainsKey(key))
            {
                return $"{txt}{options[key]}";
            }
            else
            {
                return "";
            }
        }
        private void addConfigOption(Dictionary<string, string> options, PropertiesJson props, string prefix, string key)
        {
            var option = options.FirstOrDefault(o => o.Key == key).Value;

            if (option != null)
            {
                props.configurations[0].defines.Add(prefix + option);
            }
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}

