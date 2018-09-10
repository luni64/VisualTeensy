using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board2Make.Model
{
    class Configuration
    {
        public string name { get; set; }
        public List<string> includePath { get; set; } = new List<string>();
        public List<string> defines { get; set; } = new List<string>();
        public string compilerPath { get; set; }
        public string intelliSenseMode { get; set; }
    }

    class PropertiesJson
    {
        public List<Configuration> configurations = new List<Configuration>();
        public int version => 4;
    }
}
