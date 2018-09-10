using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board2Make.Model
{
    public class Entry
    {
        public Entry(string line)
        {
            line = line.Trim();
            int idx = line.IndexOf('=');
            key = line.Substring(0, idx).Split(new char[] { '.' });
            value = line.Substring(idx + 1, line.Length - idx - 1);
        }

        public string[] key { get; private set; }
        public string value { get; private set; }
    }
}
