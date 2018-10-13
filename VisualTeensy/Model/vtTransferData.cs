using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VisualTeensy.Model
{

    public class vtBoard
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

    public class projectTransferData
    {
        public class vtBoard
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
            [JsonConverter(typeof(StringEnumConverter))]
            public SetupTypes setupType { get; set; }

            [JsonProperty(Order = 3)]
            public string boardTxtPath { get; set; }

            [JsonProperty(Order = 4)]
            public string coreBase { get; set; }

            [JsonProperty(Order = 5)]
            public string compilerBase { get; set; }

            [JsonProperty(Order = 6)]
            public string makefileExtension { get; set; }

            [JsonProperty(Order = 7)]
            public bool copyBoardTxt { get; set; }

            [JsonProperty(Order = 8)]
            public bool copyCore { get; set; }


            [JsonProperty(Order = 9)]
            public IEnumerable<string> sharedLibraries{get; set;}

            [JsonProperty(Order = 10)]
            public IEnumerable<string> localLibraries { get; set; }
            [JsonProperty(Order = 11)]
            public vtBoard board { get; set; }


            public vtConfiguration(Configuration configuration)
            {
                if (configuration == null ) return;
                setupType = configuration.setupType;
                name = configuration.name;

                sharedLibraries = configuration.sharedLibs.Select(lib => lib.path);
                localLibraries = configuration.localLibs.Select(lib => lib.sourceType == Library.SourceType.local ? lib.path : lib.unversionedLibFolder);

                makefileExtension = configuration.makefileExtension;
                compilerBase = configuration.compilerBase;
                coreBase = configuration.coreBase;
                copyCore = configuration.copyCore;
                boardTxtPath = configuration.boardTxtPath;
                copyBoardTxt = configuration.copyBoardTxt;
                board = new vtBoard(configuration.selectedBoard);
            }


            public override string ToString()
            {
                return name;
            }
        }

        [JsonProperty(Order = 1)]
        public string version { get; set; }

        //[JsonProperty(Order = 2)]
        //[JsonConverter(typeof(StringEnumConverter))]
        //public SetupTypes setupType { get; set; }

        [JsonProperty(Order = 3)]
        public List<vtConfiguration> configurations;

        public projectTransferData(Project project)
        {
            this.model = project;
            version = "1";
           // setupType = project.selectedConfiguration.setupType;

            configurations = project.configurations.Select(c=>new vtConfiguration(c)).ToList();
            

            //configurations = new List<vtConfiguration>()
            //{
            //    new vtConfiguration(project.selectedConfiguration){ name = "default" }
            //};
        }

        public projectTransferData() { }

        //private string makeRelative(string path, string basePath)
        //{
        //    if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(basePath))
        //    {
        //        return path;
        //    }

        //    if (Path.GetFullPath(path).StartsWith(Path.GetFullPath(basePath)))
        //    {
        //        var p1 = new System.Uri(path);
        //        var baseUri = new System.Uri(basePath);

        //        return p1.MakeRelativeUri(baseUri).ToString();
        //    }
        //    else
        //    {
        //        return path;
        //    }
        //}

        private Project model;
    }
}
