using System;
using System.Collections.Generic;
using System.Drawing;
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

        // app folder
        static public string vtAppFolder { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "lunOptics", "VisualTeensy");

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

                        uplPjrcBase.path = arduinoTools;

                        var sketchBookFolder = Helpers.getSketchbookFolder();
                        if (sketchBookFolder != null)
                        {
                            if (!File.Exists(Helpers.preferencesPath))
                            {
                                errors.Add($"\n{Helpers.preferencesPath} not found. Please check your Arduino installation. In case of a fresh installation, run Arduino at least once to generate the file");
                            }
                            sharedLibrariesFolder = Path.Combine(Helpers.getSketchbookFolder(), "libraries");
                        }
                    }
                    else
                    {
                        arduinoCoreBase = arduinoBoardsTxt = arduinoTools = arduinoCompiler = null;
                        errors.Add($"Error checking Arduino folder: {arduinoBaseError}");
                    }
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
                    return $"Folder empty or doesn't exist ({arduinoBase})";
                }

                string arduinoExe = Path.Combine(arduinoBase, "arduino.exe");
                if (!File.Exists(arduinoExe))
                {
                    return "Arduino folder doesn't contain arduino.exe. Not a valid arduino folder";
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
        public string sharedLibrariesFolder { get; private set; }
        public string tdLibBase { get; set; }
        public string libIndex_json { get; } = Path.Combine(vtAppFolder, "library_index.json");
        public Uri libIndexSource { get; } = new Uri("https://downloads.arduino.cc/libraries/library_index.json", UriKind.Absolute);

        // settings for quick setup
        public string arduinoCoreBase { get; private set; }
        public string arduinoBoardsTxt { get; private set; }
        public string arduinoTools { get; private set; }
        public string arduinoCompiler { get; private set; }

        // misc
        public string makefile_fixed { get; set; }
        public string makefile_builder { get; set; }
        public bool debugSupportDefault { get; set; }
        public string preferencesPath { get; private set; }
        public MruList mru { get; } = new MruList(10);

        //makefile output colors ---------------
        public bool isColoredOutput { get; set; }
        public Color colorCore { get; set; }
        public Color colorUserLib { get; set; }
        public Color colorUserSrc { get; set; }
        public Color colorLink { get; set; }
        public Color colorErr { get; set; }
        public Color colorOk { get; set; }

        public static SetupData getDefault()
        {
            var sd = new SetupData();

            //sd.arduinoBase = Helpers.findArduinoFolder().Trim();
            //Helpers.arduinoPath = sd.arduinoBase;

            sd.projectBaseDefault = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source");

            //sd.uplPjrcBase.path = sd.arduinoTools;
            //sd.uplTyBase.path = Helpers.findTyToolsFolder();
            //sd.uplCLIBase.path = Helpers.findCLIFolder();
            //sd.uplJLinkBase.path = Helpers.findJLinkFolder();

            sd.makeExeBase.path = Directory.GetCurrentDirectory();
            sd.debugSupportDefault = false;

            sd.colorCore = Color.FromArgb(255, 187, 206, 251);
            sd.colorUserLib = Color.FromArgb(255, 206, 244, 253);
            sd.colorUserSrc = Color.FromArgb(255, 100, 149, 237);
            sd.colorOk = Color.FromArgb(255, 179, 255, 179);
            sd.colorLink = Color.FromArgb(255, 255, 255, 202);
            sd.colorErr = Color.FromArgb(255, 255, 159, 159);            

            return sd;
        }

        public List<string> errors { get; } = new List<string>();
    }
}


