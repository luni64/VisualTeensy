using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;

namespace VisualTeensy.Model
{
    class libTransfer
    {
        public string name { get; set; }
    }

    public class Repository
    {
        public string name { get; }
        public string path { get; set; }
        public List<Library> libraries { get; }
        public List<Library> selected { get; }


        public Repository(string name, string path)
        {
            this.name = name;
            this.path = path;

            libraries = new List<Library>();
            selected = new List<Library>();

            //this.project = project; 

            if (!Directory.Exists(path))
            {
                return;
            }

            //this.data = data;

            var json = new JavaScriptSerializer();

            foreach (var libDir in Directory.GetDirectories(path))
            {
             //   var libDirSrc = Path.Combine(libDir, "src");
             
                Library lib;
                string p;

                if (File.Exists(p = Path.Combine(libDir, "library.json")))
                {
                    using (TextReader reader = new StreamReader(p))
                    {
                        lib = json.Deserialize<Library>(reader.ReadToEnd());
                        lib.path = Path.GetFileName(libDir);
                    }
                }
                else if (File.Exists(p = Path.Combine(libDir, "library.properties")))
                {
                    lib = new Library()
                    {
                        path = Path.GetFileName(libDir)
                    };

                    using (TextReader reader = new StreamReader(p))
                    {
                        var lines = reader.ReadToEnd().Split('\n');

                        //string name = null;
                        //string description = null;
                        foreach (var line in lines)
                        {
                            var tok = line.Split('=');
                            if (tok.Length < 2)
                            {
                                break;
                            }

                            if (tok[0] == "name")
                            {
                                lib.name = tok[1].Trim();
                            }
                            else if (tok[0] == "sentence")
                            {
                                lib.description = tok[1].Trim();
                            }
                            if (lib.name != null && lib.description != null)
                            {
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
                        path = libDir,
                        description = "no information"
                    };
                }

                libraries.Add(lib);
            }
        }

        //public ProjectData project { get; }

        //public SetupData data { get; }
    }
}


