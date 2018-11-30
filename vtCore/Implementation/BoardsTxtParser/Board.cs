using System;
using System.Collections.Generic;
using System.Linq;

namespace vtCore
{
    class Board : IBoard
    {
        #region IBoard
        public String name { get; private set; }
        public string fqbn
        {
            get
            {
                string r = $"teensy:avr:{id}:{_optionSets[0].optionSetID}={_optionSets[0]._selectedOption.id}";
                foreach (var os in _optionSets.Skip(1))
                {
                    if (os._selectedOption != null)
                    {
                        r += $",{os.optionSetID}={os._selectedOption.id}";
                    }
                }
                return r;
            }
        }
        public IEnumerable<IOptionSet> optionSets => _optionSets;
        public string core { get; private set; }
        public string id { get; }

        public Dictionary<String, String> getAllOptions()
        {
            Dictionary<string, string> allOptions = fixedOptions.ToDictionary(o => o.name, o => o.value);

            foreach (var optionSet in _optionSets)
            {
                var options = optionSet?._selectedOption?.paramList;
                if (options != null)
                {
                    foreach (var option in options)
                    {
                        if (allOptions.ContainsKey(option.name))
                        {
                            allOptions[option.name] = option.value;  // overwrite defaults if necessary
                        }
                        else
                        {
                            allOptions.Add(option.name, option.value);
                        }
                    }
                }
            }

            allOptions.Remove("build.flags.ld");
            allOptions.Add("build.flags.ld", "-Wl,--gc-sections,--relax,--defsym=__rtc_localtime=$(shell powershell [int](Get-Date -UFormat +%s)[0])");

            return allOptions;
        }


        #endregion


        public Board(IEnumerable<Menu> menus, IEnumerable<Entry> entries)
        {
            try
            {
                var nameEntry = entries.FirstOrDefault(e => e.key[1] == "name");
                name = nameEntry.value;
                id = nameEntry.key[0];

                _optionSets = menus.Select(m => new OptionSet(m.MenuName, m.OptionSetID)).ToList();

                parse(entries);
            }
            catch
            {
                _optionSets = null;
            }
        }

        private void parse(IEnumerable<Entry> entries)
        {
            foreach (var optionSet in _optionSets)
            {
                var options = entries.Where(e => e.key[1] == "menu" && e.key[2] == optionSet.optionSetID);  // all options from a menu
                if (options != null)
                {
                    foreach (var optionEntries in options.GroupBy(o => o.key[3]))                           // option entries grouped by optionId (e.g. serial, keyboard...)
                    {
                        var option = new Option(optionEntries);
                        if (option.name != null)
                        {
                            optionSet._options.Add(new Option(optionEntries));
                        }
                    }
                }
                optionSet.selectedOption = optionSet._options.FirstOrDefault();
            }

            // parse board-fixed options
            fixedOptions = entries.Where(e => e.key[1] == "build").Select(e => new BuildEntry(e)).ToList();

            // get the core defintion of the board
            core = fixedOptions?.FirstOrDefault(o => o.name == "build.core")?.value ?? "unknown";
        }

        private List<OptionSet> _optionSets = new List<OptionSet>();
        private List<BuildEntry> fixedOptions;


    }
}
