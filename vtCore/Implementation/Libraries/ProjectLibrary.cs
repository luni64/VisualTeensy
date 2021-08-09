using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using vtCore.Interfaces;

namespace vtCore
{
    public class ProjectLibrary : Library, IProjectLibrary
    {
        public string targetFolder { get; set; }
       
        public static IProjectLibrary cloneFromLib(ILibrary lib)
        {
            return new ProjectLibrary(lib);
        }
        protected ProjectLibrary(ILibrary source) : base(source) { }
    }
}
