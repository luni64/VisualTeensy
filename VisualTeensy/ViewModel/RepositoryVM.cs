using System.Collections.ObjectModel;
using VisualTeensy.Model;
using System.Linq;

namespace ViewModel
{
    public class RepositoryVM : BaseViewModel
    {
        public ObservableCollection<LibraryVM> libs { get; }

        //public void update()
        //{
        //    repository.data.libraries.Clear();            
        //    repository.data.libraries.AddRange((libs.Where(l => l.selected).Select(l => l.lib)));           
        //}

        public RepositoryVM(PjrcLibs repository)
        {
            this.repository = repository;

            libs = new ObservableCollection<LibraryVM>();
            foreach (var lib in repository.libraries)
            {
                var lvm = new LibraryVM(lib);
                if(repository.project.libraries.Contains(lib))
                {
                    lvm.selected = true;
                }


                lvm.PropertyChanged += Lvm_PropertyChanged;

                libs.Add(lvm);
            }            
        }

        private void Lvm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var lib = sender as LibraryVM;
            if(lib != null)
            {
                if(lib.selected)
                {
                    repository.project.libraries.Add(lib.lib);
                }
                else
                {
                    repository.project.libraries.Remove(lib.lib);
                }
            }
            OnPropertyChanged("libraries");
        }

        PjrcLibs repository;       
    }
}
