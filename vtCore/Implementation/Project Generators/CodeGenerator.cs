using System.Collections.Generic;
using vtCore.Interfaces;

namespace vtCore
{
    public static class CodeGenerator
    {
        public static IReadOnlyList<ITask> getTasks(IProject project, LibManager libManager, SetupData setup)
        {
            var tasks = new List<ITask>();

            switch (project.target)
            {
                case Target.vsCode:
                    tasks.Add(new PrepareFolders(project));
                    tasks.Add(new GenerateSettings(project));
                    tasks.Add(new GenerateIntellisense(project, libManager, setup));
                    tasks.Add(new GenerateMakefile(project, libManager, setup));
                    tasks.Add(new GenerateTasks(project, setup));
                    if (project.buildSystem == BuildSystem.makefile)
                    {
                        tasks.Add(new CopyLibs(project));
                        if (project.selectedConfiguration.copyCore)
                        {
                            tasks.Add(new CopyCore(project));
                        }
                    }
                    if(project.debugSupport == DebugSupport.cortex_debug)
                    {
                        tasks.Add(new GenerateDebugSupport(project, setup));                            
                    }
                    tasks.Add(new CleanBinaries(project));
                    tasks.Add(new GenerateSketch(project));
                    break;

                case Target.atom:
                    break;
                case Target.sublimeText:
                    break;
            }

            return tasks;
        }

        // static string mainFile;

        //public async Task generate(IProject p, LibManager l, SetupData s, IProgress<string> progressHandler)
        //{

        //    return;

        //    //progressHandler.Report("Check or create folders");
        //    var vsCodeFolder = Path.Combine(project.path, ".vscode");
        //    var vsTeensyFolder = Path.Combine(project.path, ".vsteensy");
        //    //var buildFolder = Path.Combine(project.path, vsTeensyFolder, "build");
        //    //Directory.CreateDirectory(vsCodeFolder);
        //    //Directory.CreateDirectory(vsTeensyFolder);
        //    //if (Directory.Exists(buildFolder)) Directory.Delete(buildFolder, true);
        //    //Directory.CreateDirectory(buildFolder);

        //    // await Task.Delay(1);
        //    //progressHandler.Report("OK");

        //    // Intellisense -----------------------------------------------------------------------
        //    //progressHandler.Report("Generate c_cpp_properties.json");
        //    //var c_cpp_propsFile = Path.Combine(vsCodeFolder, "c_cpp_properties.json");
        //    //var c_cpp_props = IntellisenseFile.generate(project, libManager, setup);
        //    //File.WriteAllText(c_cpp_propsFile, c_cpp_props);
        //    //progressHandler.Report("OK");
        //    //await Task.Delay(1);

        //    //// Settings ---------------------------------------------------------------------------
        //    //progressHandler.Report("Generate vsteensy.json");
        //    //var settingsFile = Path.Combine(vsTeensyFolder, "vsteensy.json");
        //    //var projectSettingsJson = ProjectSettings.generate(project);
        //    //File.WriteAllText(settingsFile, projectSettingsJson);
        //    //progressHandler.Report("OK");
        //    //await Task.Delay(1);

        //    // Makefile ---------------------------------------------------------------------------
        //    //progressHandler.Report("Generate makefile");
        //    //var makefile = Path.Combine(project.path, "makefile");
        //    //File.WriteAllText(makefile, Makefile.generate(project, libManager, setup));
        //    //progressHandler.Report("OK");
        //    //await Task.Delay(1);

        //    // Task_json --------------------------------------------------------------------------
        //    //progressHandler.Report("Generate tasks.json");
        //    //var taskJsonFile = Path.Combine(vsCodeFolder, "tasks.json");
        //    //var tasks_json = TaskFile.generate(project, libManager, setup);
        //    //File.WriteAllText(taskJsonFile, tasks_json);
        //    //progressHandler.Report("OK");
        //    //await Task.Delay(1);

