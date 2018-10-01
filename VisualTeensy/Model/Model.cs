using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace VisualTeensy.Model
{
    public class Model
    {
        public ProjectData project { get; private set; }
        public SetupData setup { get; private set; }

        public LibManager libManager { get; private set; }

        public List<Board> boards { get; private set; }
        public Board selectedBoard { get; set; }

        
        public bool openProjectPath()
        {
            vtTransferData transferData;

            var configFile = Path.Combine(project.path, ".vscode", "visual_teensy.json");
            try
            {
                var reader = new StreamReader(configFile);
                var jsonString = reader.ReadToEnd();
                var json = new JavaScriptSerializer();
                transferData = json.Deserialize<vtTransferData>(jsonString);

                if (transferData != null)
                {
                    project.setupType = transferData.quickSetup;
                    setup.arduinoBase = transferData.arduinoBase;
                    project.compilerBase = transferData.compilerBase;
                    setup.makeExePath = transferData.makeExePath;

                    project.boardTxtPath = transferData.boardTxtPath.StartsWith("\\") ? Path.Combine(project.path, transferData.boardTxtPath.Substring(1)) : transferData.boardTxtPath;
                    project.coreBase = transferData.coreBase.StartsWith("\\") ? Path.Combine(project.path, transferData.coreBase.Substring(1)) : transferData.coreBase;

                    parseBoardsTxt();



                    var libs = libManager.repositories[0].libraries;

                    foreach (string libName in transferData.libraries[0].libraries)
                    {
                        var lib = libs.FirstOrDefault(l => l.name == libName);
                        if (lib != null) project.libraries.Add(lib);
                    }


                    selectedBoard = boards?.FirstOrDefault(b => b.name == transferData.board.name);
                    if (selectedBoard != null)
                    {
                        foreach (var tos in transferData.board.options)
                        {
                            var os = selectedBoard.optionSets.FirstOrDefault(x => x.name == tos.Key);
                            if (os != null)
                            {
                                os.selectedOption = os.options.FirstOrDefault(x => x.name == tos.Value);
                            }
                        }
                    }
                }

                generateFiles();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void generateFiles()
        {
            project.makefile = project.tasks_json = project.props_json = null;

            bool ok = selectedBoard != null && setup.uplTyBaseError == null && project.pathError == null;
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
                Console.WriteLine("generate files");

                var options = selectedBoard.getAllOptions();

                project.makefile = generateMakefile(options);
                project.props_json = generatePropertiesFile(options);
                project.vsSetup_json = generateVisualTeensySetup();
            }
            project.tasks_json = generateTasksFile();
        }

        public Model(ProjectData project, SetupData setup)
        {
            this.project = project;
            this.setup = setup;

            boards = new List<Board>();

            loadSettings();
            libManager = new LibManager(this.project, this.setup);
            if (!openProjectPath())
            {
                parseBoardsTxt();
                selectedBoard = boards?.FirstOrDefault();
                generateFiles();
            }
        }

        public void parseBoardsTxt()
        {
            Console.WriteLine("parseBoardsTxt");

            string boardTxtPath = project.setupType == SetupTypes.quick ? setup.boardFromArduino : project.boardTxtPath;
            boards = FileContent.parse(boardTxtPath).Where(b => b.core == "teensy3").ToList();
        }
        private string generateVisualTeensySetup()
        {
            var json = new JavaScriptSerializer();
            return FileHelpers.formatOutput(json.Serialize(new vtTransferData(project, setup, selectedBoard)));
        }
        private string generateTasksFile()
        {
            if (setup.makeExePathError != null) return null;

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
                    }
                    ,
                    new Task()
                    {
                        label = "Upload (TyCommander)",
                        group = new Group(),
                        command = makePath.Replace('\\','/'),
                        args = new List<string>{"uploadTy" ,"-j","-Otarget"},
                    }
                }
            };
            var json = new JavaScriptSerializer();
            return FileHelpers.formatOutput(json.Serialize(tasks));
        }
        private string generatePropertiesFile(Dictionary<string, string> options)
        {
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

            return FileHelpers.formatOutput(new JavaScriptSerializer().Serialize(props));
        }
        private string generateMakefile(Dictionary<string, string> options)
        {
            StringBuilder mf = new StringBuilder();

            mf.Append("#******************************************************************************\n");
            mf.Append("# Generated by VisualTeensy (https://github.com/luni64/VisualTeensy)\n");
            mf.Append("#\n");
            mf.Append($"# {"Board",-18} {selectedBoard.name}\n");
            selectedBoard.optionSets.ForEach(o => mf.Append($"# {o.name,-18} {o.selectedOption?.name}\n"));
            mf.Append("#\n");
            mf.Append($"# {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}\n");
            mf.Append("#******************************************************************************\n");

            mf.Append("SHELL := cmd.exe\nexport SHELL\n\n");

            mf.Append($"TARGET_NAME      := {project.name.Replace(" ", "_")}\n\n");

            mf.Append($"LIBS_SHARED_BASE := {setup.libBaseShort}\n");
            mf.Append($"LIBS_SHARED      := ");
            project.libraries.ForEach(l => mf.Append($"{l.path} "));
            mf.Append("\n\n");

            mf.Append($"LIBS_LOCAL_BASE  := lib\n");
            mf.Append($"LIBS_LOCAL       := \n\n");

            mf.Append(makeEntry("BOARD_ID    := ", "build.board", options) + "\n");
            mf.Append($"CORE_BASE   := {((project.copyCore || (Path.GetDirectoryName(project.coreBase) == project.path)) ? "core" : project.coreBaseShort)}\n");
            mf.Append($"GCC_BASE    := {project.compilerBaseShort}\n");
            mf.Append($"UPL_PJRC_B  := {setup.uplPjrcBaseShort}\n");
            mf.Append($"UPL_TYCMD_B := {setup.uplTyBaseShort}\n\n");

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
        void loadSettings()
        {
            setup.libBase = Path.Combine(setup.arduinoBase ??"", "hardware", "Teensy", "avr", "libraries"); //HACK need to be user settable        
        }
    }
}

