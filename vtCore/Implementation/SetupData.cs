using System;
using System.IO;

namespace vtCore
{
   
    
    public class SetupData
    {
        // project path
        public string projectBaseDefault { get; set; }
        public string projectBaseDefaultError => (!String.IsNullOrWhiteSpace(projectBaseDefault) && (Directory.Exists(projectBaseDefault)) ? null : "Error");

        // gnu make
        public string makeExePath { get; set; }
        public string makeExePathError => makeExePath != null ? (File.Exists(makeExePath) ? null : "Error") : "Error";

        // upload TyTools
        public string uplTyBase { get; set; }
        public string uplTyBaseError
        {
            get
            {
                if (String.IsNullOrEmpty(uplTyBase))
                {
                    return null; // setting is optional
                }

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
       
        // upload PJRC 
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

        // upload PJRC 
        public string uplCLIBase { get; set; }
        public string uplCLIBaseError
        {
            get
            {
                if (!String.IsNullOrEmpty(uplCLIBase) && (Directory.Exists(uplCLIBase)))
                {
                    string uploader = Path.Combine(uplCLIBase, "teensy_loader_cli.exe");
                    if (File.Exists(uploader))
                    {
                        return null;
                    }
                    return "Teensy_loader_cli.exe not found in the specified directory";
                }
                return "Folder doesn't exist";
            }
        }

        // arduinoBase
        public string arduinoBase
        {
            get => _arduinoBase;
            set
            {
                if (value != _arduinoBase)
                {
                    _arduinoBase = value;

                    if (arduinoBaseError == null)
                    {
                        string path = Path.Combine(arduinoBase, "hardware", "teensy", "avr", "cores", "teensy3");
                        arduinoCore = Directory.Exists(path) ? path : null;

                        path = Path.Combine(arduinoBase, "hardware", "teensy", "avr", "boards.txt");
                        arduinoBoardsTxt = File.Exists(path) ? path : null;

                        path = Path.Combine(arduinoBase, "hardware", "tools");
                        arduinoTools = Directory.Exists(path) ? path : null;

                        path = Path.Combine(arduinoBase, "hardware", "tools", "arm");
                        arduinoCompiler = Directory.Exists(path) ? path : null;
                    }
                }
                else
                {
                    arduinoCore = arduinoBoardsTxt = arduinoTools = arduinoCompiler = null;
                }
            }
        }
        string _arduinoBase = String.Empty;
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

        // libraries
        public string libBase { get; set; }      
        
        // settings for quick setup
        public string arduinoCore { get; private set; }
        public string arduinoBoardsTxt { get; private set; }
        public string arduinoTools { get; private set; }
        public string arduinoCompiler { get; private set; }

        // misc
        public string makefile_fixed { get; set; }
        public string makefile_builder { get; set; }
      

        public static SetupData getDefault()
        {
            SetupData sd = new SetupData();

            sd.arduinoBase = Helpers.findArduinoFolder().Trim();
            Helpers.arduinoPath = sd.arduinoBase;

            sd.projectBaseDefault = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source");
            
            sd.uplPjrcBase = sd.arduinoTools;
            sd.uplTyBase = Helpers.findTyToolsFolder();
            sd.uplCLIBase = Helpers.findCLIFolder();

            sd.makeExePath = Path.Combine(Directory.GetCurrentDirectory(), "make.exe");

            return sd;
        }
    }
}


