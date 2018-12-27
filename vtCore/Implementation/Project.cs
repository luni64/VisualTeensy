using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace vtCore
{
    public static class Factory
    {
        public static IProject makeProject(SetupData setup, LibManager libManager)
        {
            return new Project(setup, libManager);
        }
        public static LibManager makeLibManager(SetupData setup)
        {
            return new LibManager(setup);
        }
    }


    internal class Project : IProject
    {
        public IEnumerable<IConfiguration> configurations => _configurations;
        public IConfiguration selectedConfiguration => _selectedConfiguration;

        private SetupData setup;
        public LibManager libManager { get; private set; }

        public Target target { get; set; } = Target.vsCode;
        public BuildSystem buildSystem { get; set; }

        private List<Configuration> _configurations { get; }
        private Configuration _selectedConfiguration { get; set; }

        public bool isMakefileBuild { get; set; } = true;





        // files ------------------------------------
        // public string makefile { get; set; }

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
            // Project Path -------------------------------------
            int i = 1;
            path = Path.Combine(setup.projectBaseDefault, $"newProject");
            while (Directory.Exists(path)) { path = Path.Combine(setup.projectBaseDefault, $"newProject({i++})"); }

            buildSystem = BuildSystem.makefile;
            target = Target.vsCode;

            // Add a default configuration ----------------------
            _configurations.Clear();
            var sc = Configuration.getDefault(setup);

            _selectedConfiguration = sc;// Configuration.getDefault(setup);
            _configurations.Add(sc);
            //_configs.Add(selectedConfiguration);

            var dummyConfig = Configuration.getDefault(setup);
            dummyConfig.name = "Testdummy";
            _configurations.Add(dummyConfig);


           

           

        }
        public void openProject(string projectPath)
        {
            path = projectPath;

            _configurations.Clear();

            var vsTeensyJson = Path.Combine(projectPath, ".vsteensy", "vsteensy.json");
            if (!File.Exists(vsTeensyJson))
            {
                buildSystem = BuildSystem.makefile;
                target = Target.vsCode;

                var sc = Configuration.getDefault(setup);
                _selectedConfiguration = sc; // Configuration.getDefault(setup);
                _configurations.Add(sc);// selectedConfiguration);                          
                return;
            }

            try
            {
                string jsonString = File.ReadAllText(vsTeensyJson);
                //log.Debug("vsTeensy.json content:\n" + jsonString);

                var fileContent = JsonConvert.DeserializeObject<projectTransferData>(jsonString);

                if (fileContent?.version == "1" && fileContent.configurations.Count > 0)
                {
                    buildSystem = fileContent.buildSystem;
                    target = fileContent.target;

                    foreach (var cfg in fileContent.configurations)
                    {
                        var configuration = new Configuration()
                        {
                            setupType = cfg.setupType,  // remove from config

                            name = cfg.name,
                            guid = cfg.guid != null ? cfg.guid : Guid.NewGuid().ToString(),

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
                            foreach (var cfgLocalLib in cfg.localLibraries)
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
                        _configurations.Add(configuration);
                        // _configs.Add(configuration);
                    }
                    _selectedConfiguration = _configurations.FirstOrDefault();
                    //log.Info($"{vsTeensyJson} read sucessfully");
                }
                else
                {
                    var sc = Configuration.getDefault(setup);
                    _selectedConfiguration = sc;// Configuration.getDefault(setup);
                    _configurations.Add(sc);//selectedConfiguration);
                    //_configs.Add(selectedConfiguration);
                    //log.Info($"{vsTeensyJson} parse error, using default configuration");
                }
                //  generateFiles();
            }
            catch //(Exception ex)
            {
                //log.Warn($"config file {vsTeensyJson} does not exist");
                var sc = Configuration.getDefault(setup);
                _selectedConfiguration = sc;// Configuration.getDefault(setup);
                _configurations.Add(sc);//electedConfiguration);
                                        //_configs.Add(selectedConfiguration);
                                        //   generateFiles();
                return;
            }
        }

        public Project(SetupData setup, LibManager libManager)
        {
            this.setup = setup;
            this.libManager = libManager;
            this._configurations = new List<Configuration>();

            //var userPackages = Directory.GetDirectories(Helpers.arduinoAppPath + "\\packages");
            //var builtInPackages = Directory.GetDirectories(Helpers.arduinoPath + "\\hardware");
            //var packages = userPackages.Select(p => new Package(p)).ToList();
        }

        public RepositoryIndexJson sharedLibraries { get; }


        //public void generateFiles()
        //{
        //    //log.Info("enter");


        //    bool ok = _selectedConfiguration.selectedBoard != null && setup.uplTyBaseError == null && pathError == null;
        //    if (_configurations.Any(t => t.setupType == SetupTypes.quick))
        //    {
        //        ok = ok && setup.arduinoBaseError == null;
        //    }
        //    else
        //    {
        //        //TMPCommentar ok = ok && selectedConfiguration.corePathError == null && selectedConfiguration.compilerPathError == null;  // extend to all configurations
        //    }

        //    if (ok)
        //    {
        //        //log.Debug("OK (makefile, props_json, vsSetup_json)");
        //     //   _configurations.ForEach(c => c.makefile = generateMakefile(c));

        //      //  props_json = generatePropertiesFile(_selectedConfiguration.selectedBoard.getAllOptions());
        //        vsSetup_json = generateVisualTeensySetup();
        //    }
        //    else
        //    {
        //        //log.Debug("NOK (makefile, props_json, vsSetup_json)");
        //        //selectedConfiguration.logProject();
        //    }

        // //   tasks_json = generateTasksFile();
        //}

        //private string generateVisualTeensySetup()
        //{
        //    // log.Debug("enter");
        //    return JsonConvert.SerializeObject(new projectTransferData(this), Formatting.Indented);
        //}




    }
}

