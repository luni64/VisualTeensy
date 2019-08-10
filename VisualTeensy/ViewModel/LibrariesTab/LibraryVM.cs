using System.Collections.Generic;
using vtCore;
using System.Linq;
using vtCore.Interfaces;

namespace ViewModel
{
    public class LibraryVM : BaseViewModel
    {
        public string name => selectedVersion?.name;
        public string description => selectedVersion?.sentence;
        public IEnumerable<ILibrary> versions { get; }
        //public string parentRepository { get; }

        public ILibrary selectedVersion { get; set; }
                       

        public LibraryVM(IEnumerable<ILibrary> lib/*, string repositoryName*/)
        {
            //parentRepository = string.IsNullOrWhiteSpace(repositoryName) ? "?" : repositoryName;
            this.versions = lib.OrderBy(v => v.version).ToList();
            selectedVersion = this.versions.LastOrDefault();
        }

        public override string ToString() => name;
    }
}
