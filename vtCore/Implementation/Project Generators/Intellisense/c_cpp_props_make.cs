using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using vtCore.Interfaces;

namespace vtCore
{
    static class C_CPP_Props_make
    {
        public static string generate(IProject project, SetupData setup, LibManager libManager)
        {
            var cfg = project.selectedConfiguration;
            if (!cfg.isOk) return "ERROR";

;            var brd = cfg.selectedBoard;
           // if (project.selectedConfiguration.compilerBase == null || brd == null) return ""; // hack

            var props = new PropertiesJson()
            {
                configurations = new List<ConfigurationJson>()
                {
                    new ConfigurationJson()
                    {
                        name = "VisualTeensy",
                        compilerPath =  Path.Combine(cfg.compiler,"arm-none-eabi-gcc.exe").Replace('\\','/'),
                        intelliSenseMode = "gcc-x64",
                        includePath = new List<string>(),                        
                        defines = new List<string>()
                    }
                }
            };


            var cfgp = props.configurations[0];

            // include path -------------------------------------------------------------
            cfgp.includePath.Add("src/**");
            cfgp.includePath.Add(cfg.core);
           
            foreach (var lib in cfg.sharedLibs)
            {
                cfgp.includePath.Add(Path.Combine(lib.sourceUri.AbsolutePath, "**").Replace('\\', '/'));
            }

            foreach (var lib in cfg.localLibs)
            {
                cfgp.includePath.Add(Path.Combine("lib",lib.targetFolderName, "**").Replace('\\', '/'));
            }

            // Compiler switches ----------------------------------------------------------

            var options = project.selectedConfiguration?.selectedBoard?.getAllOptions();
            if (options == null) return "";
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
