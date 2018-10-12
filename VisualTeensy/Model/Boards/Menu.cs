using System;
using ViewModel;

namespace VisualTeensy.Model
{
    public class Menu : BaseViewModel
    {
        public String OptionSetID { get; private set; }
        public String MenuName { get; private set; }
        public Option selectedOption
        {
            get { return _selectedOption; }
            set { SetProperty(ref _selectedOption, value); }
        }
        Option _selectedOption;

        public Menu(String OptionSetID, string MenuName)
        {
            this.OptionSetID = OptionSetID;
            this.MenuName = MenuName;
        }
    }
}
