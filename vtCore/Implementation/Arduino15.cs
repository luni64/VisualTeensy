using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;

namespace vtCore
{
    // Since Teensyduino 1.58 the Teensy platform is installed by the Arduino boards manager
    // (Arduino IDE 2.x, arduino-cli) into the Arduino15 data folder:
    //
    //   Arduino15/packages/teensy/hardware/avr/<version>            core, boards.txt, libraries
    //   Arduino15/packages/teensy/tools/teensy-compile/<version>/arm gcc
    //   Arduino15/packages/teensy/tools/teensy-tools/<version>       teensy.exe, teensy_post_compile...
    //
    // The tool versions belonging to a platform version are read from the platforms installed.json.
    public class Arduino15
    {
        public string platformVersion { get; }
        public string platformFolder { get; }
        public string boardsTxt { get; }
        public string compilerFolder { get; }
        public string toolsFolder { get; }
        public string error { get; }

        public static bool isArduino15Folder(string folder)
        {
            return !String.IsNullOrWhiteSpace(folder) && Directory.Exists(Path.Combine(folder, "packages"));
        }

        public static string defaultFolder => Helpers.arduinoAppPath;

        public Arduino15(string arduino15Folder)
        {
            string teensyPackage = Path.Combine(arduino15Folder, "packages", "teensy");
            string platforms = Path.Combine(teensyPackage, "hardware", "avr");

            platformFolder = newestVersionFolder(platforms, f => File.Exists(Path.Combine(f, "boards.txt")));
            if (platformFolder == null)
            {
                error = "Arduino15 folder doesn't contain a Teensy platform (packages/teensy/hardware/avr). Install it with the boards manager";
                return;
            }
            platformVersion = Path.GetFileName(platformFolder);
            boardsTxt = Path.Combine(platformFolder, "boards.txt");

            string compiler = toolFolder(teensyPackage, "teensy-compile");
            compilerFolder = compiler != null && File.Exists(Path.Combine(compiler, "arm", "bin", "arm-none-eabi-gcc.exe")) ? Path.Combine(compiler, "arm") : null;
            toolsFolder = toolFolder(teensyPackage, "teensy-tools");

            if (compilerFolder == null) error = $"Teensy platform {platformVersion}: compiler (packages/teensy/tools/teensy-compile) not found";
            else if (toolsFolder == null) error = $"Teensy platform {platformVersion}: tools (packages/teensy/tools/teensy-tools) not found";
        }

        // version required by the platform, fallback: newest installed version
        private string toolFolder(string teensyPackage, string toolName)
        {
            string toolBase = Path.Combine(teensyPackage, "tools", toolName);

            string version = requiredToolVersion(toolName);
            if (version != null && Directory.Exists(Path.Combine(toolBase, version)))
            {
                return Path.Combine(toolBase, version);
            }
            return newestVersionFolder(toolBase, f => true);
        }

        private string requiredToolVersion(string toolName)
        {
            try
            {
                string installedJson = Path.Combine(platformFolder, "installed.json");
                if (!File.Exists(installedJson)) return null;

                var platform = JObject.Parse(File.ReadAllText(installedJson))["packages"]?
                    .SelectMany(p => p["platforms"] ?? Enumerable.Empty<JToken>())
                    .FirstOrDefault(p => (string)p["version"] == platformVersion);

                return platform?["toolsDependencies"]?
                    .FirstOrDefault(t => (string)t["name"] == toolName)?["version"]?.ToString();
            }
            catch
            {
                return null;
            }
        }

        private static string newestVersionFolder(string baseFolder, Func<string, bool> isValid)
        {
            if (!Directory.Exists(baseFolder)) return null;

            return Directory.GetDirectories(baseFolder)
                .Where(isValid)
                .OrderByDescending(d => Version.TryParse(Path.GetFileName(d), out var v) ? v : new Version(0, 0))
                .FirstOrDefault();
        }

        // arduino-cli / IDE 2.x keep the sketchbook location in arduino-cli.yaml (directories: user: ...),
        // default is <Documents>/Arduino
        public static string findSketchbookFolder(string arduino15Folder)
        {
            var configs = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".arduinoIDE", "arduino-cli.yaml"),
                Path.Combine(arduino15Folder, "arduino-cli.yaml"),
            };

            foreach (var config in configs.Where(File.Exists))
            {
                bool inDirectories = false;
                foreach (var line in File.ReadAllLines(config))
                {
                    if (!line.StartsWith(" ") && !line.StartsWith("\t")) inDirectories = line.TrimEnd() == "directories:";
                    else if (inDirectories && line.Trim().StartsWith("user:"))
                    {
                        string path = line.Trim().Substring(5).Trim().Trim('"', '\'');
                        if (Directory.Exists(path)) return path;
                    }
                }
            }

            string defaultSketchbook = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Arduino");
            return Directory.Exists(defaultSketchbook) ? defaultSketchbook : null;
        }
    }
}
