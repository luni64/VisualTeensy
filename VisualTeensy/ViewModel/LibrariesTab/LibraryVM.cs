using System.Collections.Generic;
using VisualTeensy.Model;
using System.Linq;

namespace ViewModel
{
    public class LibraryVM : BaseViewModel
    {
        public string name => selectedVersion?.name;
        public string description => selectedVersion?.sentence;
        public IEnumerable<Library> versions { get; }

        public Library selectedVersion { get; set; }
                       

        public LibraryVM(IEnumerable<Library> lib)
        {
            this.versions = lib.OrderBy(v => v.version).ToList();
            selectedVersion = this.versions.LastOrDefault();
        }

        public override string ToString() => name;
    }
}
