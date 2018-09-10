using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board2Make.Model
{
    public class Option
    {
        public string name { get; set; } = null;
        public List<BuildEntry> paramList { get; }
        override public string ToString() => name;

        public Option(IEnumerable<Entry> optionEntries)
        {
            var titleEntry = optionEntries.FirstOrDefault(m => m.key.Length == 4);
            if (titleEntry != null)
            {
                name = titleEntry != null ? titleEntry.value : optionEntries.FirstOrDefault().key[2];

                var paramEntries = optionEntries.Where(m => m.key.Length >= 6 && m.key[4] == "build");
                paramList = paramEntries.Select(e => new BuildEntry(e)).ToList();
            }
        }
    }
}
