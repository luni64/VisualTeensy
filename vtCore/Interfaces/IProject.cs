using System.Collections.Generic;

namespace vtCore
{
    public enum Target
    {
        vsCode,
        sublimeText,
        atom,
        vsFolder
    }

    public enum BuildSystem
    {
        makefile,
        arduino
    }

    public enum DebugSupport
    {
        none,
        cortex_debug
    }

    public interface IProject
    {
        string name { get; }
        string cleanName { get; }

        IEnumerable<IConfiguration> configurations { get; }
        IConfiguration selectedConfiguration { get;  }

        void openProject(string path);
        void newProject();

        string path { get; set; }
        string pathError { get; }

        Target target{ get; set; }
        BuildSystem buildSystem { get; set; }
        DebugSupport debugSupport { get; set; }
    }
}
