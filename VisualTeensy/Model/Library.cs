using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualTeensy.Model
{
    public class Library
    {
        public string name { get; set; }
        public string path { get; set; }
        public string description { get; set; }
        public List<Library> dependencies;

        public override string ToString() { return name; }

        public Library()
        {

        }
    }
}
