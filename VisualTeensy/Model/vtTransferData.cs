using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
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
            public string name { get; set; }

            public string coreBase { get; set; }
            public bool copyCore { get; set; }
            public string boardTxtPath { get; set; }
            public bool copyBoardTxt { get; set; }
            public string compilerBase { get; set; }

            public string makefileExtension { get; set; }

            // public List<vtRepo> libraries { get; set; }
            public IEnumerable<string> sharedLibraries{get; set;}
            public vtBoard board { get; set; }


            public vtConfiguration(Configuration configuration)
            {
                if (configuration == null ) return;

                sharedLibraries = configuration.sharedLibs.Select(lib => lib.path);
                
              

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

        [JsonProperty(Order = 2)]
        [JsonConverter(typeof(StringEnumConverter))]
        public SetupTypes setupType { get; set; }

        [JsonProperty(Order = 3)]
        public List<vtConfiguration> configurations;

        public projectTransferData(Project project)
        {
            this.model = project;
            version = "1";
            setupType = project.selectedConfiguration.setupType;

            configurations = new List<vtConfiguration>()
            {
                new vtConfiguration(project.selectedConfiguration){ name = "default" }
            };
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
