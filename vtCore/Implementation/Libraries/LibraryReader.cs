//using log4net;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
            catch (Exception ex)
            {
                log.Warn($"Parsing of {library_indexJson} failed", ex);

                var dummyRepo = new List<ILibrary>()
                {
                    //new Library()
                    //{
                    //    name = "Please update index"
                    //}
                };

                return dummyRepo.ToLookup(k => k.name);
            }
        }


        public static string getValueOrDefault(this IDictionary<string, string> dictionary, string key)
        {
            return dictionary.TryGetValue(key, out string value) ? value.Trim() : null;
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    }
}

