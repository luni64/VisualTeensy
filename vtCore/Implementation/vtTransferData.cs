using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using vtCore.Interfaces;

namespace vtCore
{

    internal class vtBoard
    {
        public vtBoard(Board board)
        {
            name = board?.name;
            options = board?.optionSets?.ToDictionary(o => o.name, o => o.selectedOption?.name);
        }
        public string name { get; set; }
        public Dictionary<string, string> options { get; set; } = new Dictionary<string, string>();
        public override string ToString()
        {
            return name;
        }
    }

    public class ProjectTransferData
    {
        public class vtBoard
        {
            public vtBoard(IBoard board)
            {
                name = board?.name;
                options = board?.optionSets?.ToDictionary(o => o.name, o => o.selectedOption?.name ?? o.options.FirstOrDefault()?.name);
               
                
            }
            public string name { get; set; }
            public Dictionary<string, string> options { get; set; } = new Dictionary<string, string>();
            public override string ToString()
            {
                return name;
            }
        }
        public class vtRepo
        {
            public string name { get; set; }
            public IEnumerable<string> libraries { get; set; }
        }
        public class vtConfiguration
        {
            [JsonProperty(Order = 1)]
            public string name { get; set; }

            [JsonProperty(Order = 2)]
            public string guid { get; set; }

            [JsonProperty(Order = 3)]
            [JsonConverter(typeof(StringEnumConverter))]
            public SetupTypes setupType { get; set; }

            //[JsonProperty(Order = 4)]
            //public string boardTxtPath { get; set; }

            [JsonProperty(Order = 5)]
            public string coreBase { get; set; }

            [JsonProperty(Order = 6)]
            public string compilerBase { get; set; }

            [JsonProperty(Order = 7)]
            public string makefileExtension { get; set; }

            //[JsonProperty(Order = 8)]
            //public bool copyBoardTxt { get; set; }

            [JsonProperty(Order = 9)]
            public bool copyCore { get; set; }

            [JsonProperty(Order = 10)]
            public IEnumerable<string> sharedLibraries { get; set; }

            [JsonProperty(Order = 11)]
            public IEnumerable<string> projectLibraries { get; set; }

            [JsonProperty(Order = 12)]
            public vtBoard board { get; set; }
            
            public vtConfiguration(IConfiguration configuration)
            {
                if (configuration == null)
                {
                    return;
                }

                setupType = configuration.setupType;
                name = configuration.name;
                guid = configuration.guid;

                //sharedLibraries = configuration.sharedLibs.Select(lib => lib.path);
                //projectLibraries = configuration.localLibs.Select(lib => lib.isLocalSource ? lib.path : lib.unversionedLibFolder);
                sharedLibraries = configuration.sharedLibs.Select(lib => lib.sourceUri.Segments.Last());
                projectLibraries = configuration.localLibs.Select(lib => lib.sourceUri.Segments.Last());

                makefileExtension = configuration.makefileExtension;
                compilerBase = configuration.compilerBase.path;
                coreBase = configuration.coreBase.path;
                copyCore = configuration.copyCore;
                //  boardTxtPath = configuration.boardTxtPath;
                //  copyBoardTxt = configuration.copyBoardTxt;
                board = new vtBoard(configuration.selectedBoard);
            }

            public override string ToString()
            {
                return name;
            }

            public static explicit operator Configuration(vtConfiguration vtConf)
            {
                var conf = new Configuration();

                conf.name = vtConf.name;
                conf.setupType = vtConf.setupType;
                conf.compilerBase.path = vtConf.compilerBase;
                conf.coreBase.path = vtConf.coreBase;
                conf.copyCore = vtConf.copyCore;
                conf.makefileExtension = vtConf.makefileExtension;
                conf.guid = vtConf.guid != null ? vtConf.guid : Guid.NewGuid().ToString();
                return conf;
            }
        }

        [JsonProperty(Order = 1)]
        public string version { get; set; }

        [JsonProperty(Order = 2)]
        [JsonConverter(typeof(StringEnumConverter))]
        public Target target { get; set; }

        [JsonProperty(Order = 3)]
        [JsonConverter(typeof(StringEnumConverter))]
        public BuildSystem buildSystem { get; set; }

        [JsonProperty(Order = 4)]
        [JsonConverter(typeof(StringEnumConverter))]
        public DebugSupport debugSupport { get; set; }

        [JsonProperty(Order = 5)]
        public List<vtConfiguration> configurations;

        internal ProjectTransferData(IProject project)
        {
            this.model = project;
            version = "1";
            target = project.target;
            buildSystem = project.buildSystem;
            debugSupport = project.debugSupport;

            configurations = project.configurations.Select(c => new vtConfiguration(c)).ToList();
        }

        public ProjectTransferData() { }

       

        private IProject model;
    }


}
