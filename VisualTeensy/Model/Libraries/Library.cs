using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualTeensy.Model
{
    public class Library
    {
        public string name { get; set; }
        public string version { get; set; }
        public string author { get; set; }
        public string maintainer { get; set; }
        public string sentence { get; set; }
        public string paragraph { get; set; }
        public string category { get; set; }
        public string archiveFileName { get; set; }
        public uint size { get; set; }
        public string website { get; set; }
        public string repository { get; set; }
        public string url { get; set; }
        public List<string> architectures { get; set; }
        public List<string> types { get; set; }
        public string checksum { get; set; }

        public string path { get; set; }
        public List<Library> dependencies;

        public override string ToString() => $"{(isLocal ? "+" : "-")}{name} {version}";

        
        public bool isLocal { get; set; } = false;             
    }
   
}
