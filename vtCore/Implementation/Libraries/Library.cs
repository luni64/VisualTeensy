using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using vtCore.Interfaces;

namespace vtCore
{
    public class Library : ILibrary
    {
        public string name { get; set; }                // name, can contain spaces
        public string version { get; set; }             // more than one version of the lib can be found in repos
        public Version v // experimental, replace version string by version if this works out
        {
            get
            {
                if(Version.TryParse(version, out Version ver))
                {
                    return ver;
                }
                return new Version();
            }
        }

        public string author { get; set; }              // no format requirements
        public string maintainer { get; set; }          // no format requirements
        public string sentence { get; set; }            // short description
        public string paragraph { get; set; }           // long description
        public string category { get; set; }            // https://github.com/arduino/Arduino/wiki/Arduino-IDE-1.5:-Library-specification#libraryproperties-file-format
        public string archiveFileName { get; set; }     // name of the *.zip if zipped
        public uint size { get; set; }                  // bytes
        public string website { get; set; }             // additional info, not the download site        
        public Uri sourceUri { get; set; }              // source location of the lib          
        public string sourceFolderName => sourceUri?.Segments.Last();
        public List<string> architectures { get; set; }
        public List<string> types { get; set; }
        public string checksum { get; set; }
        public List<ILibrary> dependencies { get; set; }
        public override string ToString() => $"{name} {version}";
        public bool isLocalSource => sourceUri.IsFile;
        public bool isWebSource => sourceUri.Scheme == Uri.UriSchemeHttp || sourceUri.Scheme == Uri.UriSchemeHttps;

        protected Library(ILibrary copyMe)
        {
            Type t = copyMe.GetType();
            foreach (PropertyInfo propInf in t.GetProperties().Where(p => p.SetMethod != null))
            {
                propInf.SetValue(this, propInf.GetValue(copyMe));
            }
        }
        internal Library() { }
    }
}
