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
        public BuildSystem buildSystem { get; set; } = BuildSystem.makefile;
        public DebugSupport debugSupport { get; set; } = DebugSupport.none;

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
        public string cleanName => name.Replace(" ", "_");

        public void newProject()
        {
            // Project Path -------------------------------------
            int i = 1;
            path = Path.Combine(setup.projectBaseDefault, $"newProject");
            while (Directory.Exists(path)) { path = Path.Combine(setup.projectBaseDefault, $"newProject({i++})"); }

            buildSystem = BuildSystem.makefile;
            target = Target.vsCode;
            debugSupport = setup.debugSupportDefault == true ? DebugSupport.cortex_debug : DebugSupport.none;

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
            _configurations.Clear();

            this.path = projectPath;
            string vsTeensyJsonPath = Path.Combine(projectPath, ".vsteensy", "vsteensy.json");


            if (!File.Exists(vsTeensyJsonPath))
            {
                openNewFolder();
            }
            else
            {
                openExistingFolder(vsTeensyJsonPath);
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



        void openNewFolder()
        {
            buildSystem = BuildSystem.makefile;
            target = Target.vsCode;
            debugSupport = DebugSupport.none;

            var sc = Configuration.getDefault(setup);
            _configurations.Add(sc);
            _selectedConfiguration = sc;
        }

        void openExistingFolder(string vsTeensyJsonPath)
        {
            try
            {  // read vsTeensy.json
                var fileContent = JsonConvert.DeserializeObject<ProjectTransferData>(File.ReadAllText(vsTeensyJsonPath));

                if (fileContent == null || fileContent.version != "1" || fileContent.configurations.Count == 0)
                {
                    openNewFolder();
                }
                else
                {
                    buildSystem = fileContent.buildSystem;
                    target = fileContent.target;
                    debugSupport = fileContent.debugSupport;


                    foreach (var cfg in fileContent.configurations)
                    {
                        var configuration = (Configuration)cfg;

                        // add shared libraries
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

                        // add libraries installed in ./lib                                                
                        string libPath = Path.Combine(path, "lib");

                        var localLibs = LibraryReader.parseLibraryLocal(libPath)?.Select(version => version.FirstOrDefault());

                        if (localLibs != null)
                        {
                            foreach (var lib in localLibs)
                            {
                                lib.source = Path.Combine(libPath, lib.path);  // lib will be copied to local => set source to the local path
                                configuration.localLibs.Add(lib);
                            }
                        }

                        // initialize boards list and options
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

                    }
                    _selectedConfiguration = _configurations.FirstOrDefault();
                }

            }
            catch (Exception ex)
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


    }
}

