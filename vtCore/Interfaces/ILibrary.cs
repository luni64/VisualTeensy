using System;
using System.Collections.Generic;

namespace vtCore.Interfaces
{
    public interface ILibrary
    {
        string name { get; }
        string author { get; }
        string sentence { get;  }
        string paragraph { get;  }        
        string category { get; }
        List<string> architectures { get; }
        string archiveFileName { get; }
        string checksum { get; }
        bool isLocalSource { get; }
        bool isWebSource { get; }
        string maintainer { get; }
        uint size { get; }
        string sourceFolderName { get; }
        Uri sourceUri { get; }
        List<string> types { get;  }
        string version { get;  }
        Version v { get; }
        string website { get; }
        string ToString();
    }
}