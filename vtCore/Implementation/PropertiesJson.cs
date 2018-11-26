using System.Collections.Generic;

namespace vtCore
{
    class ConfigurationJson
    {
        public string name { get; set; }
        public List<string> includePath { get; set; } = new List<string>();
        public List<string> forcedInclude { get; set; } = new List<string>();
        public List<string> defines { get; set; } = new List<string>();
        public string compilerPath { get; set; }
        public string intelliSenseMode { get; set; }
    }

    class PropertiesJson
    {
        public List<ConfigurationJson> configurations = new List<ConfigurationJson>();
        public int version => 4;
    }
}
