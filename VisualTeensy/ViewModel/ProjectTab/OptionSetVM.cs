using VisualTeensy.Model;
using System;
using System.Collections.Generic;

namespace ViewModel
{
    public class OptionSetVM : BaseViewModel
    {
        public String name => optionSet.name;
        public List<Option> options => optionSet.options;

        public Option selectedOption
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
        
        public OptionSetVM(OptionSet optionSet)
        {
            this.optionSet = optionSet;
        }

        private OptionSet optionSet;
    }
}
