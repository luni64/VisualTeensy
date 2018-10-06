using VisualTeensy.Model;

namespace ViewModel
{
    public class LibraryVM : BaseViewModel
    {
        public string name => lib.name;
        public string description => lib.description;

        public bool selected
        {
            get => lib.isSelected;
            set
            {
                if(value != lib.isSelected)
                {
                    lib.isSelected = value;
                    OnPropertyChanged();
                }                
            }
        }        

        public LibraryVM(Library lib)
        {
            this.lib = lib;
        }
        
        public override string ToString() => name;

        private Library lib { get; }
    }
}
