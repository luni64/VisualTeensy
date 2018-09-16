using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Linq;

namespace Board2Make.Model
{
    static class FileHelpers
    {
        public static string getBoardFromArduino(string arduinoPath)
        {
            if (arduinoPath == null)
            {
                return null;
            }

            string boardPath = Path.Combine(arduinoPath, "hardware", "teensy", "avr", "boards.txt");
            return File.Exists(boardPath) ? boardPath : null;
        }
        public static string getToolsFromArduino(string arduinoPath)
        {
            if (String.IsNullOrWhiteSpace(arduinoPath))
            {
                return null;
            }

            string path = Path.Combine(arduinoPath, "hardware", "tools");
            return Directory.Exists(path) ? path : null;
        }
        public static string getCoreFromArduino(string arduinoPath)
        {
            if (arduinoPath == null)
            {
                return null;
            }

            string path = Path.Combine(arduinoPath, "hardware", "teensy", "avr", "cores", "teensy3");
            return Directory.Exists(path) ? path : null;
        }

        public static bool isArduinoFolder(string folder)
        {
            if (folder == null || !Directory.Exists(folder)) return false;

            var arduinoExe = Path.Combine(folder, "arduino.exe");
            if (!File.Exists(arduinoExe)) return false;

            var boardsTxt = Path.Combine(folder, "hardware" , "teensy", "avr", "boards.txt");
            if (!File.Exists(boardsTxt)) return false;

            return true;
        }

        public static string findArduinoFolder()
        {
            string programFiles = @"C:\Program Files";

            if (Directory.Exists(programFiles))
            {
                foreach (string dir in Directory.GetDirectories(programFiles).Where(d => d.Contains("Arduino")))
                {
                    if (isArduinoFolder(dir)) return dir;
                }
            }

            programFiles = @"C:\Program Files (x86)";
            if (Directory.Exists(programFiles))
            {
                foreach (string dir in Directory.GetDirectories(programFiles).Where(d => d.Contains("Arduino")))
                {
                    if (isArduinoFolder(dir)) return dir;
                }
            }

            foreach (string dir in Directory.GetDirectories(@"C:\").Where(d => d.Contains("Arduino")))
            {
                if (isArduinoFolder(dir)) return dir;
                foreach (string subDir in Directory.GetDirectories(dir).Where(d => d.Contains("Arduino")))
                {
                    if (isArduinoFolder(subDir)) return subDir;
                }
            }

            return null;


        }


        public static string FormatOutput(string jsonString)
        {
            var stringBuilder = new StringBuilder();

            bool escaping = false;
            bool inQuotes = false;
            int indentation = 0;

            foreach (char character in jsonString)
            {
                if (escaping)
                {
                    escaping = false;
                    stringBuilder.Append(character);
                }
                else
                {
                    if (character == '\\')
                    {
                        escaping = true;
                        stringBuilder.Append(character);
                    }
                    else if (character == '\"')
                    {
                        inQuotes = !inQuotes;
                        stringBuilder.Append(character);
                    }
                    else if (!inQuotes)
                    {
                        if (character == ',')
                        {
                            stringBuilder.Append(character);
                            stringBuilder.Append("\n");
                            stringBuilder.Append(' ', indentation);
                            stringBuilder.Append(' ', indentation);
                        }
                        else if (character == '[' || character == '{')
                        {
                            stringBuilder.Append(character);
                            stringBuilder.Append("\r\n");
                            stringBuilder.Append(' ', ++indentation);
                            stringBuilder.Append(' ', indentation);
                        }
                        else if (character == ']' || character == '}')
                        {
                            stringBuilder.Append("\r\n");
                            stringBuilder.Append(' ', --indentation);
                            stringBuilder.Append(' ', indentation);
                            stringBuilder.Append(character);
                        }
                        else if (character == ':')
                        {
                            stringBuilder.Append(character);
                            stringBuilder.Append(' ');
                            //stringBuilder.Append(' ');
                        }
                        else
                        {
                            stringBuilder.Append(character);
                        }
                    }
                    else
                    {
                        stringBuilder.Append(character);
                    }
                }
            }

            return stringBuilder.ToString();
        }

        [DllImport("kernel32", EntryPoint = "GetShortPathName", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetShortPathName(string longPath, StringBuilder shortPath, int bufSize);

        public static string getShortPath(string longPath)
        {
            const int maxPath = 255;

            StringBuilder shortPath = new StringBuilder(maxPath);
            int i = GetShortPathName(longPath, shortPath, maxPath);
            return i > 0 ? shortPath.ToString() : "ERROR IN PATH";

        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetLongPathName(string path, StringBuilder longPath, int longPathLength);
    }
}
