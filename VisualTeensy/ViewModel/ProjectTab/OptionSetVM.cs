using vtCore;
using System;
using System.Collections.Generic;


namespace ViewModel
{
    public class OptionSetVM : BaseViewModel
    {
        public String name => optionSet.name;
        public IEnumerable<IOption> options => optionSet.options;

        public IOption selectedOption
        {
            get => optionSet.selectedOption;
            set
            {
                if (optionSet.selectedOption != value)
                {
                    optionSet.selectedOption = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public OptionSetVM(IOptionSet optionSet)
        {
            this.optionSet = optionSet;
        }

        private IOptionSet optionSet;
    }
}
