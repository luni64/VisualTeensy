using System.Collections.Generic;
using System.Linq;

namespace Board2Make.Model
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
            boardTxtPath = data.boardTxtPath;
            compilerBase = data.compilerBase;
            makeExePath = data.makeExePath;

            board = new vsBoard()
            {
                name = _board.name,
                options = _board?.optionSets?.ToDictionary(o => o.name, o => o.selectedOption?.name)
            };

            data.fromArduino = oldSetup;
        }

        public vtTransferData() { }

    }
}
