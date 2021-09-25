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

            ; var brd = cfg.selectedBoard;
            // if (project.selectedConfiguration.compilerBase == null || brd == null) return ""; // hack

            var props = new PropertiesJson()
            {
                configurations = new List<ConfigurationJson>()
                {
                    new ConfigurationJson()
                    {
                        name = "VisualTeensy",
                        compilerPath =  Path.Combine(cfg.compiler,"arm-none-eabi-gcc.exe").Replace('\\','/'),
                        intelliSenseMode = "gcc-arm",
                        cppStandard = "gnu++14", // hack: might be better to extract from boards.txt
                        includePath = new List<string>(),
                        defines = new List<string>()
                    }
                }
            };
            var cfgp = props.configurations[0];

            // include path -------------------------------------------------------------


            cfgp.includePath.Add("src");

            string coresPath = "cores/" + cfg.selectedBoard.core;
            if (cfg.coreStrategy == LibStrategy.link)
            {
                coresPath = cfg.coreBase.path + "/" + coresPath;
            }
            cfgp.includePath.Add(coresPath.Replace('\\', '/'));


            foreach (var lib in cfg.sharedLibs)
            {
                string basePath = lib.sourceUri.LocalPath.Replace('\\', '/');                                                
                cfgp.includePath.Add(basePath);

                var utiliyPath = basePath + "/utility";
                if (Directory.Exists(utiliyPath)) cfgp.includePath.Add(utiliyPath);

                var srcPath = basePath + "/src";
                if(Directory.Exists(srcPath)) cfgp.includePath.Add(srcPath);                
            }

            foreach (var lib in cfg.localLibs)
            {
                string basePath = "lib/" + lib.targetFolder.Replace('\\', '/');
                cfgp.includePath.Add(basePath);

                var utiliyPath = basePath + "/utility";
                if (Directory.Exists(utiliyPath)) cfgp.includePath.Add(utiliyPath);

                var srcPath = basePath + "/src";
                if (Directory.Exists(srcPath)) cfgp.includePath.Add(srcPath);
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

            var boardDef = "ARDUINO_" + options.FirstOrDefault(o => o.Key == "build.board").Value;
            props.configurations[0].defines.Add(boardDef);

            addConfigOption(options, props, "F_CPU=", "build.fcpu");
            addConfigOption(options, props, "", "build.usbtype");
            addConfigOption(options, props, "LAYOUT_", "build.keylayout");
            props.configurations[0].defines.Add("ARDUINO=10813");


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
