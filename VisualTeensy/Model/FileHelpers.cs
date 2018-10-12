﻿using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace VisualTeensy.Model
{
    static class Helpers
    {
        public static string arduinoPath { set; get; }


        //Folders ----------------------------------------

        public static string arduinoPrefsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Arduino15");

        public static string getSketchbookFolder()
        {
            var preferencesPath = Path.Combine(arduinoPrefsPath, "preferences.txt");

            string sketchbookPath = "";

            if (File.Exists(preferencesPath))
            {
                using (TextReader reader = new StreamReader(preferencesPath))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        var parts = line.Split('=');

                        if (parts.Length == 2 && parts[0] == "sketchbook.path")
                        {
                            sketchbookPath = parts[1];
                            break;
                        }
                    }
                }
            }
            return sketchbookPath;
        }

        public static string findArduinoFolder()
        {
            string folder;

            folder = checkFolder(@"C:\Program Files", f => isArduinoFolder(f));
            if (folder != null)
            {
                return folder;
            }

            folder = checkFolder(@"C:\Program Files (x86)", f => isArduinoFolder(f));
            if (folder != null)
            {
                return folder;
            }

            return null;
        }
        public static string findTyToolsFolder()
        {
            string folder;

            folder = checkFolder(@"C:\Program Files", f => isTyToolsFolder(f));
            if (folder != null)
            {
                return folder;
            }

            folder = checkFolder(@"C:\Program Files (x86)", f => isTyToolsFolder(f));
            if (folder != null)
            {
                return folder;
            }

            return null;
        }
        public static string findCLIFolder()
        {
            string folder;

            folder = checkFolder(@"C:\Users", f => isCLIFolder(f));
            if (folder != null)
            {
                return folder;
            }

            return null;
        }

        // Download libraries ----------------------------
        public static bool downloadLibrary(Library lib, string targetFolder)
        {
            string libFolder = Path.Combine(targetFolder, lib.name);
            if (Directory.Exists(libFolder)) return false;
            
            WebClient client = null;
            MemoryStream zippedStream = null;
            ZipArchive libArchive = null;
            try
            {
                client = new WebClient();                
                zippedStream = new MemoryStream(client.DownloadData(lib.url));
                libArchive = new ZipArchive(zippedStream);
                
                
                ZipFileExtensions.ExtractToDirectory(libArchive, targetFolder);

                var extDir = Path.Combine(targetFolder,  Path.GetFileNameWithoutExtension(lib.url));

                Directory.Move(extDir, libFolder);



                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
            finally
            {
                client?.Dispose();
                zippedStream?.Dispose();
                libArchive?.Dispose();
            }
        }


        private static bool isArduinoFolder(string folder)
        {
            if (folder == null || !Directory.Exists(folder))
            {
                return false;
            }

            var arduinoExe = Path.Combine(folder, "arduino.exe");
            if (!File.Exists(arduinoExe))
            {
                return false;
            }

            var boardsTxt = Path.Combine(folder, "hardware", "teensy", "avr", "boards.txt");
            if (!File.Exists(boardsTxt))
            {
                return false;
            }

            return true;
        }
        private static bool isTyToolsFolder(string folder)
        {
            if (String.IsNullOrWhiteSpace(folder) || Path.GetFileName(folder) != "TyTools" || !Directory.Exists(folder))
            {
                return false;
            }

            var tyCommanderC = Path.Combine(folder, "TyCommanderC.exe");
            return (File.Exists(tyCommanderC));
        }
        private static bool isCLIFolder(string folder)
        {
            var cli = Path.Combine(folder, "teensy_loader_cli.exe");
            return (File.Exists(cli));
        }

        private static string checkFolder(string baseFolder, Predicate<string> isValid)
        {
            if (Directory.Exists(baseFolder))
            {
                foreach (string dir in Directory.GetDirectories(baseFolder).Where(d => !d.Contains("Recycle.Bin")))
                {
                    if (isValid(dir))
                    {
                        return dir;
                    }

                    try
                    {
                        foreach (string subDir in Directory.GetDirectories(dir))
                        {
                            if (isValid(subDir))
                            {
                                return subDir;
                            }
                        }
                    }
                    catch { }
                }
            }
            return null;
        }

        [DllImport("kernel32", EntryPoint = "GetShortPathName", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetShortPathName(string longPath, StringBuilder shortPath, int bufSize);
        public static string getShortPath(string longPath)
        {

            if (longPath == null || !longPath.Contains(' '))
            {
                return longPath;
            }

            const int maxPath = 255;
            StringBuilder shortPath = new StringBuilder(maxPath);
            int i = GetShortPathName(longPath, shortPath, maxPath);
            return i > 0 ? shortPath.ToString() : "ERROR IN PATH";
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int GetLongPathName(string path, StringBuilder longPath, int longPathLength);
    }
}