using VisualTeensy.Model;
using System.Linq;
using System.Collections.Generic;

namespace ViewModel
{
    public class RepositoryVM : BaseViewModel
    {
        public List<LibraryVM> libs { get; }               

        public RepositoryVM(Repository repository)
        {
            libs = repository.libraries.Select(lib => new LibraryVM(lib)).ToList();
            libs.ForEach(libVM => libVM.PropertyChanged += (s, e) => OnPropertyChanged("libraries"));
        }
    }
}
