using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Script.Serialization;

namespace VisualTeensy.Model
{
    class libTransfer
    {
        public string name { get; set; }
    }

    public class Repo
    {
        public string name { get; }
        public List<Library> libraries { get; }
        public List<Library> selected { get; }


        public Repo(string repoName, string repoBase)
        {
            this.name = repoName;
            libraries = new List<Library>();
            selected = new List<Library>();
            
            //this.project = project; 

            if (!Directory.Exists(repoBase)) return;

            //this.data = data;

            var json = new JavaScriptSerializer();

            foreach (var libDir in Directory.GetDirectories(repoBase))
            {
                var libDirSrc = Path.Combine(libDir, "src");
                //if (Directory.Exists(libDirSrc))
                //{

                //}
                //else
                //{

                //}


                string p = Path.Combine(libDir, "library.json");

                if (File.Exists(p))
                {
                    using (TextReader reader = new StreamReader(p))
                    {
                        var lib = json.Deserialize<Library>(reader.ReadToEnd());
                        lib.path = Path.GetFileName(libDir);
                        libraries.Add(lib);
                    }
                }
                else if (File.Exists( p = Path.Combine(libDir, "library.properties")))
                {
                    var lib = new Library()
                    {
                        path = Path.GetFileName(libDir)
                    };

                    using (TextReader reader = new StreamReader(p))
                    {
                        var lines = reader.ReadToEnd().Split('\n');

                        string name = null;
                        string description = null;

                        foreach (var line in lines)
                        {
                            var tok = line.Split('=');
                            if (tok[0] == "name")
                            {
                                lib.name = tok[1].Trim();
                            }
                            else if (tok[0] == "sentence")
                            {
                                lib.description = tok[1].Trim();
                            }
                            if (name != null && description != null) break;
                        }
                        libraries.Add(lib);
                    }

                }
                else
                {
                    var lib = new Library()
                    {
                        name = Path.GetFileName(libDir),
                        path = libDir,
                        description = "no information"
                    };                    
                    libraries.Add(lib);
                }
            }
        }

        //public ProjectData project { get; }

        //public SetupData data { get; }
    }
}


