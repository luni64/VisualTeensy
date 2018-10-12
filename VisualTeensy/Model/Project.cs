using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;


namespace VisualTeensy.Model
{
    public class Project
    {
        public SetupData setup { get; private set; }
        public LibManager libManager { get; private set; }
        public List<Configuration> configurations { get; }
        public Configuration selectedConfiguration { get; private set; }

        // files ------------------------------------
        public string makefile { get; set; }
        public string tasks_json { get; set; }
        public string props_json { get; set; }
        public string vsSetup_json { get; set; }

        public string path { get; private set; }
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
                    var setupType = fileContent.setupType;

                    var configurations = new List<Configuration>();
                    foreach (var cfg in fileContent.configurations)
                    {
                        var configuration = new Configuration()
                        {
                            setupType = setupType,  // remove from config

                            compilerBase = cfg.compilerBase,
                            makefileExtension = cfg.makefileExtension,
                            boardTxtPath = cfg.boardTxtPath,//.StartsWith("\\") ? Path.Combine(projectPath, cfg.boardTxtPath.Substring(1)) : cfg.boardTxtPath,
                            coreBase = cfg.coreBase,//.StartsWith("\\") ? Path.Combine(projectPath, cfg.coreBase.Substring(1)) : cfg.coreBase,                                                 
                        };
                        
                        if (cfg.sharedLibraries?.Any() ?? false)
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

                        configuration.parseBoardsTxt(setupType == SetupTypes.quick ? setup.arduinoBoardsTxt : null);

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
            makefile = tasks_json = props_json = null;

            bool ok = selectedConfiguration.selectedBoard != null && setup.uplTyBaseError == null && pathError == null;
            if (selectedConfiguration.setupType == SetupTypes.quick)
            {
                ok = ok && setup.arduinoBaseError == null;
            }
            else
            {
                ok = ok && selectedConfiguration.corePathError == null && selectedConfiguration.compilerPathError == null;
            }

            if (ok)
            {
                log.Debug("OK (makefile, props_json, vsSetup_json)");


                makefile = selectedConfiguration.generateMakefile(setup, path, name, libManager);
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

