using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace vtCore
{
    class PrepareFolders : ITask
    {
        public string title => "Project Folders";
        public string description => ".vsteensy and .vscode";
        public string status => done ? "OK" : "Generate";

        bool done;

        public PrepareFolders(IProject project)
        {
            vsCodeFolder = new DirectoryInfo(Path.Combine(project.path, ".vscode"));
            vsTeensyFolder = new DirectoryInfo(Path.Combine(project.path, ".vsteensy"));
            srcFolder = new DirectoryInfo(Path.Combine(project.path, "src"));

            done = vsCodeFolder.Exists && vsTeensyFolder.Exists && srcFolder.Exists;
        }

        public Action action => () =>
        {
            if (!done)
            {
                vsCodeFolder.Create();
                vsTeensyFolder.Create();
                srcFolder.Create();
            }
            done = true;
        };


        DirectoryInfo vsCodeFolder, vsTeensyFolder, srcFolder;
    }

    class CleanBinaries : ITask
    {
        public string title => $"Clean Binaries";
        public string description => buildFolder.FullName;
        public string status => buildFolder.Exists && !done ? "Delete files" : "OK";

        public CleanBinaries(IProject project)
        {
            buildFolder = new DirectoryInfo(Path.Combine(project.path, ".vsteensy", "build"));
        }

        public Action action => () =>
        {
            if (buildFolder.Exists)
            {
                buildFolder.Delete(true);
            }
            buildFolder.Create();
            done = true;
        };

        DirectoryInfo buildFolder;
        bool done = false;
    }

    class GenerateIntellisense : ITask
    {
        public string title => $"Intellisense configuration";
        public string description => c_cpp_propertiesFile.FullName;
        public string status => done ? "OK" : exists ? "Overwrite" : "Generate";

        public GenerateIntellisense(IProject project, LibManager libManager, SetupData setup)
        {
            this.project = project;
            this.libManager = libManager;
            this.setup = setup;

            c_cpp_propertiesFile = new FileInfo(Path.Combine(project.path, ".vscode", "c_cpp_properties.json"));
            exists = c_cpp_propertiesFile.Exists;
            done = false;
        }

        public Action action => () =>
        {
            string properties = IntellisenseFile.generate(project, libManager, setup);
            File.WriteAllText(c_cpp_propertiesFile.FullName, properties);
            done = true;
        };

        bool exists, done;
        IProject project;
        LibManager libManager;
        SetupData setup;

        FileInfo c_cpp_propertiesFile;
    }

    class GenerateSettings : ITask
    {
        public string title => $"VisualTeensy settings";
        public string description => settingsFile.FullName;
        public string status => done ? "OK" : exists ? "Overwrite" : "Generate";

        bool exists, done;

        public GenerateSettings(IProject project)
        {
            this.project = project;


            settingsFile = new FileInfo(Path.Combine(project.path, ".vsteensy", "vsteensy.json"));

            exists = settingsFile.Exists;
            done = false;
        }

        public Action action => () =>
        {
            string properties = ProjectSettings.generate(project);
            File.WriteAllText(settingsFile.FullName, properties);
            done = true;
        };

        IProject project;
        FileInfo settingsFile;
    }

    class GenerateMakefile : ITask
    {
        public string title => $"Makefile";
        public string description => file.FullName;

        enum State
        {
            none, existsSame, existsDifferent, done
        };

        public string status { get; private set; }
               
        public GenerateMakefile(IProject project, LibManager libManager, SetupData setup)
        {
            string oldMakefile = "";

            file = new FileInfo(Path.Combine(project.path, "makefile"));
            if (file.Exists) oldMakefile = File.ReadAllText(file.FullName);
            newMakefile = Makefile.generate(project, libManager, setup);

            if (!file.Exists)
                status = "Generate";
            else if (String.Equals(oldMakefile.Substring(400), newMakefile.Substring(400)))
                status = "Up-To-Date";
            else
                status = "Overwrite";
        }

        public Action action => () =>
        {
            if(status != "Up-To-Date")
                File.WriteAllText(file.FullName, newMakefile);
            status = "OK";
        };

        string newMakefile;

        FileInfo file;
    }

    class GenerateTasks : ITask
    {
        public string title => $"Tasks File";
        public string description => file.FullName;
        public string status => done ? "OK" : file.Exists ? "Overwrite" : "Generate";

        bool done;

        public GenerateTasks(IProject project, LibManager libManager, SetupData setup)
        {
            file = new FileInfo(Path.Combine(project.path, ".vscode", "tasks.json"));
            tasksJson = TaskFile.generate(project, libManager, setup);

            done = false;
        }

        public Action action => () =>
        {
            File.WriteAllText(file.FullName, tasksJson);
            done = true;
        };

        string tasksJson;

        FileInfo file;
    }

    class GenerateMainCpp : ITask
    {
        public string title => $"Main Sketch";
        public string description => file.FullName;

        public string status => done? "OK" : file.Exists ? "Exists" : "Generate";

        public GenerateMainCpp(IProject project)
        {
            file = new FileInfo(Path.Combine(project.path,"src", "make.cpp"));
            done = false;
        }

        public Action action => () =>
        {
            if (status == "Generate") File.WriteAllText(file.FullName, Strings.mainCpp);
            done = true;
        };

        bool done = false;

        FileInfo file;
    }

    class CopyCore : ITask
    {
        public string title => $"Copy core libraries";
        public string description =>$"from: {sourceFolder.FullName}";

        public string status => done ? "OK" : targetFolder.Exists ? "Exists" : "Copy";

        public CopyCore(IProject project)
        {
            sourceFolder = new DirectoryInfo(project.selectedConfiguration.core);
            targetFolder = new DirectoryInfo(Path.Combine(project.path, "core"));            
            done = false;
        }

        public Action action => () =>
        {
            if (status == "Copy")
            {                
                if(targetFolder.Exists) targetFolder.Delete(true);
                targetFolder.Create();
                Helpers.copyFilesRecursively(sourceFolder, targetFolder);
            }
            done = true;
        };

        bool done = false;

        DirectoryInfo targetFolder;
        DirectoryInfo sourceFolder;
    }
}
