using System.Collections.Generic;
using vtCore.Interfaces;




namespace vtCore.Interfaces
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

    public interface IFolders
    {
        CheckedPath compiler { get;  }
        //CheckedPath coreBase { get; }
        //CheckedPath sharedLibs { get; }
    }


    public interface IProject
    {     
        string name { get; }
        string cleanName { get; }

        IEnumerable<IConfiguration> configurations { get; }
        IConfiguration selectedConfiguration { get; }

        void openProject(string path);
        void newProject();

        string path { get; set; }
        string pathError { get; }

        string libBase { get; }
        string mainSketchPath { get; }

        Target target { get; set; }
        BuildSystem buildSystem { get; set; }
        DebugSupport debugSupport { get; set; }
    }
}
