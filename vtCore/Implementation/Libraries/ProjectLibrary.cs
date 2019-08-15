using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using vtCore.Interfaces;

namespace vtCore
{
    public class ProjectLibrary : Library, IProjectLibrary
    { 
        public Uri targetUri { get; set; }
        public string targetFolderName => targetUri?.Segments.Last();

        public static IProjectLibrary cloneFromLib(ILibrary lib)
        {
            return new ProjectLibrary(lib);
        }
        protected ProjectLibrary(ILibrary source) : base(source) { }
    }
}
