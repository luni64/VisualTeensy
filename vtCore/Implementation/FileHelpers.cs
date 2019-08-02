using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace vtCore
{
    public static class Helpers
    {
        public static string arduinoPath { set; get; }


        //Folders ----------------------------------------

        public static string arduinoAppPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Arduino15");

        public static string getSketchbookFolder()
        {
            var preferencesPath = Path.Combine(arduinoAppPath, "preferences.txt");

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
                return Path.Combine(folder, "JLINK64");
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
        public async static Task downloadLibrary(Library lib, DirectoryInfo libBase)
        {
            if(!libBase.Exists) libBase.Create();

            var libDir = new DirectoryInfo(Path.Combine(libBase.FullName, lib.unversionedLibFolder));            
            if (libDir.Exists) libDir.Delete(true);

            var tempFile = new FileInfo(Path.GetTempFileName());
            var tempFolder = new DirectoryInfo(Path.Combine(Path.GetTempPath(), "vslib"));
            if (tempFolder.Exists) tempFolder.Delete(true);

            WebClient wclient = null;
            try
            {
                Console.WriteLine(lib.name);

                wclient = new WebClient();
                wclient.Proxy = null;                
                wclient.DownloadProgressChanged += Wclient_DownloadProgressChanged1;
                
                await wclient.DownloadFileTaskAsync(lib.url, tempFile.FullName);
                await Task.Run(() => ZipFile.ExtractToDirectory(tempFile.FullName, tempFolder.FullName));
                tempFile.Delete();

                var sourceFolder = tempFolder.GetDirectories().FirstOrDefault();
                sourceFolder.MoveTo(libDir.FullName);
                tempFolder.Delete(true);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                wclient.DownloadProgressChanged -= Wclient_DownloadProgressChanged1;
                wclient?.Dispose();
            }



            //    WebClient client = null;
            //MemoryStream zippedStream = null;
            //ZipArchive libArchive = null;
            //try
            //{
            //    client = new WebClient();                
            //    zippedStream = new MemoryStream(client.DownloadData(lib.url));
            //    libArchive = new ZipArchive(zippedStream);
            //    ZipFileExtensions.ExtractToDirectory(libArchive, targetFolder);

            //    Directory.Move(versionedLibFolder, unversionedLibFolder);
            //    return true;
            //}
            //catch //(Exception ex)
            //{
            //    return false;
            //}
            //finally
            //{
            //    if (Directory.Exists(versionedLibFolder)) Directory.Delete(versionedLibFolder);
            //    client?.Dispose();
            //    zippedStream?.Dispose();
            //    libArchive?.Dispose();
            //}
        }

        private static void Wclient_DownloadProgressChanged1(object sender, DownloadProgressChangedEventArgs e)
        {            
            Console.WriteLine($"{e.BytesReceived} {e.ProgressPercentage}%");
        }

       
        private static void Wclient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine(e.BytesReceived);
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