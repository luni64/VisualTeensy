using System.Collections.Generic;
using System.Linq;

namespace vtCore
{
    class Option : IOption
    {
        public string name { get; set; } = null;

        internal List<BuildEntry> paramList { get; }       
        internal Option(IEnumerable<Entry> optionEntries)
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
