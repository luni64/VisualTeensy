using System;
using System.Collections.Generic;
using System.IO;

namespace VisualTeensy.Model
{
    public enum SetupTypes
    {
        quick, expert
    }


    public class SetupData
    {
        public string projectBaseDefault { get; set; }
        public string projectBaseDefaultError => (!String.IsNullOrWhiteSpace(projectBaseDefault) && (Directory.Exists(projectBaseDefault)) ? null : "Error");

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
        public string uplTyBaseShort => (uplTyBase ?? "").Contains(" ") ? Helpers.getShortPath(uplTyBase) : uplTyBase;


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
        public string uplPjrcBaseShort
        {
            get
            {
                //string path = setupType == SetupTypes.quick ? FileHelpers.getToolsFromArduino() : uplPjrcBase;
                string path = uplPjrcBase;
                return (path ?? "").Contains(" ") ? Helpers.getShortPath(path) : path;
            }
        }

        public string arduinoBase { get; set; } = String.Empty;
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


        public string libBase { get; set; }
        public string libBaseShort => libBase.Contains(" ") ? Helpers.getShortPath(libBase) : libBase;

        public string sharedLibBase { get; set; }
        public string sharedLibBaseShort => (sharedLibBase ?? "").Contains(" ") ? Helpers.getShortPath(sharedLibBase) : sharedLibBase;


        public string makefile_fixed { get; set; }


        public string getCoreFromArduino()
        {
            if (String.IsNullOrWhiteSpace(arduinoBase)) { return null; }

            string path = Path.Combine(arduinoBase, "hardware", "teensy", "avr", "cores", "teensy3");
            return Directory.Exists(path) ? path : null;
        }

        public string getBoardFromArduino()
        {
            if (String.IsNullOrWhiteSpace(arduinoBase)) { return null; }

            string boardPath = Path.Combine(arduinoBase, "hardware", "teensy", "avr", "boards.txt");
            return File.Exists(boardPath) ? boardPath : null;
        }

        public string getToolsFromArduino()
        {
            if (String.IsNullOrWhiteSpace(arduinoBase)) { return null; }

            string path = Path.Combine(arduinoBase, "hardware", "tools");
            return Directory.Exists(path) ? path : null;
        }

        public string getCompilerFromArduino()
        {
            string tools = getToolsFromArduino();
            if (String.IsNullOrWhiteSpace(tools)) { return null; }

            string path = Path.Combine(tools, "arm");
            return Directory.Exists(path) ? path : null;
        }


        public static SetupData getDefault()
        {
            SetupData sd = new SetupData();

            sd.arduinoBase = Helpers.findArduinoFolder().Trim();
            Helpers.arduinoPath = sd.arduinoBase;

            sd.projectBaseDefault = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source");


            sd.uplPjrcBase = sd.getToolsFromArduino();
            sd.uplTyBase = Helpers.findTyToolsFolder();

            sd.makeExePath = Path.Combine(Directory.GetCurrentDirectory(), "make.exe");

            return sd;
        }
    }
}


