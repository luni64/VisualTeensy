using VisualTeensy.Model;

namespace ViewModel
{
    public class LibraryVM : BaseViewModel
    {
        public string name => lib.name;
        public string description => lib.description;

        public bool selected
        {
            get => _isSelected;
            set
            {
                SetProperty(ref _isSelected, value);                                
            }
        }
        bool _isSelected;

        public LibraryVM(Library lib)
        {
            this.lib = lib;
        }

        public Library lib { get; }
    }
}
