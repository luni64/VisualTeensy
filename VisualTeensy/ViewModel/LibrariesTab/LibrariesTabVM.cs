﻿using GongSolutions.Wpf.DragDrop;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        public LibrariesTabVM(Project project)
        {
            this.project = project;

            cmdDel = new RelayCommand(doDel);

            repositories = project.libManager.repositories.Select(r => new RepositoryVM(r)).ToList();
            selectedRepository = repositories.FirstOrDefault();
            

            projectLibraries = new ObservableCollection<Library>(project.selectedConfiguration.sharedLibs);
            projectLibraries.CollectionChanged += projedtLibrariesChanged;
        }

        private void projedtLibrariesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Library library in e.NewItems)
                    {
                      //  project.libManager.projectLibraries.Add(library);
                        if (library.isLocal)
                        {
                            project.selectedConfiguration.localLibs.Add(library);
                        }
                        else
                        {
                            project.selectedConfiguration.sharedLibs.Add(library);
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (Library library in e.OldItems)
                    {
                       // project.libManager.projectLibraries.Remove(library);
                        if (library.isLocal)
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
            project.generateFiles();
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


        Project project;
    }
}
