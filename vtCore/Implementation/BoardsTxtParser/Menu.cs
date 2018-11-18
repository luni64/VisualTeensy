using System;


namespace vtCore
{
    class Menu : npc
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
