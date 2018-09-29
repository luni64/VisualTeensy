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


    public class PjrcLibs
    {
        public string name => "PJRC Libraries";
        public List<Library> libraries { get; }
        public List<Library> selected { get; }


        public PjrcLibs(SetupData data)
        {
            var path = data.libBase;
            
            if (!Directory.Exists(path)) return;

            this.data = data;

            libraries = new List<Library>();
            selected = new List<Library>();

            var json = new JavaScriptSerializer();

            foreach (var libDir in Directory.GetDirectories(path))
            {
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
                else
                {
                    p = Path.Combine(libDir, "library.properties");
                    if (File.Exists(p))
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
                }
            }
        }

        public SetupData data { get; }
    }
}


