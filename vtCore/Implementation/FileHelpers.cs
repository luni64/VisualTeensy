﻿using LibGit2Sharp;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using vtCore.Interfaces;

namespace vtCore
{
    public static class Helpers
    {
        static readonly HttpClient client = new HttpClient();

        public static string arduinoPath { set; get; }

        //Folders ----------------------------------------

        public static string arduinoAppPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Arduino15");
        public static string getPreferencesPath(string arduinoPath)
        {
            String prefPath = Path.Combine(arduinoPath, "portable", "preferences.txt");
            return File.Exists(prefPath) ? prefPath : Path.Combine(arduinoAppPath, "preferences.txt");
        }
        public static string getSketchbookFolder(string arduinoBase = "")
        {
            string sketchbookPath = "";

            string preferencesPath = getPreferencesPath(arduinoBase);
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
                            if (Path.GetDirectoryName(preferencesPath).EndsWith("portable"))
                            {
                                sketchbookPath = Path.Combine(arduinoBase, "portable", sketchbookPath);
                            }
                            break;
                        }
                    }
                }
            }

            return sketchbookPath;
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
        public static string findJLinkFolder()
        {
            string folder;

            folder = checkFolder(@"C:\Program Files", f => isJLinkFolder(f));
            if (folder != null)
            {
                return Path.Combine(folder, "JLINK64");
            }

            folder = checkFolder(@"C:\Program Files (x86)", f => isJLinkFolder(f));
            if (folder != null)
            {
                return Path.Combine(folder, "JLink_V646j");
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

            //var boardsTxt = Path.Combine(folder, "hardware", "teensy", "avr", "boards.txt");
            //if (!File.Exists(boardsTxt))
            //{
            //    return false;
            //}

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
        private static bool isJLinkFolder(string folder)
        {
            if (String.IsNullOrWhiteSpace(folder) || Path.GetFileName(folder) != "SEGGER" || !Directory.Exists(folder))
            {
                return false;
            }

            var jlink = Path.Combine(folder, "JLink_V646j", "JLink.exe");
            return (File.Exists(jlink));
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

        public static void copyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                copyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            }

            foreach (FileInfo file in source.GetFiles())
            {
                string targetFileName = Path.Combine(target.FullName, file.Name);
                if (!File.Exists(targetFileName))
                {
                    file.CopyTo(Path.Combine(target.FullName, file.Name));
                }
            }
        }

        public static void copyFilesRecursively(Uri s, Uri t)
        {
            if (s == t) return;

            DirectoryInfo source = new DirectoryInfo(s.LocalPath);
            DirectoryInfo target = new DirectoryInfo(t.LocalPath);

            if (target.Exists) target.Delete(true);
            copyFilesRecursively(source, target);
        }


        public static async Task downloadFileAsync(Uri source, string target, TimeSpan expiry = default)
        {
            if (File.Exists(target))
            {
                var exp = DateTime.Now - File.GetLastWriteTime(target);

                if ((DateTime.Now - File.GetLastWriteTime(target)) < expiry)
                {
                    log.Info($"Skip downloading {source} to {target}, {target} is newer than {expiry.Days} days");
                    return;
                }
            }

            log.Info($"Try downloading {source} to {target}");
            try
            {
                try
                {
                    WebClient client = new WebClient();
                    await client.DownloadFileTaskAsync(source, target);
                }
                catch
                {
                    log.Error($"Error downloading index file {source}");
                }
            }
            catch
            {
                log.Error($"Error downloading {target}");
            }
        }


        public async static Task downloadLibrary(IProjectLibrary lib, DirectoryInfo libBase)
        {
            if (!libBase.Exists) libBase.Create();


            var libDir = new DirectoryInfo(Path.Combine(libBase.FullName, lib.targetFolder));
            if (libDir.Exists) libDir.Delete(true);

            // we will save the *.zip in a temp file and unzip into %temp%/vslib            
            var tempFolder = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "vslib"));
            if (tempFolder.Exists) tempFolder.Delete(true);

            try
            {
                Console.Write($"Read {lib.name}... ");

                using (var response = await client.GetAsync(lib.sourceUri))
                {
                    response.EnsureSuccessStatusCode();

                    using (var s = await response.Content.ReadAsStreamAsync())
                    {
                        using (var archive = new ZipArchive(s))
                        {
                            archive.ExtractToDirectory(tempFolder.FullName);
                        }
                    }
                }

                var sourceFolder = tempFolder.GetDirectories().FirstOrDefault();
                copyFilesRecursively(sourceFolder, libDir);
                tempFolder.Delete(true);

                Console.WriteLine("done");
            }
            catch (Exception)
            {
                throw;
            }
        }

      


        [DllImport("kernel32", EntryPoint = "GetShortPathName", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int getShortPathName(string longPath, StringBuilder shortPath, int bufSize);
        public static string getShortPath(string longPath)
        {
            if (String.IsNullOrWhiteSpace(longPath)) return "";
            if (!longPath.Contains(' ')) return longPath;

            const int maxPath = 255;
            StringBuilder shortPath = new StringBuilder(maxPath);
            int i = getShortPathName(longPath, shortPath, maxPath);
            return i > 0 ? shortPath.ToString() : "ERROR IN PATH";
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern int getLongPathName(string path, StringBuilder longPath, int longPathLength);

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}