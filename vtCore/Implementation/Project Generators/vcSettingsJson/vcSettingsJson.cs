using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using vtCore.Interfaces;

namespace vtCore
{
    public static class vcSettingsJson
    {
        private static void addLib(string basePath, string lib, List<string> includes)
        {
            string p = Path.Combine(basePath, lib, "src");
            if (Directory.Exists(p))
            {
            }

            if (Directory.Exists(Path.Combine(basePath, lib, "src")))  // library format 1.5, only src is searched for includes
            {
                includes.Add("-I'" + lib + "/src'");
            }
            else // old library format, base folder and utility folder are searched for includes
            {
                includes.Add("-I'" + lib + "'");

                var utiliyPath = lib + "/utility";
                if (Directory.Exists(Path.Combine(basePath, lib, "utility")))
                    includes.Add("-I'" + lib + "/utility'");
            }
        }

        private static string getCfgOption(Dictionary<string, string> options, string prefix, string key)
        {
            var option = options.FirstOrDefault(o => o.Key == key).Value;
            return option != null ? prefix + option : null;
        }

        public static bool generate(JObject json, IConfiguration cfg, IProject prj)
        {            
            var flags = new List<string>();
                        
            // core files
            flags.Add($"-I'{prj.path}/src'".Replace('\\', '/'));        
            if (cfg.coreStrategy == LibStrategy.link)
            {
                flags.Add($"-I'{cfg.coreBase.path}/cores/{cfg.selectedBoard.core}'".Replace('\\', '/'));
            }
            else
            {
                flags.Add($"-I'{prj.path}/cores/{cfg.selectedBoard.core}'".Replace('\\', '/'));
            }

            // inlcude path shard libraries
            foreach (var lib in cfg.sharedLibs)
            {
                string basePath = lib.sourceUri.LocalPath.Replace('\\', '/');
                addLib("", basePath, flags);
            }

            // include path local libraries
            foreach (var lib in cfg.localLibs)
            {
                string basePath =  $"{prj.path}/lib/{lib.targetFolder}".Replace('\\', '/');
                addLib(prj.path, basePath, flags);
            }
            

            // Compiler switches ----------------------------------------------------------
            var brdOpt = cfg.selectedBoard?.getAllOptions();

            // defines
            foreach (var defEntry in brdOpt.Where(o => o.Key == "build.flags.defs"))
            {
                flags.Add(defEntry.Value.Trim());
            }

            // teensyduino flags
            flags.Add(getCfgOption(brdOpt, "-DF_CPU=", "build.fcpu"));
            flags.Add(getCfgOption(brdOpt, "-D", "build.usbtype"));
            flags.Add(getCfgOption(brdOpt, "-DLAYOUT_", "build.keylayout"));            
            flags.Add("-DARDUINO_" + brdOpt.FirstOrDefault(o => o.Key == "build.board").Value);
            flags.Add("-DARDUINO=10815");

            // compiler flags
            flags.Add(brdOpt.FirstOrDefault(o => o.Key == "build.flags.cpp").Value);            
            flags.Add(brdOpt.FirstOrDefault(o => o.Key == "build.flags.c").Value);           
            flags.Add(brdOpt.FirstOrDefault(o => o.Key == "build.flags.cpu").Value);
            flags.Add(brdOpt.FirstOrDefault(o => o.Key == "build.flags.optimize").Value);
            flags.Add(brdOpt.FirstOrDefault(o => o.Key == "build.flags.common").Value);
            flags.Add(brdOpt.FirstOrDefault(o => o.Key == "build.flags.deps").Value);

            // settings.json
            json["task.autoDetect"] = "off";
            json["compiler-explorer.url"] = "http://localhost:10240";
            json["compiler-explorer.compiler"] = "g54";
            json["compiler-explorer.options"] = JToken.FromObject(flags);
           
            return true;
        }
    }
}



//var props = new PropertiesJson()
//{
//    configurations = new List<ConfigurationJson>()
//    {
//        new ConfigurationJson()
//        {
//            name = "VisualTeensy",
//            compilerPath =  Path.Combine(cfg.compiler,"arm-none-eabi-gcc.exe").Replace('\\','/'),
//            intelliSenseMode = "gcc-arm",
//            cppStandard = "gnu++14", // hack: might be better to extract from boards.txt
//            includePath = new List<string>(),
//            defines = new List<string>()
//        }
//    }
//};
