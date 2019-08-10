//using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using vtCore.Interfaces;

namespace vtCore
{
    internal class Libs
    {
        public List<IdxLib> libraries { get; set; }
    }

    internal class IdxLib
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
        public string url { get; set; }
        public List<string> architectures { get; set; }
        public List<string> types { get; set; }
        public string checksum { get; set; }
    }

    static class LibraryReader
    {
        public static ILookup<string, ILibrary> parseIndexJsonRepository(string library_indexJson)
        {
            try
            {
                string jsonString = File.ReadAllText(library_indexJson);
                var jsonLibs = JsonConvert.DeserializeObject<Libs>(jsonString).libraries;

                var ret = jsonLibs.Select(t => new Library()
                {
                    name = t.name,
                    version = t.version,
                    author = t.author,
                    maintainer = t.maintainer,
                    sentence = t.sentence,
                    paragraph = t.paragraph,
                    archiveFileName = t.archiveFileName,
                    website = t.website,
                    sourceUri = new Uri(t.url),


                    architectures = t.architectures,
                    types = t.types,


                } as ILibrary).ToLookup(k => k.name); // libraries structured by name (more than on version per name possible)

                return ret;
            }
            catch 
            {
                //log.Warn($"Parsing of {library_indexJson} failed", ex);
                return null;
            }
        }

        //public static ILookup<string, Library> parseLocalRepository(string repoBase)
        //{
        //    return Directory.GetDirectories(repoBase)
        //        .Select(libFolder => parseLibProps(libFolder))
        //        .ToLookup(k => k.name);
        //}

        //static Library parseLibProps(string libFolder)
        //{
        //    var lib = new Library();

        //    lib.sourceUri = new Uri(libFolder);
        //    //lib.path = Path.GetFileName(libFolder);

        //    var libProps = Path.Combine(libFolder, "library.properties");

        //    if (File.Exists(libProps))
        //    {
        //       // lib.source = lib.path;
        //      //  lib.sourceType = Library.SourceType.localFolder;
        //      //  lib.isLocal = true;

        //        char[] keySep = { '=' };
        //        var props = File.ReadAllLines(libProps)
        //            .Select(line => line.Split(keySep, StringSplitOptions.RemoveEmptyEntries))
        //            .Where(line => line.Count() == 2)
        //            .ToDictionary(keySelector: line => line[0].Trim().ToLower(), elementSelector: line => line[1].Trim());

        //        lib.name = props.GetValueOrDefault("name");
        //        lib.version = props.GetValueOrDefault("version");
        //        lib.author = props.GetValueOrDefault("author");
        //        lib.maintainer = props.GetValueOrDefault("maintainer");
        //        lib.sentence = props.GetValueOrDefault("sentence");
        //        lib.paragraph = props.GetValueOrDefault("paragraph");
        //        lib.category = props.GetValueOrDefault("category");
        //        lib.website = props.GetValueOrDefault("url");


        //    }
        //    else
        //    {
        //        lib.name = Path.GetFileName(libFolder);
        //        //  path = Path.GetFileName(libFolder);
        //        //lib.sourceType = Library.SourceType.localFolder;
        //        //lib.source = libFolder;
        //        lib.sentence = "no information";
        //        lib.version = "?";
        //    }

        //    if (String.IsNullOrWhiteSpace(lib.website))
        //    {
        //        lib.website = lib.sourceUri.AbsolutePath;
        //    }

        //    return lib;
        //}

        public static string getValueOrDefault(this IDictionary<string, string> dictionary, string key)
        {            
            return dictionary.TryGetValue(key, out string value) ? value.Trim() : null;
        }

    }
}

