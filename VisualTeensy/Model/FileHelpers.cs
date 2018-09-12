using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

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


        /// <summary>
        /// Adds indentation and line breaks to output of JavaScriptSerializer
        /// </summary>
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
            GetShortPathName(longPath, shortPath, maxPath);
            return shortPath.ToString();

        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetLongPathName(string path, StringBuilder longPath, int longPathLength);
    }
}
