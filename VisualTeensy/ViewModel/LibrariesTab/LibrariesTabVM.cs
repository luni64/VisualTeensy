using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Data;
using vtCore;
using vtCore.Interfaces;


namespace ViewModel
{
    public  class LibrariesTabVM : BaseViewModel, IDropTarget
    {
        #region commands --------------------------------------------
        public RelayCommand cmdDel { get; }
        void doDel(object o)
        {
            var lib = o as IProjectLibrary;
            projectLibraries.Remove(lib);
        }

        public AsyncCommand cmdUpdateIndex { get; }
        async Task doUpdateIndex()
        {
            isUpdating = true;

            await libManager.updateArduinoIndex();
            repos.CurrentChanged -= repoChanged;
            repos = new ListCollectionView(libManager.repositories.Select(r => new RepositoryVM(r)).ToList());
            repos.CurrentChanged += repoChanged;
            repos.MoveCurrentToLast();
            OnPropertyChanged("repos");

            isUpdating = false;
        }
        #endregion

        public bool isUpdating
        {
            get => _isUpdating;
            set => SetProperty(ref _isUpdating, value);
        }
        private bool _isUpdating = false;

        public bool isArduinoIndexRepo
        {
            get => _isArduinoIndexRepo;
            set => SetProperty(ref _isArduinoIndexRepo, value);
        }
        private bool _isArduinoIndexRepo;


        public ObservableCollection<IProjectLibrary> projectLibraries { get; }
        public ListCollectionView repos { get; private set; }


        public LibrariesTabVM(IProject project, LibManager libManager, SetupData setup)
        {
            this.project = project;
            this.setup = setup;
            this.libManager = libManager;

            cmdDel = new RelayCommand(doDel);
            cmdUpdateIndex = new AsyncCommand(doUpdateIndex);

            repos = new ListCollectionView(libManager.repositories.Select(r => new RepositoryVM(r)).ToList());
            repos.CurrentChanged += repoChanged;
                     
            projectLibraries = new ObservableCollection<IProjectLibrary>(project.selectedConfiguration.localLibs.Union(project.selectedConfiguration.sharedLibs));
            projectLibraries.CollectionChanged += projectLibrariesChanged;
        }

        public void repoChanged(object s, EventArgs e)
        {
            searchString = searchString; // trigger a filter event on new repo

            var repo = ((RepositoryVM)repos.CurrentItem);
            isArduinoIndexRepo = repo.name == "Arduino Repository";
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

                    foreach (IProjectLibrary library in e.NewItems)
                    {
                        if (curRepo.isShared)
                        {
                            project.selectedConfiguration.sharedLibs.Add(library);
                        }
                        else
                        {
                            if (library.isLocalSource)
                            {
                                string  source = library.sourceUri.LocalPath;                              
                                library.targetFolder = Path.GetFileName(source);

                                // library.targetUri = Path.Combine("lib", Path.GetFileName(curRepo.repoPath));
                            }
                            else if (library.isWebSource)
                            {
                                string source = library.sourceUri.LocalPath;
                                library.targetFolder = Path.GetFileNameWithoutExtension(source);
                                
                                //library.targetUri = new Uri(Path.Combine(project.libBase, library.name.Replace(' ', '_')));
                                //library.targetUri = new Uri(Path.Combine("lib", library.name.Replace(' ', '_')),UriKind.Relative);
                            }
                            project.selectedConfiguration.localLibs.Add(library);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (IProjectLibrary library in e.OldItems)
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
            projectLibraries.Add(ProjectLibrary.cloneFromLib(lib));
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
        SetupData setup;
        LibManager libManager;
    }
}
