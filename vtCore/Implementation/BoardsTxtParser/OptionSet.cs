using System.Collections.Generic;
using System.Linq;
using vtCore.Interfaces;

namespace vtCore
{
    class OptionSet : IOptionSet
    {
        #region IOptionSet
        public IEnumerable<IOption> options => _options;
        public IOption selectedOption
        {
            get => _selectedOption;
            set => _selectedOption = _options.FirstOrDefault(o => o.name == value?.name);
        }
        public string name { get; }
        #endregion
        
        internal List<Option> _options { get; } = new List<Option>();
        internal Option _selectedOption { get; set; }
        internal string optionSetID { get; }
        
        internal OptionSet(string name, string optionSetID)
        {
            this.name = name;
            this.optionSetID = optionSetID;
        }
    }
}