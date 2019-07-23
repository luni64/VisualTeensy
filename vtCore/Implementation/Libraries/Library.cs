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
        public string unversionedLibFolder => versionedLibFolder?.Substring(0, versionedLibFolder.LastIndexOf('-'));
        string versionedLibFolder => Path.GetFileNameWithoutExtension(url);

        public List<Library> dependencies;

        public override string ToString() => $"{name} {version}";

        public enum SourceType { local, net }

        public bool isLocal { get; set; } = false;

        public SourceType sourceType = SourceType.net;

        public void parse(string libFolder)
        {
            var libProps = Path.Combine(libFolder, "library.properties");

            if (File.Exists(libProps))
            {
                using (TextReader reader = new StreamReader(libProps))
                {
                    path = Path.GetFileName(libFolder);
                    source = path;
                    sourceType = Library.SourceType.local;
                    isLocal = true;

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
                                name = parts[1].Trim();
                                break;
                            case "version":
                                version = parts[1].Trim();
                                break;
                            case "author":
                                author = parts[1].Trim();
                                break;
                            case "maintainer":
                                maintainer = parts[1].Trim();
                                break;
                            case "sentence":
                                sentence = parts[1].Trim();
                                break;
                            case "paragraph":
                                paragraph = parts[1].Trim();
                                break;
                            case "category":
                                category = parts[1].Trim();
                                break;
                            case "url":
                                website = parts[1].Trim();
                                break;
                        }
                    }
                }
            }
            else
            {

                name = Path.GetFileName(libFolder);
                path = Path.GetFileName(libFolder);
                sourceType = Library.SourceType.local;
                source = libFolder;
                sentence = "no information";
                version = "?";
            }
        }
    }
}
