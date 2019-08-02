//using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;


namespace vtCore
{
    internal class libs
    {
        public List<Library> libraries { get; set; }
    }

    static class LibraryReader
    {
        public static Library search(string name)
        {
            return null;
        }

        public static ILookup<string, Library> parseProjectLibs(string projectPath)
        {
            return parseLibraryLocal(projectPath);
        }


        public static ILookup<string, Library> parseLibrary_Index_Json(string library_indexJson)
        {
            //log.Debug(library_indexJson);
            try
            {
                string jsonString = File.ReadAllText(library_indexJson);
                var transfered = JsonConvert.DeserializeObject<libs>(jsonString);

                return transfered.libraries.ToLookup(k => k.name); // libraries structured by name (more than on version per name possible)
            }
            catch //(Exception ex)
            {
                //log.Warn($"Parsing of {library_indexJson} failed", ex);
                return null;
            }
        }

        public static ILookup<string, Library> parseLibraryLocal(string repository)
        {
            //log.Debug(repository);

            List<Library> libraries = new List<Library>();
            try
            {
                foreach (var libDir in Directory.GetDirectories(repository))
                {
                    Library lib = new Library();
                    //string p;

                    lib.parse(libDir);


                    if (String.IsNullOrWhiteSpace(lib.website))
                    {
                        lib.website = new Uri(lib.source).LocalPath;
                    }
                    libraries.Add(lib);
                }
            }
            catch //(Exception ex)
            {
                //log.Warn($"Parsing of {repository} failed", ex);
                return null;
            }
            return libraries.ToLookup(k => k.name); // libraries structured by name (more than on version per name possible)
        }

  

    }
}

