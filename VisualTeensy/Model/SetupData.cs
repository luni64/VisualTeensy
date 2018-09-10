using Board2Make.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board2Make.Model
{
    public class SetupData
    {
        public bool fromArduino { get; set; }

        public string projectName { get; set; }
        public string projectNameError
        {
            get
            {
                return String.IsNullOrWhiteSpace(projectName) ? "Error" : null;
            }
        }

        public string arduinoBase { get; set; }
        public string arduinoBaseError
        {
            get
            {
                return arduinoBase != null ? (Directory.Exists(arduinoBase) ? null : "Error") : "Error";
            }

        }

        public string projectBase { get; set; }
        public string projectBaseError
        {
            get
            {
                return (!String.IsNullOrWhiteSpace(projectBase) && (Directory.Exists(projectBase)) ? null : "Error");
            }

        }

        public string boardTxtPath
        {
            get => fromArduino ? FileHelpers.getBoardFromArduino(arduinoBase) : _boardTxt;
            set => _boardTxt = value;
        }
        string _boardTxt;
        public string boardTxtPathError
        {
            get
            {
                return (!String.IsNullOrWhiteSpace(boardTxtPath) && File.Exists(boardTxtPath)) ? null : "Error";
            }
        }
        public bool copyBoardTxt { get; set; }

        public string compilerBase
        {
            get
            {
                if (arduinoBaseError != null) return null;
                return fromArduino ? Path.Combine(FileHelpers.getToolsFromArduino(arduinoBase), "arm") : _compilerPath;
            }
            set => _compilerPath = value;
        }
        string _compilerPath;
        public string compilerPathError
        {
            get
            {
                return compilerBase != null ? (Directory.Exists(compilerBase) ? null : "Error") : "Error";
            }

        }

        public string makeExePath { get; set; }
        public string makeExePathError
        {
            get
            {
                return makeExePath != null ? (File.Exists(makeExePath) ? null : "Error") : "Error";
            }

        }

        public string coreBase
        {
            get => fromArduino ? FileHelpers.getCoreFromArduino(arduinoBase) : _corePath;
            set => _corePath = value;
        }
        string _corePath;
        public string corePathError
        {
            get
            {
                return coreBase != null ? (Directory.Exists(coreBase) ? null : "Error") : "Error";
            }
        }
        public bool copyCore { get; set; }

        public string uplPjrcBase
        {
            get => fromArduino ? FileHelpers.getToolsFromArduino(arduinoBase) : _uplPjrcBase;
            set => _uplPjrcBase = value;
        }
        string _uplPjrcBase;

        public string uplTyBase { get; set; }
        public string uplTyBaseError
        {
            get
            {
                return String.IsNullOrEmpty(uplTyBase) ? null : (Directory.Exists(uplTyBase) ? null : "error");
            }
        }


        public string makefile { get; set; }
        public string tasks_json { get; set; }
        public string propsFile { get; set; }


        public void loadSettings()
        {
            arduinoBase = Settings.Default.arduinoBase;
            makeExePath = Settings.Default.makeExePath;
            uplPjrcBase = Settings.Default.uplPjrcBase;
            uplTyBase = Settings.Default.uplTyBase;
            projectBase = Settings.Default.projectBase;
            boardTxtPath = Settings.Default.boardsTxtPath;
            coreBase = Settings.Default.coreBase;
            compilerBase = Settings.Default.compilerBase;
        }

        public void saveSettings()
        {
            bool oldState = fromArduino;
            fromArduino = false;

            Settings.Default.arduinoBase = arduinoBase;
            Settings.Default.makeExePath = makeExePath;
            Settings.Default.uplPjrcBase = uplPjrcBase;
            Settings.Default.uplTyBase = uplTyBase;
            Settings.Default.projectBase = projectBase;
            Settings.Default.boardsTxtPath = boardTxtPath;
            Settings.Default.coreBase = coreBase;
            Settings.Default.compilerBase = compilerBase;
            Settings.Default.Save();

            fromArduino = oldState;
        }     
    }
}
