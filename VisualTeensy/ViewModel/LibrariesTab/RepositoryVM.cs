using System.Collections.ObjectModel;
using System.Linq;
using VisualTeensy.Model;

namespace ViewModel
{
    public class RepositoryVM : BaseViewModel
    {
        public string name { get; private set; }

        public ObservableCollection<LibraryVM> libVMs { get; }
        public LibraryVM selectedLib
        {
            get => _selectedLib;
            set => SetProperty(ref _selectedLib, value);
        }
        LibraryVM _selectedLib;

        public RepositoryVM(IRepository repo)
        {
            if (repo != null)
            {
                libVMs = new ObservableCollection<LibraryVM>(
                    repo
                    .libraries?
                    .OrderBy(lib => lib.Key)
                    .Select(lib => new LibraryVM(lib)));

                selectedLib = libVMs.FirstOrDefault();
                this.name = repo.name;
            }
            else
            {
                name = "used";
                libVMs = new ObservableCollection<LibraryVM>();
            }
        }
    }
}
