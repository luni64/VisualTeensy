using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VisualTeensy.Model
{

    public class vtTransferData
    {
        public class vsBoard
        {
            public string name { get; set; }
            public Dictionary<string, string> options { get; set; } = new Dictionary<string, string>();
        }

        public class LibraryRepositiory
        {
            public string repository { get; set; }
           // public string path { get; set; }
            public List<string> libraries { get; set; }
        }

        public SetupTypes quickSetup { get; set; }

        //public string arduinoBase { get; set; }
        public string coreBase { get; set; }
        public string boardTxtPath { get; set; }
        public string compilerBase { get; set; }
        //public string makeExePath { get; set; }
        public string projectName { get; set; }
        public List<LibraryRepositiory> libraries { get; set; }


        public vsBoard board { get; set; }

        public vtTransferData(ProjectData project, /*SetupData setup,*/ Board _board)
        {
            //var oldSetup = project.setupType;
            //project.setupType = SetupTypes.expert;
            //quickSetup = oldSetup;           

            compilerBase = project.compilerBase;

            libraries = new List<LibraryRepositiory>()
            {
                new LibraryRepositiory()
                {
                    repository = "Shared",
                  //  path = setup.libBase,
                    libraries = project.libraries.Select(l => l.name).ToList(),
                },

                new LibraryRepositiory() ///ToDo not yet functional
                {
                    repository = "Local",
                  //  path = "lib",
                    //libraries = data.libraries.Select(l => l.name).ToList(),
                }
            };

            if (project.coreBase != null)
            {
                coreBase = (project.copyCore || project.coreBase.StartsWith(project.path)) ? "\\core" : project.coreBase;
            }

            if (project.boardTxtPath != null)
            {
                boardTxtPath = (project.copyBoardTxt || project.boardTxtPath.StartsWith(project.path)) ? "\\boards.txt" : project.boardTxtPath;
            }

            board = new vsBoard()
            {
                name = _board.name,
                options = _board?.optionSets?.ToDictionary(o => o.name, o => o.selectedOption?.name)
            };

            // project.setupType = oldSetup;
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
