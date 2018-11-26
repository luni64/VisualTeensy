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

    public interface IProject
    {
        string name { get; }

        IEnumerable<IConfiguration> configurations { get; }
        IConfiguration selectedConfiguration { get;  }

        void openProject(string path);
        void newProject();

        string path { get; set; }
        string pathError { get; }

        Target target{ get; set; }
        BuildSystem buildSystem { get; set; }
    }
}
