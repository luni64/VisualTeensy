using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace vtCore
{
    static class C_CPP_Props_builder
    {
        public static string generate(IProject project, LibManager libManager, SetupData setup)
        {
            var props = new PropertiesJson()
            {
                configurations = new List<ConfigurationJson>()
                {
                    new ConfigurationJson()
                    {
                        name = "VisualTeensy",
                        compilerPath =  Path.Combine(project.selectedConfiguration.compilerBase ,"bin","arm-none-eabi-gcc.exe").Replace('\\','/'),
                        intelliSenseMode = "gcc-x64",
                        includePath = new List<string>()
                        {
                            project.selectedConfiguration.coreBase?.Replace('\\','/') + "/*",                         
                            $"{Path.Combine(setup.arduinoBase, "hardware", "teensy", "avr", "libraries/**")}".Replace('\\','/'),
                            $"{Path.Combine(setup.arduinoBase ,"libraries/**")}".Replace('\\','/'),
                            $"{Path.Combine(Helpers.getSketchbookFolder() ,"libraries/**")}".Replace('\\','/'),
                        },
                        forcedInclude = new List<string>()
                        {
                            Path.Combine(project.selectedConfiguration.coreBase,"arduino.h").Replace('\\','/'),
                        },
                        defines = new List<string>()
                    }
                }
            };

            var options = project.selectedConfiguration.selectedBoard.getAllOptions();
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
            props.configurations[0].defines.Add("ARDUINO");

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
