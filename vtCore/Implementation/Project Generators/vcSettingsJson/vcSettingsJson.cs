using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using vtCore.Interfaces;

namespace vtCore
{
    public static class vcSettingsJson
    {
        public static bool generate(JObject json)
        {   
            bool dirty = false;
                        
            if (!json.ContainsKey("task.autoDetect"))
            {
                json.Add("task.autoDetect", "off");
                dirty = true;
            }
            // more settings..

            return dirty;
            
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
