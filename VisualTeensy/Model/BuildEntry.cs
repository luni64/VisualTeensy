using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board2Make.Model
{
    public class BuildEntry
    {
        public BuildEntry(Entry e)
        {
            var startIndex = Array.IndexOf(e.key, "build") + 1;
            name = "build";
            for (int i = startIndex; i < e.key.Length; i++)
            {
                name += "." + e.key[i];
            }
            value = e.value;
        }
        public String name { get; private set; }
        public String value { get; private set; }
        override public string ToString() => name;
    }
}
