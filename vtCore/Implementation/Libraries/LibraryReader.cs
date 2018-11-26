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
                    string p;

                    if (File.Exists(p = Path.Combine(libDir, "library.properties")))
                    {
                        using (TextReader reader = new StreamReader(p))
                        {
                            lib.path = Path.GetFileName(libDir);
                            lib.source = libDir;
                            lib.sourceType = Library.SourceType.local;
                            lib.isLocal = true;

                            foreach (var line in reader.ReadToEnd().Split('\n'))
                            {
                                string[] parts = line.Split('=');
                                if (parts.Length < 2)
                                {
                                    break;
                                }

                                switch (parts[0])
                                {
                                    case "name":
                                        lib.name = parts[1].Trim();
                                        break;
                                    case "version":
                                        lib.version = parts[1].Trim();
                                        break;
                                    case "author":
                                        lib.author = parts[1].Trim();
                                        break;
                                    case "maintainer":
                                        lib.maintainer = parts[1].Trim();
                                        break;
                                    case "sentence":
                                        lib.sentence = parts[1].Trim();
                                        break;
                                    case "paragraph":
                                        lib.paragraph = parts[1].Trim();
                                        break;
                                    case "category":
                                        lib.category = parts[1].Trim();
                                        break;
                                    case "url":
                                        lib.website = parts[1].Trim();
                                        break;
                                }
                            }
                        }
                    }
                    else
                    {
                        lib = new Library()
                        {
                            name = Path.GetFileName(libDir),
                            path = Path.GetFileName(libDir),
                            sourceType = Library.SourceType.local,
                            source = libDir,
                            sentence = "no information",
                            version = "?"
                        };
                    }

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

       // private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}

