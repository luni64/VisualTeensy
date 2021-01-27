using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using vtCore.Interfaces;

namespace vtCore
{
    static class C_CPP_Props_builder
    {
        public static string generate(IProject project, LibManager libManager, SetupData setup)
        {
            var cfg = project.selectedConfiguration;

            var props = new PropertiesJson()
            {
                configurations = new List<ConfigurationJson>()
                {
                    new ConfigurationJson()
                    {
                        name = "VisualTeensy",
                        compilerPath =  Path.Combine(cfg.compiler ,"arm-none-eabi-gcc.exe").Replace('\\','/'),
                        intelliSenseMode = "gcc-arm",
                        cppStandard = "gnu++14", // hack: might be better to extract from boards.txt
                        includePath = new List<string>()
                        {
                            $"{Path.Combine(setup.arduinoCoreBase,"cores",cfg.core).Replace('\\', '/') + "/**"}",
                            $"{Path.Combine(setup.arduinoBase, "hardware", "teensy", "avr", "libraries/**")}".Replace('\\','/'),
                            $"{Path.Combine(setup.arduinoBase ,"libraries/**")}".Replace('\\','/'),
                            $"{setup.sharedLibrariesFolder}/**".Replace('\\','/'),
                        },

                        forcedInclude = new List<string>()
                        {
                            Path.Combine(setup.arduinoCoreBase,"cores",cfg.core,"arduino.h").Replace('\\','/'),
                        },

                        defines = new List<string>()
                    }
                }
            };

            var options = cfg.selectedBoard.getAllOptions();
            var defines = options.FirstOrDefault(o => o.Key == "build.flags.defs");
            if (defines.Value != null)
            {
                foreach (var define in defines.Value.Split(new string[] { "-D" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    props.configurations[0].defines.Add(define.Trim());
                }
            }

            addConfigOption(options, props, "F_CPU=", "build.fcpu");
            addConfigOption(options, props, "", "build.usbtype");
            addConfigOption(options, props, "LAYOUT_", "build.keylayout");
            props.configurations[0].defines.Add("ARDUINO=18013");

            return JsonConvert.SerializeObject(props, Formatting.Indented);
        }

        private static void addConfigOption(Dictionary<string, string> options, PropertiesJson props, string prefix, string key)
        {
            var option = options.FirstOrDefault(o => o.Key == key).Value;

            if (option != null)
            {
                props.configurations[0].defines.Add(prefix + option);
            }
        }
    }
}
