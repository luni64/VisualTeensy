using System;
using System.Collections.Generic;
using System.Text;

namespace vtCore.Interfaces
{
    public interface IProjectLibrary : ILibrary
    {
     //   static IProjectLibrary  cloneFromLib(ILibrary lib);
        string targetFolderName { get; }
        Uri targetUri { get; set; }
    }
}
