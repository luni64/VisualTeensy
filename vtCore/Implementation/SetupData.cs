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
        public CheckedPath makeExeBase { get; } = new CheckedPath("make.exe");
       
        // uploaders
        public CheckedPath uplPjrcBase { get; } = new CheckedPath("teensy.exe");             // upload PJRC 
        public CheckedPath uplTyBase { get; } = new CheckedPath("TyCommanderC.exe");         // upload TyTools        
        public CheckedPath uplJLinkBase { get; } = new CheckedPath("JLink.exe");             // upload JLink
        public CheckedPath uplCLIBase { get; } = new CheckedPath("teensy_loader_cli.exe");   // upload PJRC      

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
                        string path = Path.Combine(arduinoBase, "hardware", "teensy", "avr");
                        arduinoCoreBase = Directory.Exists(path) ? path : null;

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
                    arduinoCoreBase = arduinoBoardsTxt = arduinoTools = arduinoCompiler = null;
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
        public string tdLibBase { get; set; }

        // settings for quick setup
        public string arduinoCoreBase { get; private set; }
        public string arduinoBoardsTxt { get; private set; }
        public string arduinoTools { get; private set; }
        public string arduinoCompiler { get; private set; }

        // misc
        public string makefile_fixed { get; set; }
        public string makefile_builder { get; set; }
        public bool debugSupportDefault { get; set; }


        public static SetupData getDefault()
        {
            var sd = new SetupData();
            
            sd.arduinoBase = Helpers.findArduinoFolder().Trim();
            Helpers.arduinoPath = sd.arduinoBase;

            sd.projectBaseDefault = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source");

            sd.uplPjrcBase.path = sd.arduinoTools;
            sd.uplTyBase.path = Helpers.findTyToolsFolder();
            sd.uplCLIBase.path = Helpers.findCLIFolder();
            sd.uplJLinkBase.path = Helpers.findJLinkFolder();
            
            sd.makeExeBase.path = Directory.GetCurrentDirectory();
            sd.debugSupportDefault = false;

            return sd;
        }
    }
}


