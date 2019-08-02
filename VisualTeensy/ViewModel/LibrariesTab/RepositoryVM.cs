using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using vtCore;

namespace ViewModel
{
    public class RepositoryVM : BaseViewModel
    {
        public string name { get; }
        public ListCollectionView libVMs { get; }

        public void filter(List<string> sl)
        {
            searchStrings = sl;
            libVMs.Refresh();
        }

        public string searchString
        {
            get => _searchString;
            set
            {
                SetProperty(ref _searchString, value);
                searchStrings = _searchString?.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.ToLower()).ToList();
                libVMs.Refresh();
            }
        }

        public RepositoryVM(IRepository repo)
        {
            if (repo == null || repo.libraries == null) return;
            
            var libList = repo.libraries.Select(lib => new LibraryVM(lib)).ToList();
            libVMs = new ListCollectionView(libList)
            {
                Filter = (o) => filterLibs(o),
                CustomSort = new AlphabeticSorter(),
            };

            name = repo.name;
        }

        #region private methods and fields ------------------------------

        private string _searchString;

        private List<string> searchStrings;

        private bool filterLibs(object o)
        {
            if (searchStrings == null) return true;

            var libVM = o as LibraryVM;
            var name = libVM.name.ToLower();
            var description = libVM.description.ToLower();

            foreach (var searchString in searchStrings)
            {
                if (!name.Contains(searchString) && !description.Contains(searchString)) return false;
            }
            return true;
        }

        private class AlphabeticSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                var X = x as LibraryVM;
                var Y = y as LibraryVM;
                return X.name.CompareTo(Y.name);
            }
        }

        #endregion
    }
}
