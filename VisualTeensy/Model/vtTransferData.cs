using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VisualTeensy.Model
{

    public class vtTransferData
    {
        public int version { get; } = 1;

        public class vsBoard
        {
            public vsBoard(Board board)
            {
                name = board?.name;
                options = board?.optionSets?.ToDictionary(o => o.name, o => o.selectedOption?.name);
            }
            public string name { get; set; }
            public Dictionary<string, string> options { get; set; } = new Dictionary<string, string>();
        }

        public class LibraryRepositiory
        {
            public string repository { get; set; }
            // public string path { get; set; }
            public List<string> libraries { get; set; }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public SetupTypes setupType { get; set; }


        public string coreBase { get; set; }
        public string boardTxtPath { get; set; }
        public string compilerBase { get; set; }

        public string projectName { get; set; }
        public List<LibraryRepositiory> libraries { get; set; }

        // public Dictionary<string, List<string>> libraries;


        public vsBoard board { get; set; }

        public vtTransferData(ProjectData project)
        {
            setupType = project.setupType;


            compilerBase = project.compilerBase;//.Replace('\\','/');



            libraries = new List<LibraryRepositiory>()
            {
                project.sharedLibraries,
                project.localLibraries,
            };

            if (project.coreBase != null)
            {
                coreBase = (project.copyCore || project.coreBase.StartsWith(project.path)) ? "\\core" : project.coreBase;
            }

            if (project.boardTxtPath != null)
            {
                boardTxtPath = (project.copyBoardTxt || project.boardTxtPath.StartsWith(project.path)) ? "\\boards.txt" : project.boardTxtPath;
            }

            board = new vsBoard(project.selectedBoard);
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
