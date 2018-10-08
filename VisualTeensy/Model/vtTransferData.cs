using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VisualTeensy.Model
{

    public class vtTransferData
    {
        [JsonProperty(Order = 1)]
        public string version { get; }

        [JsonProperty(Order = 2)]
        [JsonConverter(typeof(StringEnumConverter))]
        public SetupTypes setupType { get; set; }

        [JsonProperty(Order = 3)]
        public List<vtConfiguration> configurations;

        public class vtBoard
        {
            public vtBoard(Board board)
            {
                name = board?.name;
                options = board?.optionSets?.ToDictionary(o => o.name, o => o.selectedOption?.name);
            }
            public string name { get; set; }
            public Dictionary<string, string> options { get; set; } = new Dictionary<string, string>();
            public override string ToString() => name;
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
            public string boardTxtPath { get; set; }
            public string compilerBase { get; set; }

            public string projectName { get; set; }

            public string makefileExtension { get; set; }

            public List<vtRepo> repositories { get; set; }
            public vtBoard board { get; set; }

            ProjectData project;

            public vtConfiguration(Model model)
            {
                if (model?.project == null) return;

                this.project = model.project;

                makefileExtension = project.makefileExtension;
                compilerBase = project.compilerBase;

                repositories = new List<vtRepo>()
                {
                    new vtRepo()
                    {
                        name =   "shared",
                        libraries = model.libManager.projectLibraries.Where(lib => !lib.isLocal).Select(lib => lib.name)
                    },
                    new vtRepo()
                    {
                        name = "local",
                        libraries = model.libManager.projectLibraries.Where(lib => lib.isLocal).Select(lib => lib.name)
                    },
                };

                if (project.coreBase != null)
                {
                    coreBase = (project.copyCore || project.coreBase.StartsWith(project.path)) ? "\\core" : project.coreBase;
                }

                if (project.boardTxtPath != null)
                {
                    boardTxtPath = (project.copyBoardTxt || project.boardTxtPath.StartsWith(project.path)) ? "\\boards.txt" : project.boardTxtPath;
                }

                board = new vtBoard(project.selectedBoard);
            }
            public override string ToString() => name;
        }


        public vtTransferData(Model model)
        {
            version = "1";
            setupType = model.project.setupType;

            configurations = new List<vtConfiguration>()
            {
                new vtConfiguration(model){ name = "default" }
            };
        }
        
        public vtTransferData() { }
        
        string makeRelative(string path, string basePath)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(basePath))
            {
                return path;
            }

            if (Path.GetFullPath(path).StartsWith(Path.GetFullPath(basePath)))
            {
                var p1 = new System.Uri(path);
                var baseUri = new System.Uri(basePath);

                return p1.MakeRelativeUri(baseUri).ToString();
            }
            else
            {
                return path;
            }
        }

    }
}
