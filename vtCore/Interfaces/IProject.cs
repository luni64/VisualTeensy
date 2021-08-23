using System.Collections.Generic;
using System.Threading.Tasks;

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

    public enum LibStrategy
    {
        copy, link, clone
    }

    public enum GitError
    {
        OK, 
        Unexpected,
    }
   
    public interface IFolders
    {
        CheckedPath compiler { get; }
        //CheckedPath coreBase { get; }
        //CheckedPath sharedLibs { get; }
    }
            

    public interface IProject
    {
        string name { get; }
        string cleanName { get; }
        bool isNew { get; set; }
        void openProject(string path);
        void newProject();
        Task<GitError> gitInitAsync();


        IEnumerable<IConfiguration> configurations { get; }
        IConfiguration selectedConfiguration { get; }


        string path { get; set; }
        string pathError { get; }

        string libBase { get; }
        string mainSketchPath { get; }

        Target target { get; set; }
        BuildSystem buildSystem { get; set; }
        DebugSupport debugSupport { get; set; }      
    }
}
