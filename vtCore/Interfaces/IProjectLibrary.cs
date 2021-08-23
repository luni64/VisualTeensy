using System;
using System.Collections.Generic;
using System.Text;

namespace vtCore.Interfaces
{
    public interface IProjectLibrary : ILibrary
    {   
        string targetFolder { get; set; }
    }
}
