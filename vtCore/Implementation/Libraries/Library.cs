using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace vtCore
{
    public class Library
    {
        public string name { get; set; }                // name, can contain spaces
        public string version { get; set; }             // more than one version of the lib can be found in repos
        public string author { get; set; }              // no format requirements
        public string maintainer { get; set; }          // no format requirements
        public string sentence { get; set; }            // short description
        public string paragraph { get; set; }           // long description
        public string category { get; set; }            // https://github.com/arduino/Arduino/wiki/Arduino-IDE-1.5:-Library-specification#libraryproperties-file-format
        public string archiveFileName { get; set; }     // name of the *.zip if zipped
        public uint size { get; set; }                  // bytes
        public string website { get; set; }             // additional info, not the download site        
        public Uri sourceUri { get; set; }              // source location of the lib    
        public Uri targetUri { get; set; }              // target location of the lib
        public string targetFolderName => targetUri?.Segments.Last();
        public string sourceFolderName => sourceUri?.Segments.Last();
        public List<string> architectures { get; set; } 
        public List<string> types { get; set; }
        public string checksum { get; set; }

        public string path { get; set; }
       // public string source { get; set; }
        public string unversionedLibFolder => versionedLibFolder?.Substring(0, versionedLibFolder.LastIndexOf('-')).Replace(" ", "_");
        
        string versionedLibFolder => Path.GetFileNameWithoutExtension(sourceUri.AbsolutePath);

        public List<Library> dependencies;

        public override string ToString() => $"{name} {version}";

        public enum SourceType { localFolder, website, invalid }

        public bool isLocalSource => sourceUri.IsFile;
        public bool isWebSource => sourceUri.Scheme == Uri.UriSchemeHttp || sourceUri.Scheme == Uri.UriSchemeHttps;

      //  public SourceType sourceType => sourceUri.IsFile ? SourceType.localFolder : () ? SourceType.website : SourceType.invalid ;
       

        //public void parse(string libFolder)
        //{
        //    var libProps = Path.Combine(libFolder, "library.properties");

        //    if (File.Exists(libProps))
        //    {
        //        using (TextReader reader = new StreamReader(libProps))
        //        {
        //            path = Path.GetFileName(libFolder);
        //            source = path;
        //            sourceType = Library.SourceType.local;
        //            isLocal = true;

        //            foreach (var line in reader.ReadToEnd().Split('\n'))
        //            {
        //                string[] parts = line.Split('=');
        //                if (parts.Length < 2)
        //                {
        //                    break;
        //                }

        //                switch (parts[0])
        //                {
        //                    case "name":
        //                        name = parts[1].Trim();
        //                        break;
        //                    case "version":
        //                        version = parts[1].Trim();
        //                        break;
        //                    case "author":
        //                        author = parts[1].Trim();
        //                        break;
        //                    case "maintainer":
        //                        maintainer = parts[1].Trim();
        //                        break;
        //                    case "sentence":
        //                        sentence = parts[1].Trim();
        //                        break;
        //                    case "paragraph":
        //                        paragraph = parts[1].Trim();
        //                        break;
        //                    case "category":
        //                        category = parts[1].Trim();
        //                        break;
        //                    case "url":
        //                        website = parts[1].Trim();
        //                        break;
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {

        //        name = Path.GetFileName(libFolder);
        //        path = Path.GetFileName(libFolder);
        //        sourceType = Library.SourceType.local;
        //        source = libFolder;
        //        sentence = "no information";
        //        version = "?";
        //    }
        //}
    }
}
