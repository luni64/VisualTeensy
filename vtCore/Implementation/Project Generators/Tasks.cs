using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

        public Func<Task> action => async () =>
        {
            if (!done)
            {
                vsCodeFolder.Create();
                vsTeensyFolder.Create();
                srcFolder.Create();
            }
            done = true;
            await Task.CompletedTask;
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

        public Func<Task> action => async () =>
        {
            if (buildFolder.Exists)
            {
                buildFolder.Delete(true);
            }
            buildFolder.Create();
            done = true;
            await Task.CompletedTask;
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

        public Func<Task> action => async () =>
        {
            string properties = IntellisenseFile.generate(project, libManager, setup);
            File.WriteAllText(c_cpp_propertiesFile.FullName, properties);
            done = true;
            await Task.Delay(1000);
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

        public Func<Task> action => async () =>
        {
            string properties = ProjectSettings.generate(project);
            File.WriteAllText(settingsFile.FullName, properties);
            done = true;
            await Task.CompletedTask;
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

        public Func<Task> action => async () =>
        {
            if (status != "Up-To-Date")
                File.WriteAllText(file.FullName, newMakefile);
            status = "OK";
            await Task.CompletedTask;
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

        public Func<Task> action => async () =>
        {
            File.WriteAllText(file.FullName, tasksJson);
            done = true;
            await Task.CompletedTask;
        };

        string tasksJson;

        FileInfo file;
    }

    class GenerateMainCpp : ITask
    {
        public string title => $"Main Sketch";
        public string description => file.FullName;

        public string status => done ? "OK" : file.Exists ? "Exists" : "Generate";

        public GenerateMainCpp(IProject project)
        {
            file = new FileInfo(Path.Combine(project.path, "src", "make.cpp"));
            done = false;
        }

        public Func<Task> action => async () =>
        {
            if (status == "Generate") File.WriteAllText(file.FullName, Strings.mainCpp);
            done = true;
            await Task.CompletedTask;
        };

        bool done = false;

        FileInfo file;
    }

    class CopyCore : ITask
    {
        public string title => $"Copy core libraries";
        public string description => $"from: {sourceUri.AbsolutePath} to: ./core/";

        public string status => done ? "OK" : Directory.Exists(targetUri.AbsolutePath) ? "Exists" : "Copy";

        public CopyCore(IProject project)
        {
            sourceUri = new Uri(project.selectedConfiguration.core);
            targetUri = new Uri(Path.Combine(project.path, "core"));
            done = false;
        }

        public Func<Task> action => async () =>
        {
            if (status == "Copy")
            {                
                Helpers.copyFilesRecursively(sourceUri, targetUri);
            }
            done = true;
            await Task.CompletedTask;
        };

        bool done = false;

        Uri targetUri, sourceUri;
    }

    class CopyLibs : ITask
    {
        public string title => $"Copy local libraries";
        public string description => $"to: ./lib/";

        public string status => done ? "OK" : libBase.Exists ? "Exists" : "Copy";

        public CopyLibs(IProject project)
        {
            this.project = project;
            libBase = new DirectoryInfo(Path.Combine(project.path, "lib"));
            done = false;
        }

        
        public Func<Task> action => async () =>
        {
            var baseUri = new Uri(libBase.FullName);

            foreach (Library library in project.selectedConfiguration.localLibs)
            {               
               // var targetUri = new Uri(Path.Combine(baseUri.AbsolutePath, library.sourceUri.Segments.Last()));
                                
                if (library.isLocalSource & library.sourceUri != library.targetUri)
                {
                    Helpers.copyFilesRecursively(library.sourceUri, library.targetUri);
                }
                else if (library.isWebSource)
                {
                    await Helpers.downloadLibrary(library, libBase);
                }                
            };
            done = true;
        };

        bool done = false;

        DirectoryInfo libBase;


        IProject project;
    }
}
