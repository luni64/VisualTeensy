using VisualTeensy.Properties;
using System;
using System.Collections.Generic;
using System.IO;

namespace VisualTeensy.Model
{
    public class SetupData
    {
        public bool fromArduino { get; set; }

        public string projectName => Path.GetFileName(projectBase ?? "ERROR");

        public string arduinoBase { get; set; }
        public string arduinoBaseError
        {
            get
            {
                if (String.IsNullOrEmpty(arduinoBase) || !Directory.Exists(arduinoBase))
                {
                    return "Folder doesn't exist";
                }

                string arduinoExe = Path.Combine(arduinoBase, "arduino.exe");
                if (!File.Exists(arduinoExe))
                {
                    return "Folder doesn't contain arduino.exe. Not a valid arduino folder";
                }

                string teensyduino = Path.Combine(arduinoBase, "hardware", "teensy");
                if (!Directory.Exists(teensyduino))
                {
                    return "Arduino folder doesn't contain a ./hardware/teensy folder. Looks like Teensyduino is not installed";
                }

                return null;
            }
        }

        public string projectBase { get; set; }
        public string projectBaseError
        {
            get
            {
                try
                {
                    Path.GetFullPath(projectBase);
                    return null;
                }
                catch { return "Path to the project folder not valid"; }
            }
        }

        public string projectBaseDefault { get; set; }
        public string projectBaseDefaultError => (!String.IsNullOrWhiteSpace(projectBaseDefault) && (Directory.Exists(projectBaseDefault)) ? null : "Error");

        public string boardTxtPath
        {
            get => fromArduino ? FileHelpers.getBoardFromArduino(arduinoBase) : _boardTxt;
            set => _boardTxt = value;
        }
        string _boardTxt;
        public string boardTxtPathError => (!String.IsNullOrWhiteSpace(boardTxtPath) && File.Exists(boardTxtPath)) ? null : "Error";
        public bool copyBoardTxt { get; set; }

        public string compilerBase
        {
            get => fromArduino ? Path.Combine(FileHelpers.getToolsFromArduino(arduinoBase) ?? "", "arm") : _compilerPath;
            set => _compilerPath = value;
        }
        string _compilerPath;
        public string compilerPathError
        {
            get
            {
                if (!String.IsNullOrEmpty(compilerBase) && (Directory.Exists(compilerBase)))
                {
                    string gcc = Path.Combine(compilerBase, @"bin\arm-none-eabi-gcc.exe");
                    if (File.Exists(gcc))
                    {
                        return null;
                    }
                    return @".\bin\arm-none-eabi-gcc.exe not found in the specified directory. Please select a valid arm-none-eabi gcc folder";
                }
                return "Folder doesn't exist";
            }
        }
        public string compilerBaseShort => compilerBase.Contains(" ") ? FileHelpers.getShortPath(compilerBase) : compilerBase;


        public string libBase
        {
            get =>  Path.Combine(arduinoBase, "hardware", "teensy", "avr", "libraries") ;           
        }                
        public string libBaseShort => libBase.Contains(" ") ? FileHelpers.getShortPath(libBase) : libBase;

        public string makeExePath { get; set; }
        public string makeExePathError => makeExePath != null ? (File.Exists(makeExePath) ? null : "Error") : "Error";

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
                if (!String.IsNullOrEmpty(coreBase) && (Directory.Exists(coreBase)))
                {
                    string uploader = Path.Combine(coreBase, "Arduino.h");
                    if (File.Exists(uploader))
                    {
                        return null;
                    }
                    return "Arduino.h not found in the specified folder. Doesn't seem to be valid arduino core";
                }
                return "Folder doesn't exist";
            }
        }

        public bool copyCore { get; set; }
        public string coreBaseShort => coreBase.Contains(" ") ? FileHelpers.getShortPath(coreBase) : coreBase;

        public string uplPjrcBase { get; set; }
        public string uplPjrcBaseError
        {
            get
            {
                if (!String.IsNullOrEmpty(uplPjrcBase) && (Directory.Exists(uplPjrcBase)))
                {
                    string uploader = Path.Combine(uplPjrcBase, "teensy.exe");
                    if (File.Exists(uploader))
                    {
                        return null;
                    }
                    return "Teensy.exe not found in the specified directory";
                }
                return "Folder doesn't exist";
            }
        }

        public string uplPjrcBaseShort
        {
            get
            {
                string path = fromArduino ? FileHelpers.getToolsFromArduino(arduinoBase) : uplPjrcBase;
                return (path ?? "").Contains(" ") ? FileHelpers.getShortPath(path) : path;
            }
        }

        public string uplTyBase { get; set; }
        public string uplTyBaseError
        {
            get
            {
                if (String.IsNullOrEmpty(uplTyBase)) return null; // setting is optional

                if (Directory.Exists(uplTyBase))
                {
                    string uploader = Path.Combine(uplTyBase, "TyCommanderC.exe");
                    string gui = Path.Combine(uplTyBase, "TyCommander.exe");
                    if (File.Exists(uploader) && File.Exists(gui))
                    {
                        return null;
                    }
                    return "TyCommanderC.exe or TyCommander.exe not found in the specified folder";
                }
                return "Folder doesn't exist";
            }
        }
        public string uplTyBaseShort => (uplTyBase ?? "").Contains(" ") ? FileHelpers.getShortPath(uplTyBase) : uplTyBase;

        public string sharedLibBase { get; set; }
        public string sharedLibBaseShort => (sharedLibBase ?? "").Contains(" ") ? FileHelpers.getShortPath(sharedLibBase) : sharedLibBase;

        public string makefile { get; set; }
        public string tasks_json { get; set; }
        public string props_json { get; set; }
        public string vsSetup_json { get; set; }

        public List<Library> libraries { get; } = new List<Library>();


        public string makefile_fixed { get; set; }

        public void loadSettings()
        {
            arduinoBase = Settings.Default.arduinoBase;
            makeExePath = Settings.Default.makeExePath;
            uplPjrcBase = Settings.Default.uplPjrcBase;
            uplTyBase = Settings.Default.uplTyBase;
            projectBase = Settings.Default.projectBase;
            //   projectName = Settings.Default.projectName;
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
            // Settings.Default.projectName = projectName;
            Settings.Default.boardsTxtPath = boardTxtPath;
            Settings.Default.coreBase = coreBase;
            Settings.Default.compilerBase = compilerBase;
            Settings.Default.Save();

            fromArduino = oldState;
        }
    }
}
