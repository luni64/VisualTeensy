using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using vtCore.Interfaces;

namespace vtCore
{
    public class LibManager
    {
        public List<IRepository> repositories { get; }
        public IRepository sharedRepository => repositories.FirstOrDefault(r => r.type == RepoType.shared);
        public LibManager(SetupData setup)
        {
            this.setup = setup;
            repositories = new List<IRepository>();
            update();
        }

        public async Task updateArduinoIndex()
        {
            await Helpers.downloadFileAsync(setup.libIndexSource, setup.libIndex_json);
            update();
        }

        public void update()
        {
            repositories.Clear();

            IRepository rvm;

            rvm = new RepositoryLocal("Teensyduino Libraries", setup.tdLibBase);
            if (rvm.libraries != null) repositories.Add(rvm);

            rvm = new RepositoryLocal("Installed, shared Libraries", setup.sharedLibrariesFolder, shared: true);
            if (rvm.libraries != null) repositories.Add(rvm);

            rvm = new RepositoryIndexJson("Arduino Repository", setup.libIndex_json);
            if (rvm.libraries != null) repositories.Add(rvm);
        }

        private SetupData setup;
    }
}

