using System;
using System.IO;
using System.Linq;
using vtCore.Interfaces;
//using ViewModel;

namespace vtCore
{
    class libTransfer
    {
        public string name { get; set; }
    }



    public class RepositoryIndexJson : IRepository
    {
        public string name { get; }
        public string repoPath { get; set; }
        public ILookup<string, ILibrary> libraries { get; }
        override public string ToString() => name;
        public RepoType type => RepoType.web;

        public RepositoryIndexJson(string name, string path)
        {
            this.name = name;
            this.repoPath = path;

            libraries = LibraryReader.parseIndexJsonRepository(path);
        }
    }

    public class RepositoryLocal : IRepository
    {
        public string name { get; }
        override public string ToString() => name;
        public string repoPath { get; }
        public RepoType type { get; }
        public ILookup<string, ILibrary> libraries { get; }

        public RepositoryLocal(string name, string repoPath, bool shared = false)
        {
            this.name = name;
            this.repoPath = repoPath;
            this.type = shared ? RepoType.shared : RepoType.local;
            libraries = parseLocalRepository(repoPath);
        }



        static ILookup<string, ILibrary> parseLocalRepository(string repoBase)
        {
            if (!Directory.Exists(repoBase)) return null;

            return Directory.GetDirectories(repoBase).Select(libFolder => parseLibProps(libFolder)).ToLookup(k => k.name) as ILookup<string, ILibrary>;
        }

        static ILibrary parseLibProps(string libFolder)
        {
            var lib = new Library();

            lib.sourceUri = new Uri(libFolder);
            
            var libProps = Path.Combine(libFolder, "library.properties");

            if (File.Exists(libProps))
            {
                char[] keySep = { '=' };
                var props = File.ReadAllLines(libProps)
                    .Select(line => line.Split(keySep, StringSplitOptions.RemoveEmptyEntries))
                    .Where(line => line.Count() == 2)
                    .ToDictionary(keySelector: line => line[0].Trim().ToLower(), elementSelector: line => line[1].Trim());

                lib.name = props.getValueOrDefault("name");
                lib.version = props.getValueOrDefault("version");
                lib.author = props.getValueOrDefault("author");
                lib.maintainer = props.getValueOrDefault("maintainer");
                lib.sentence = props.getValueOrDefault("sentence");
                lib.paragraph = props.getValueOrDefault("paragraph");
                lib.category = props.getValueOrDefault("category");
                lib.website = props.getValueOrDefault("url");
            }
            else
            {
                lib.name = Path.GetFileName(libFolder);
                //  path = Path.GetFileName(libFolder);
                //lib.sourceType = Library.SourceType.localFolder;
                //lib.source = libFolder;
                lib.sentence = "no information";
                lib.version = "?";
            }

            if (String.IsNullOrWhiteSpace(lib.website))
            {
                lib.website = lib.sourceUri.AbsolutePath;
            }

            return lib;
        }

    }
}


