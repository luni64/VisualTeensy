using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Board2Make.Model
{
    public class Board
    {
        public String name { get; private set; }

        public List<OptionSet> optionSets { get; } = new List<OptionSet>();

        public List<BuildEntry> fixedOptions { get; private set; }

        public string core { get; private set; }

        public bool ParseError { get; private set; }

        override public string ToString() => name;


        public Board(IEnumerable<Menu> menus, IEnumerable<Entry> entries)
        {
            ParseError = false;
            try
            {
                foreach (var menu in menus)
                {
                    optionSets.Add(new OptionSet(menu.MenuName, menu.OptionSetID));
                }
                parse(entries);
            }
            catch { ParseError = true; }
        }

        void parse(IEnumerable<Entry> entries)
        {
            name = entries.FirstOrDefault(e => e.key[1] == "name").value;

            foreach (var optionSet in optionSets)
            {
                var options = entries.Where(e => e.key[1] == "menu" && e.key[2] == optionSet.optionSetID);  // all options from a menu
                if (options != null)
                {
                    foreach (var optionEntries in options.GroupBy(o => o.key[3]))                           // option entries grouped by optionId (e.g. serial, keyboard...)
                    {
                        var option = new Option(optionEntries);
                        if (option.name != null) optionSet.options.Add(new Option(optionEntries));
                    }
                }
                optionSet.selectedOption = optionSet.options.FirstOrDefault();
            }

            // parse board-fixed options
            fixedOptions = entries.Where(e => e.key[1] == "build").Select(e => new BuildEntry(e)).ToList();

            // get the core defintion of the board
            core = fixedOptions?.FirstOrDefault(o => o.name == "build.core")?.value ?? "unknown";
        }


        public Dictionary<String, String> getAllOptions()
        {
            Dictionary<string, string> allOptions = fixedOptions.ToDictionary(o => o.name, o => o.value);

            foreach (var optionSet in optionSets)
            {
                var options = optionSet?.selectedOption?.paramList;
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

            //Hack, would be better to change the build.flags.ld entry instead of replacing
            allOptions.Remove("build.flags.ld");
            allOptions.Add("build.flags.ld", "-Wl,--gc-sections,--relax,--defsym=__rtc_localtime=0");

            return allOptions;
        }
    }
}