        //    // Debugging --------------------------------------------------------------------------
        //    if (project.debugSupport != DebugSupport.none)
        //    {
        //        progressHandler.Report("Generate flash.jlink");
        //        var flashJinkFile = Path.Combine(vsTeensyFolder, "flash.jlink");
        //        var flash_jlink = JLinkUploadScript.generate(project, setup);
        //        File.WriteAllText(flashJinkFile, flash_jlink);
        //        progressHandler.Report("OK");
        //        await Task.Delay(1);

        //        progressHandler.Report("Generate launch.json");
        //        var launchJsonFile = Path.Combine(vsCodeFolder, "launch.json");
        //        var launch_json = DebugFile.generate(project, setup);
        //        File.WriteAllText(launchJsonFile, launch_json);
        //        progressHandler.Report("OK");
        //        await Task.Delay(1);
        //    }

        //    // BuildSystem Makefile ---------------------------------------------------------------
        //    if (project.buildSystem == BuildSystem.makefile)
        //    {
        //        DirectoryInfo source, target;

        //        target = new DirectoryInfo(Path.Combine(project.path, "src"));
        //        target.Create();

        //        //FileInfo mainFile = new FileInfo(Path.Combine(target.FullName, "main.cpp"));
        //        ////mainFile = Path.Combine(srcFolder, "main.cpp");
        //        //if (!mainFile.Exists)
        //        ////if (!File.Exists(mainFile))
        //        //{
        //        //    progressHandler.Report($"{mainFile.Name} generated");
        //        //    File.WriteAllText(mainFile.FullName, Strings.mainCpp);
        //        //    progressHandler.Report($"OK");
        //        //    await Task.Delay(1);
        //        //}

        //        //// copy core ----------------------------------------------------------------------                
        //        //DirectoryInfo coreFolder = new DirectoryInfo(Path.Combine(project.path, "core"));
        //        //if (coreFolder.Exists) coreFolder.Delete(true);

        //        //if (project.selectedConfiguration.copyCore)
        //        //{
        //        //    source = new DirectoryInfo(project.selectedConfiguration.core);
        //        //    Helpers.copyFilesRecursively(source, coreFolder);
        //        //}

        //        // copy local libraries -----------------------------------------------------------
        //        if (project.selectedConfiguration.localLibs.Any())
        //        {
        //            DirectoryInfo libFolder = new DirectoryInfo(Path.Combine(project.path, "lib"));
        //            libFolder.Create();

        //            foreach (Library library in project.selectedConfiguration.localLibs)
        //            {
        //                if (library.sourceType != Library.SourceType.net)
        //                {
        //                    target = new DirectoryInfo(Path.Combine(libFolder.FullName, library.path));
        //                    source = new DirectoryInfo(library.source);
        //                    if (source.FullName != target.FullName)
        //                    {
        //                        if (target.Exists) target.Delete();

        //                        progressHandler.Report($"Copy library {library.name}");
        //                        await Task.Delay(1);

        //                        Helpers.copyFilesRecursively(source, target);

        //                        progressHandler.Report($"OK");
        //                        await Task.Delay(1);
        //                    }
        //                }
        //                else
        //                {
        //                    progressHandler.Report($"Download library {library.name}");
        //                    await Task.Delay(1);

        //                    Helpers.downloadLibrary(library, libFolder.FullName);

        //                    progressHandler.Report($"OK");
        //                    await Task.Delay(1);
        //                }
        //            }
        //        }


        //    }
        //    else    // BuildSystem Arduino Builder-------------------------------------------------
        //    {
        //        mainFile = Path.Combine(project.path, project.name + ".ino");
        //        if (!File.Exists(mainFile))
        //        {
        //            progressHandler.Report($"{mainFile} generated");
        //            File.WriteAllText(mainFile, Strings.sketchIno);
        //            progressHandler.Report("OK");
        //            await Task.Delay(1);
        //        }
        //    }
        //    progressHandler.Report("Start vsCode");
        //    await Task.Delay(1);
        //    Starter.start_vsCode(project.path, mainFile);
        //    progressHandler.Report("OK");
        //}


        //static public void ardGenerator(IProject project, LibManager libManager, SetupData setup)
        //{


        //}

        //static public void mkGenerator(IProject project, LibManager libManager, SetupData setup)
        //{


        //    // copy makefile ----------------------------------------------------------------------

        //}
    }
}
