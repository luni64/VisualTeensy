using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VisualTeensy.Model
{
    public class Project
    {
        public SetupData setup { get; private set; }
        public LibManager libManager { get; private set; }
        public List<Configuration> configurations { get; }
        public Configuration selectedConfiguration { get; private set; }

        // files ------------------------------------
       // public string makefile { get; set; }
        public string tasks_json { get; set; }
        public string props_json { get; set; }
        public string vsSetup_json { get; set; }

        public string path { get; set; }
        public string pathError
        {
            get
            {
                try
                {
                    Path.GetFullPath(path);
                    return null;
                }
                catch { return "Path to the project folder not valid"; }
            }
        }
        public string name => Path.GetFileName(path ?? "ERROR");

        public void newProject()
        {
            log.Info("new project");

            // Project Path -------------------------------------
            int i = 1;
            path = Path.Combine(setup.projectBaseDefault, $"newProject");
            while (Directory.Exists(path)) { path = Path.Combine(setup.projectBaseDefault, $"newProject({i++})"); }

            // Add a default configuration ----------------------
            configurations.Clear();
            selectedConfiguration = Configuration.getDefault(setup);
            configurations.Add(selectedConfiguration);

            var dummyConfig = Configuration.getDefault(setup);
            dummyConfig.name = "Testdummy";
            configurations.Add(dummyConfig);
            
            generateFiles();
        }
        public void openProject(string projectPath)
        {
            log.Info($"open project {projectPath}");

            path = projectPath;

            configurations.Clear();

            var vsTeensyJson = Path.Combine(projectPath, ".vsteensy", "vsteensy.json");
            if (!File.Exists(vsTeensyJson))
            {
                log.Warn($"config file {vsTeensyJson} does not exist");
                selectedConfiguration = Configuration.getDefault(setup);
                configurations.Add(selectedConfiguration);
                generateFiles();
                return;
            }

            try
            {
                string jsonString = File.ReadAllText(vsTeensyJson);
                log.Debug("vsTeensy.json content:\n" + jsonString);

                var fileContent = JsonConvert.DeserializeObject<projectTransferData>(jsonString);

                if (fileContent?.version == "1" && fileContent.configurations.Count > 0)
                {
                    foreach (var cfg in fileContent.configurations)
                    {
                        var configuration = new Configuration()
                        {
                            setupType = cfg.setupType,  // remove from config
                            name = cfg.name,

                            compilerBase = cfg.compilerBase,
                            makefileExtension = cfg.makefileExtension,
                            boardTxtPath = cfg.boardTxtPath,//.StartsWith("\\") ? Path.Combine(projectPath, cfg.boardTxtPath.Substring(1)) : cfg.boardTxtPath,
                            coreBase = cfg.coreBase,//.StartsWith("\\") ? Path.Combine(projectPath, cfg.coreBase.Substring(1)) : cfg.coreBase,                                                 
                        };
                        
                        if (cfg.sharedLibraries != null)
                        {
                            var sharedLibraries = libManager.sharedRepository.libraries.Select(version => version.FirstOrDefault()); //flatten out list by selecting first version. Shared libraries  can only have one version
                            foreach (var cfgSharedLib in cfg.sharedLibraries)
                            {
                                var library = sharedLibraries.FirstOrDefault(lib => lib.path == cfgSharedLib); // find the corresponding lib
                                if (library != null)
                                {
                                    configuration.sharedLibs.Add(library);
                                }
                            }
                        }

                        if (cfg.localLibraries != null)
                        {
                            var localLibraries = LibraryReader.parseLibraryLocal(Path.Combine(projectPath, "lib")).Select(version => version.FirstOrDefault());
                            foreach(var cfgLocalLib in cfg.localLibraries)
                            {
                                var library = localLibraries.FirstOrDefault(lib => lib.path == cfgLocalLib);
                                if (library != null)
                                {
                                    configuration.localLibs.Add(library);
                                }
                            }
                        }                       

                        configuration.parseBoardsTxt(configuration.setupType == SetupTypes.quick ? setup.arduinoBoardsTxt : null);

                        configuration.selectedBoard = configuration.boards?.FirstOrDefault(b => b.name == cfg.board.name);
                        if (configuration.selectedBoard != null)
                        {
                            foreach (var option in cfg.board.options)
                            {
                                var optionSet = configuration.selectedBoard.optionSets.FirstOrDefault(x => x.name == option.Key);
                                if (optionSet != null)
                                {
                                    optionSet.selectedOption = optionSet.options.FirstOrDefault(x => x.name == option.Value);
                                }
                            }
                        }
                        configurations.Add(configuration);
                    }
                    selectedConfiguration = configurations.FirstOrDefault();
                    log.Info($"{vsTeensyJson} read sucessfully");
                }
                else
                {
                    selectedConfiguration = Configuration.getDefault(setup);
                    configurations.Add(selectedConfiguration);
                    log.Info($"{vsTeensyJson} parse error, using default configuration");
                }
                generateFiles();
            }
            catch (Exception ex)
            {
                log.Warn($"config file {vsTeensyJson} does not exist");
                selectedConfiguration = Configuration.getDefault(setup);
                configurations.Add(selectedConfiguration);
                generateFiles();
                return;
            }
        }

        public Project(SetupData setup, LibManager libManager)
        {
            this.setup = setup;
            this.libManager = libManager;
            this.configurations = new List<Configuration>();

            // openProject(Settings.Default.lastProject);           

        }

        public RepositoryIndexJson sharedLibraries { get; }


        public void generateFiles()
        {
            log.Info("enter");
            tasks_json = props_json = null;
            configurations.ForEach(c => c.makefile = null);

            bool ok = selectedConfiguration.selectedBoard != null && setup.uplTyBaseError == null && pathError == null;
            if (configurations.Any(t=> t.setupType == SetupTypes.quick))
            {
                ok = ok && setup.arduinoBaseError == null;
            }
            else
            {
                ok = ok && selectedConfiguration.corePathError == null && selectedConfiguration.compilerPathError == null;  // extend to all configurations
            }

            if (ok)
            {
                log.Debug("OK (makefile, props_json, vsSetup_json)");
                configurations.ForEach(c => c.makefile = generateMakefile(c));

                props_json = generatePropertiesFile(selectedConfiguration.selectedBoard.getAllOptions());
                vsSetup_json = generateVisualTeensySetup();
            }
            else
            {
                log.Debug("NOK (makefile, props_json, vsSetup_json)");
                selectedConfiguration.logProject();
            }

            tasks_json = generateTasksFile();
        }

        private string generateVisualTeensySetup()
        {
            log.Debug("enter");
            return JsonConvert.SerializeObject(new projectTransferData(this), Formatting.Indented);
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
            if (selectedConfiguration.compilerPathError != null)
            {
                return null;
            }

            var props = new PropertiesJson()
            {
                configurations = new List<ConfigurationJson>()
                {
                    new ConfigurationJson()
                    {
                        name = "VisualTeensy",
                        compilerPath =  Path.Combine(selectedConfiguration.compilerBase ,"bin","arm-none-eabi-gcc.exe").Replace('\\','/'),
                        intelliSenseMode = "gcc-x64",
                        includePath = new List<string>()
                        {
                            "src/**",
                            "lib/**",
                            selectedConfiguration.coreBase?.Replace('\\','/') + "/**",
                            libManager.sharedRepositoryPath.Replace('\\','/') + "/**"
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
        public string generateMakefile(Configuration cfg)
        {
            var options = cfg.selectedBoard.getAllOptions();

            log.Debug("enter");
            StringBuilder mf = new StringBuilder();

            mf.Append("#******************************************************************************\n");
            mf.Append("# Generated by VisualTeensy (https://github.com/luni64/VisualTeensy)\n");
            mf.Append("#\n");
            mf.Append($"# {"Board",-18} {cfg.selectedBoard.name}\n");
            cfg.selectedBoard.optionSets.ForEach(o => mf.Append($"# {o.name,-18} {o.selectedOption?.name}\n"));
            mf.Append("#\n");
            mf.Append($"# {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}\n");
            mf.Append("#******************************************************************************\n");

            mf.Append($"SHELL            := cmd.exe\nexport SHELL\n\n");

            mf.Append($"TARGET_NAME      := {name?.Replace(" ", "_")}\n");

            mf.Append(makeEntry("BOARD_ID         := ", "build.board", options) + "\n\n");

            mf.Append($"LIBS_SHARED_BASE := {Helpers.getShortPath(libManager.sharedRepositoryPath)}\n");
            mf.Append($"LIBS_SHARED      := ");
            foreach (var lib in cfg.sharedLibs)
            {
                mf.Append($"{lib.path ?? "ERROR"} "); //hack, improve library to distinguish between libraries to download and loacal libs
            }
            mf.Append("\n\n");

            mf.Append($"LIBS_LOCAL_BASE  := lib\n");
            mf.Append($"LIBS_LOCAL       := ");
            foreach (var lib in cfg.localLibs)
            {
                mf.Append($"{lib.path ?? lib.name} "); //hack, improve library to distinguish between libraries to download and loacal libs
            }
            mf.Append("\n\n");

            if (cfg.setupType == SetupTypes.quick)
            {
                mf.Append($"CORE_BASE        := {Helpers.getShortPath(setup.arduinoCore)}\n");
                mf.Append($"GCC_BASE         := {Helpers.getShortPath(setup.arduinoCompiler)}\n");
                mf.Append($"UPL_PJRC_B       := {Helpers.getShortPath(setup.arduinoTools)}\n");
            }
            else
            {
                mf.Append($"CORE_BASE        := {((cfg.copyCore || (Path.GetDirectoryName(cfg.coreBase) == path)) ? "core" : Helpers.getShortPath(cfg.coreBase))}\n");
                mf.Append($"GCC_BASE         := {Helpers.getShortPath(cfg.compilerBase)}\n");
                mf.Append($"UPL_PJRC_B       := {Helpers.getShortPath(setup.uplPjrcBase)}\n");
            }
            mf.Append($"UPL_TYCMD_B      := {Helpers.getShortPath(setup.uplTyBase)}\n");
            mf.Append($"UPL_CLICMD_B     := {Helpers.getShortPath(setup.uplCLIBase)}\n\n");

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

            if (cfg.setupType == SetupTypes.expert && !String.IsNullOrWhiteSpace(cfg.makefileExtension))
            {
                mf.Append("\n");
                mf.Append(cfg.makefileExtension);
                mf.Append("\n");
            }

            mf.Append(setup.makefile_fixed);

            return mf.ToString();
        }

        private void addConfigOption(Dictionary<string, string> options, PropertiesJson props, string prefix, string key)
        {
            var option = options.FirstOrDefault(o => o.Key == key).Value;

            if (option != null)
            {
                props.configurations[0].defines.Add(prefix + option);
            }
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

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}

