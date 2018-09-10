using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel;

namespace Board2Make.Model
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
