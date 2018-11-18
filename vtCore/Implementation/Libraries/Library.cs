using System.Collections.Generic;
using System.IO;

namespace vtCore
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
        public string source { get; set; }
        public string unversionedLibFolder => versionedLibFolder.Substring(0, versionedLibFolder.LastIndexOf('-'));
        string versionedLibFolder =>  Path.GetFileNameWithoutExtension(url);
        
        public List<Library> dependencies;

        public override string ToString() => $"{name} {version}";
        
        public enum SourceType {local, net }
        
        public bool isLocal { get; set; } = false;

        public SourceType sourceType = SourceType.net;
    }   
}
