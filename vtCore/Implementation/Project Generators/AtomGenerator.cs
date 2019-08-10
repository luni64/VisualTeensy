using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using vtCore.Interfaces;

namespace vtCore
{
    public class AtomGenerator : ICodeGenerator
    {
        public IReadOnlyList<ITask> getTasks(IProject project, LibManager libManager, SetupData setup)
        {
            var tasks = new List<ITask>();

            tasks.Add(new PrepareFolders(project));

            tasks.Add(new GenerateSettings(project));
            tasks.Add(new GenerateIntellisense(project, libManager, setup));
            tasks.Add(new GenerateMakefile(project, libManager, setup));
            tasks.Add(new GenerateTasks(project, setup));
            tasks.Add(new CopyLibs(project));

            if (project.selectedConfiguration.copyCore)
            {
                tasks.Add(new CopyCore(project));
            }

            if (project.buildSystem == BuildSystem.makefile)
            {
                tasks.Add(new GenerateSketch(project));
            }
            tasks.Add(new CleanBinaries(project));

            return tasks;
        }


        public List<ITask> tasks { get; } = null;

        static string mainFile;
        static string taskJsonFile;
        static string settingsFile;

        public async Task generate(IProject project, LibManager libManager, SetupData setup, IProgress<string> progressHandler)
        {
            progressHandler.Report("Check or create folders");
            var tasksFolder = project.path;
            var vsTeensyFolder = Path.Combine(project.path, ".vsteensy");
            var buildFolder = Path.Combine(project.path, vsTeensyFolder, "build");
            Directory.CreateDirectory(vsTeensyFolder);
            Directory.CreateDirectory(buildFolder);

            progressHandler.Report("OK");
            await Task.Delay(1);

            //taskJsonFile = Path.Combine(tasksFolder, ".build-tools.cson");            
            taskJsonFile = Path.Combine(tasksFolder, "process-palette.json");
            settingsFile = Path.Combine(vsTeensyFolder, "vsteensy.json");
            string makefile = Path.Combine(project.path, "makefile");

            // Copy makefile ----------------------------------------------------------------------
            File.WriteAllText(makefile, Makefile.generate(project, libManager, setup));
            progressHandler.Report("Generate makefile");
            progressHandler.Report("OK");
            await Task.Delay(1);

            // Task_json --------------------------------------------------------------------------
            var tasksJson = TaskFile.generate(project, setup);
            File.WriteAllText(taskJsonFile, tasksJson);
            progressHandler.Report("Generate process-palette.json");
            progressHandler.Report("OK");
            await Task.Delay(1);

            // Settings ---------------------------------------------------------------------------
            var projectSettingsJson = ProjectSettings.generate(project);
            File.WriteAllText(settingsFile, projectSettingsJson);

            if (project.buildSystem == BuildSystem.makefile)
            {
                await mkGenerator(project, libManager, setup, progressHandler);
            }
            else
            {
                await ardGenerator(project, libManager, setup, progressHandler);
            }

            progressHandler.Report("Start ATOM");
            await Task.Delay(1);
            Starter.start_atom(project.path, mainFile);
            progressHandler.Report("OK");
        }


        static public async Task ardGenerator(IProject project, LibManager libManager, SetupData setup, IProgress<string> progressHandler)
        {
            // generate make.cpp ------------------------------------------------------------------
            mainFile = Path.Combine(project.path, project.name + ".ino");
            if (!File.Exists(mainFile))
            {
                File.WriteAllText(mainFile, Strings.sketchIno);
                progressHandler.Report("Generate process-palette.json");
                progressHandler.Report("OK");
                await Task.Delay(1);
            }

            //var bat = project.path + "\\build.bat";
            //File.WriteAllText(bat, builder_build_bat.generate(project,setup));

        }

        static public async Task mkGenerator(IProject project, LibManager libManager, SetupData setup, IProgress<string> progressHandler)
        {
            //string srcFolder = Path.Combine(project.path, "src");
            //string libFolder = Path.Combine(project.path, "lib");
            //Directory.CreateDirectory(srcFolder);
            //Directory.CreateDirectory(libFolder);

            //// copy local libraries -----------------------------------------------------------
            //foreach (Library library in project.selectedConfiguration.localLibs)
            //{
            //    if (library.sourceType == Library.SourceType.local)
            //    {
            //        progressHandler.Report($"Copy library {library.name}");
            //        await Task.Delay(1);

            //        DirectoryInfo source = new DirectoryInfo(library.source);
            //        DirectoryInfo target = new DirectoryInfo(Path.Combine(libFolder, library.path));
            //        Helpers.copyFilesRecursively(source, target);

            //        progressHandler.Report($"OK");
            //        await Task.Delay(1);
            //    }
            //    else
            //    {
            //        progressHandler.Report($"Download library {library.name}");
            //        await Task.Delay(1);

            //        await Helpers.downloadLibrary(library, libBase);

            //        progressHandler.Report($"OK");
            //        await Task.Delay(1);
            //    }
            //}

            //// generate make.cpp ------------------------------------------------------------------
            //mainFile = Path.Combine(srcFolder, "main.cpp");
            //if (!File.Exists(mainFile))
            //{
            //    File.WriteAllText(mainFile, Strings.mainCpp);
            //}
            await Task.CompletedTask;
        }
    }
}

