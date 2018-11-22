using System.Collections.Generic;

namespace vtCore
{
    public interface IProject
    {
        string name { get; }

        IEnumerable<IConfiguration> configurations { get; }
        IConfiguration selectedConfiguration { get; }
        SetupData setup { get; }
       // LibManager libManager { get; }

        void openProject(string path);
        void newProject();

        string path { get; set; }
        string pathError { get; }

        bool isMakefileBuild { get; set; }


        // files ------------------------------------
        // public string makefile { get; set; }
        string tasks_json { get; }
        string props_json { get; }
        string vsSetup_json { get; }

        void generateFiles();

    }
}
