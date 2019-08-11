using System;
using System.IO;
using System.Threading.Tasks;
using vtCore.Interfaces;

namespace vtCore
{
    // -----------------------------------------------------------------------
    // Prepare Folders
    // -----------------------------------------------------------------------

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
            srcFolder = project.buildSystem == BuildSystem.makefile ? new DirectoryInfo(Path.Combine(project.path, "src")) : null;
            
            done = vsCodeFolder.Exists && vsTeensyFolder.Exists && srcFolder != null && srcFolder.Exists;
        }

        public Func<Task> action => async () =>
        {
            if (!done)
            {
                vsCodeFolder.Create();
                vsTeensyFolder.Create();
                srcFolder?.Create();
            }
            done = true;
            await Task.CompletedTask;
        };

        private readonly DirectoryInfo vsCodeFolder;
        private readonly DirectoryInfo vsTeensyFolder;
        private readonly DirectoryInfo srcFolder;
    }

    // -----------------------------------------------------------------------
    // Clean Binaries 
    // -----------------------------------------------------------------------

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

        readonly DirectoryInfo buildFolder;
        bool done = false;

    }

    //-----------------------------------------------------------------------
    // Generate Intellisense 
    //-----------------------------------------------------------------------
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
            await Task.CompletedTask;
        };

        private readonly bool exists;
        private bool done;
        readonly IProject project;
        readonly LibManager libManager;
        readonly SetupData setup;
        readonly FileInfo c_cpp_propertiesFile;
    }


    //-----------------------------------------------------------------------
    // Generate Settings
    //-----------------------------------------------------------------------
    class GenerateSettings : ITask
    {
        public string title => $"VisualTeensy settings";
        public string description => settingsFile.FullName;
        public string status => done ? "OK" : exists ? "Overwrite" : "Generate";

        private readonly bool exists;
        private bool done;

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

        readonly IProject project;
        readonly FileInfo settingsFile;
    }

    //-----------------------------------------------------------------------
    // Generate Makefile 
    //-----------------------------------------------------------------------
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

        readonly string newMakefile;
        readonly FileInfo file;
    }

    //-----------------------------------------------------------------------
    // Generate  Tasks.json 
    //-----------------------------------------------------------------------
    class GenerateTasks : ITask
    {
        public string title => $"Tasks File";
        public string description => file.FullName;
        public string status => done ? "OK" : file.Exists ? "Overwrite" : "Generate";

        bool done;

        public GenerateTasks(IProject project, SetupData setup)
        {
            file = new FileInfo(Path.Combine(project.path, ".vscode", "tasks.json"));
            tasksJson = TaskFile.generate(project, setup);

            done = false;
        }

        public Func<Task> action => async () =>
        {
            File.WriteAllText(file.FullName, tasksJson);
            done = true;
            await Task.CompletedTask;
        };

        private readonly string tasksJson;
        private readonly FileInfo file;
    }

    //-----------------------------------------------------------------------
    // Generate Sketch (main.cpp oder name.ino)
    //-----------------------------------------------------------------------
    class GenerateSketch : ITask
    {
        public string title => $"Main Sketch";
        public string description => mainSketch.FullName;

        public string status => done ? "OK" : mainSketch.Exists ? "Exists" : "Generate";

        public GenerateSketch(IProject project)
        {
            mainSketch = new FileInfo(project.mainSketchPath);
            if (project.buildSystem == BuildSystem.makefile)
            {
                fileContent = Strings.mainCpp;
            }
            else
            {                
                fileContent = Strings.sketchIno;
            }
            done = false;

            this.project = project;
        }

        public Func<Task> action => async () =>
        {
            if (status == "Generate")
            {                
                File.WriteAllText(mainSketch.FullName, fileContent);
            }
            done = true;
                       
            await Task.CompletedTask;
        };

        bool done = false;

        private readonly FileInfo mainSketch;
        private readonly string fileContent;
        private readonly IProject project;
    }

    //--------------------------------------------------------------------------------------------
    // Copy Core Libraries 
    //--------------------------------------------------------------------------------------------
    class CopyCore : ITask
    {
        public string title => $"Copy core libraries";
        public string description => $"from: {sourceUri.LocalPath} to: ./core/";

        public string status => done ? "OK" : Directory.Exists(targetUri.LocalPath) ? "Exists" : "Copy";

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
        private readonly Uri targetUri;
        private readonly Uri sourceUri;
    }

    //--------------------------------------------------------------------------------------------
    // Copy local libraries
    //--------------------------------------------------------------------------------------------
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

            foreach (IProjectLibrary library in project.selectedConfiguration.localLibs)
            {
                if (library.isLocalSource)
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

        private readonly DirectoryInfo libBase;
        private readonly IProject project;
    }
}
