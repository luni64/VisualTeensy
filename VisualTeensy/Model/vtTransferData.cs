using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VisualTeensy.Model
{
    public class vsBoard
    {
        public string name { get; set; }
        public Dictionary<string, string> options { get; set; } = new Dictionary<string, string>();
    }

    public class vtTransferData
    {
        public bool quickSetup { get; set; }

        public string arduinoBase { get; set; }
        public string coreBase { get; set; }
        public string boardTxtPath { get; set; }
        public string compilerBase { get; set; }
        public string makeExePath { get; set; }
        public string projectName { get; set; }

        public vsBoard board { get; set; }

        public vtTransferData(SetupData data, Board _board)
        {
            bool oldSetup = data.fromArduino;
            data.fromArduino = false;

            quickSetup = oldSetup;
            arduinoBase = data.arduinoBase;
            
            compilerBase = data.compilerBase;
            makeExePath = data.makeExePath;
            

            if(data.coreBase != null)
            {
                coreBase = (data.copyCore || data.coreBase.StartsWith(data.projectBase)) ? "\\core" : data.coreBase;
            }

            if (data.boardTxtPath != null)
            {
                boardTxtPath = (data.copyBoardTxt || data.boardTxtPath.StartsWith(data.projectBase)) ? "\\boards.txt" : data.boardTxtPath;
            }
            
            board = new vsBoard()
            {
                name = _board.name,
                options = _board?.optionSets?.ToDictionary(o => o.name, o => o.selectedOption?.name)
            };

            data.fromArduino = oldSetup;
        }

        public vtTransferData() { }


        string makeRelative(string path, string basePath)
        {
            if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(basePath)) return path;

            if (Path.GetFullPath(path).StartsWith(Path.GetFullPath(basePath)))
            {
                var p1 = new System.Uri(path);
                var baseUri = new System.Uri(basePath);

                return p1.MakeRelativeUri(baseUri).ToString();
            }
            else return path;
        }

    }
}
