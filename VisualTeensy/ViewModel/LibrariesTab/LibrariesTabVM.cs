using GongSolutions.Wpf.DragDrop;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using VisualTeensy.Model;

namespace ViewModel
{
    class LibrariesTabVM : BaseViewModel, IDropTarget
    {
        public RelayCommand cmdDel { get; }
        void doDel(object o)
        {
            var lib = o as Library;
            projectLibraries.Remove(lib);
        }

        public List<RepositoryVM> repositories { get; }
        public RepositoryVM selectedRepository
        {
            get => _selectedRepository;
            set => SetProperty(ref _selectedRepository, value);
        }
        RepositoryVM _selectedRepository;

        public ObservableCollection<Library> projectLibraries { get; }

        public LibrariesTabVM(Model model)
        {
            this.model = model;

            cmdDel = new RelayCommand(doDel);

            repositories = model.libManager.repositories.Select(r => new RepositoryVM(r)).ToList();
            selectedRepository = repositories.FirstOrDefault();

            projectLibraries = new ObservableCollection<Library>(model.libManager.projectLibraries);
            projectLibraries.CollectionChanged += projedtLibrariesChanged;
        }

        private void projedtLibrariesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Library item in e.NewItems)
                    {
                        model.libManager.projectLibraries.Add(item);                        
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (Library item in e.OldItems)
                    {
                        model.libManager.projectLibraries.Remove(item);
                    }
                    break;

                default:
                    break;
            }
            model.generateFiles();
        }

        public void DragOver(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as LibraryVM;

            if (sourceItem != null && !projectLibraries.Contains(sourceItem.selectedVersion))
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = System.Windows.DragDropEffects.Copy;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as LibraryVM;
            var lib = sourceItem.selectedVersion;
            lib.isLocal = !selectedRepository.name.Contains("Shared");
            projectLibraries.Add(lib);
        }


        Model model;
    }
}
