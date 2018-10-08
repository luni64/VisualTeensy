using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VisualTeensy.Model;

namespace VisualTeensy.Model
{
    public class LibManager
    {
        public List<IRepository> repositories { get; }

        public string sharedRepositoryPath { get; }
        public List<Library> projectLibraries { get; }

        public void open(List<vtTransferData.vtRepo> repos)
        {
            var sharedLibNames = (repos.FirstOrDefault(r => r.name == "shared")?.libraries) ?? Enumerable.Empty<string>();
            var sharedLibs = repositories.FirstOrDefault(r => r.name.StartsWith("Shared"))?.libraries;

            if (sharedLibs == null) { return; }

            foreach (string libName in sharedLibNames)
            {
                var versions = sharedLibs.FirstOrDefault(l => l.Key == libName);


                
            }




        }

        public LibManager(ProjectData project, SetupData setup)
        {
            projectLibraries = new List<Library>();
            sharedRepositoryPath = Path.Combine(Helpers.getSketchbookFolder(), "libraries");

            repositories = new List<IRepository>();
            repositories.Add(new RepositoryIndexJson("Arduino Repository", Path.Combine(Helpers.arduinoPrefsPath, "library_index.json")));
            repositories.Add(new RepositoryLocal("Installed Teensyduino Libraries", setup.libBase));
            repositories.Add(new RepositoryLocal("Shared Libraries", sharedRepositoryPath));
        }
    }
}

 