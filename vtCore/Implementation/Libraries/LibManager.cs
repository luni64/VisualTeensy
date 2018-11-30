using System.Collections.Generic;
using System.IO;

namespace vtCore
{
    public class LibManager
    {
        public List<IRepository> repositories { get; }

        public IRepository sharedRepository { get; }
        public string sharedRepositoryPath { get; }

        public IRepository localRepository { get; set; }
        

        public LibManager(SetupData setup)
        {         
            sharedRepositoryPath = Path.Combine(Helpers.getSketchbookFolder(), "libraries");
            sharedRepository = new RepositoryLocal("Shared Libraries", sharedRepositoryPath);

            
            
            repositories = new List<IRepository>();
            repositories.Add(new RepositoryIndexJson("Arduino Repository", Path.Combine(Helpers.arduinoAppPath, "library_index.json")));
            repositories.Add(new RepositoryLocal("Installed Teensyduino Libraries", setup.libBase));
            
            repositories.Add(sharedRepository);
        }
    }
}

 