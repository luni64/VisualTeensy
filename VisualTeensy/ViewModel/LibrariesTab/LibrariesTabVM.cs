using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows.Data;
using vtCore;
using vtCore.Interfaces;



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


        public ObservableCollection<Library> projectLibraries { get; }
        public ListCollectionView repos { get; }

        public LibrariesTabVM(IProject project, LibManager libManager)
        {
            this.project = project;

            cmdDel = new RelayCommand(doDel);

            repos = new ListCollectionView(libManager.repositories.Select(r => new RepositoryVM(r)).ToList());
            repos.CurrentChanged += (s, e) => searchString = searchString; // trigger a filter event on new repo

            projectLibraries = new ObservableCollection<Library>(project.selectedConfiguration.localLibs.Union(project.selectedConfiguration.sharedLibs));
            projectLibraries.CollectionChanged += projectLibrariesChanged;
        }


        public string searchString
        {
            get => _searchString;
            set
            {
                SetProperty(ref _searchString, value);
                var searchStrings = _searchString?.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.ToLower()).ToList();
                ((RepositoryVM)repos.CurrentItem).filter(searchStrings);
            }
        }
        string _searchString;

        private void projectLibrariesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    var curRepo = repos.CurrentItem as RepositoryVM;

                    foreach (Library library in e.NewItems)
                    {                       
                        if(curRepo.isShared)
                        {
                            project.selectedConfiguration.sharedLibs.Add(library);
                        }
                        else
                        {
                            string safeName = library.unversionedLibFolder; //hack
                            library.targetUri = new Uri(Path.Combine(project.path, "lib",safeName));
                            project.selectedConfiguration.localLibs.Add(library);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (Library library in e.OldItems)
                    {
                        if (library.isLocalSource)
                        {
                            project.selectedConfiguration.localLibs.Remove(library);
                        }
                        else
                        {
                            project.selectedConfiguration.sharedLibs.Remove(library);
                        }
                    }
                    break;

                default:
                    break;
            }
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
            var col = dropInfo.DragInfo.SourceCollection;

            var sourceItem = dropInfo.Data as LibraryVM;
            var lib = sourceItem.selectedVersion;
            projectLibraries.Add(lib);
        }

        public void update()
        {
            projectLibraries.CollectionChanged -= projectLibrariesChanged;
            projectLibraries.Clear();
            foreach (var lib in project.selectedConfiguration.localLibs.Union(project.selectedConfiguration.sharedLibs))
            {
                projectLibraries.Add(lib);
            }
            projectLibraries.CollectionChanged += projectLibrariesChanged;
        }
        IProject project;
    }
}
