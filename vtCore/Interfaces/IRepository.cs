using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace vtCore.Interfaces
{
    public enum RepoType { shared, local, web };

    public interface IRepository
    {
        string name { get; }
        string repoPath { get; }
        ILookup<string, ILibrary> libraries { get; }

        RepoType type { get; }
    }
}
