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
            update(setup);
        }

        public void update(SetupData setup /*IProject project */)
        {
            repositories.Clear();

            IRepository rvm;

            rvm = new RepositoryLocal("Teensyduino Libraries", setup.tdLibBase);
            if (rvm.libraries != null) repositories.Add(rvm);

            rvm = new RepositoryLocal("Installed, shared Libraries", Path.Combine(Helpers.getSketchbookFolder(), "libraries"), shared: true);
            if (rvm.libraries != null) repositories.Add(rvm);

            rvm = new RepositoryIndexJson("Arduino Repository", Path.Combine(Helpers.arduinoAppPath, "library_index.json"));
            if (rvm.libraries != null) repositories.Add(rvm);
        }
    }
}

 