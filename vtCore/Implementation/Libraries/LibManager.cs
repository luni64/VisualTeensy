using System.Collections.Generic;
using System.IO;
using System.Linq;
using vtCore.Interfaces;

namespace vtCore
{
    public class LibManager
    {
        public List<IRepository> repositories { get; }
        public IRepository sharedRepository => repositories.FirstOrDefault(r => r.type == RepoType.shared);               
        public LibManager(SetupData setup)
        {   
            repositories = new List<IRepository>();
            repositories.Add(new RepositoryIndexJson("Arduino Repository", Path.Combine(Helpers.arduinoAppPath, "library_index.json")));
            repositories.Add(new RepositoryLocal("Installed Teensyduino Libraries", setup.tdLibBase));            
            repositories.Add(new RepositoryLocal("Shared Libraries", Path.Combine(Helpers.getSketchbookFolder(), "libraries"), shared: true));
        }
    }
}

 