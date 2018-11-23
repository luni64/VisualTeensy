using System.Diagnostics;
using System.IO;

namespace vtCore
{
    static class Starter
    {
        static public void start_vsCode(string folder, string file)
        {
            var vsCode = new Process();
            vsCode.StartInfo.FileName = "cmd";
            vsCode.StartInfo.Arguments = $"/c code \"{folder}\" {file}";
            vsCode.StartInfo.WorkingDirectory = folder;
            vsCode.StartInfo.UseShellExecute = false;
            vsCode.StartInfo.CreateNoWindow = true;
            vsCode.Start();
            return;
        }

    }

    static public class vsCodeProjectGenerator
    {
        static string vsCodeFolder;
        static string vsTeensyFolder;
        static string buildFolder;
        static string mainFile;
        static string taskJsonFile;
        static string intellisenseFile;
        static string settingsFile;


        static public void generate(IProject project)
        {
            vsCodeFolder = Path.Combine(project.path, ".vscode");
            vsTeensyFolder = Path.Combine(project.path, ".vsteensy");
            taskJsonFile = Path.Combine(vsCodeFolder, "tasks.json");
            intellisenseFile = Path.Combine(vsCodeFolder, "c_cpp_properties.json");
            settingsFile = Path.Combine(vsTeensyFolder, "vsteensy.json");
            buildFolder = Path.Combine(project.path, vsTeensyFolder, "build");

            Directory.CreateDirectory(vsCodeFolder);
            Directory.CreateDirectory(vsTeensyFolder);
            Directory.CreateDirectory(buildFolder);

            File.WriteAllText(taskJsonFile, project.tasks_json);
            File.WriteAllText(intellisenseFile, project.props_json);
            File.WriteAllText(settingsFile, project.vsSetup_json);

            if (project.isMakefileBuild)
            {
                mkGenerator(project);
            }
            else
            {
                ardGenerator(project);
            }

            Starter.start_vsCode(project.path, mainFile);
            Directory.CreateDirectory(vsTeensyFolder);

            //await Task.Delay(2000);
            //System.Windows.Application.Current.Shutdown();
        }


        static public void ardGenerator(IProject project)
        {
            mainFile = Path.Combine(project.path, project.name + ".ino");
            if (!File.Exists(mainFile))
            {
                File.WriteAllText(mainFile, Strings.sketchIno);
            }
        }

        static public void mkGenerator(IProject project)
        {
            string srcFolder = Path.Combine(project.path, "src");            
            string libFolder = Path.Combine(project.path, "lib");
            string makefile = Path.Combine(project.path, "makefile");
            Directory.CreateDirectory(srcFolder);
            Directory.CreateDirectory(libFolder);
                
            // copy local libraries ---------------------------------------------------------------
            foreach (Library library in project.selectedConfiguration.localLibs)
            {
                if (library.sourceType == Library.SourceType.local)
                {
                    DirectoryInfo source = new DirectoryInfo(library.source);
                    DirectoryInfo target = new DirectoryInfo(Path.Combine(libFolder, library.path));
                    Helpers.copyFilesRecursively(source, target);
                }
                else
                {
                    Helpers.downloadLibrary(library, libFolder);
                }                
            }

            // generate make.cpp ------------------------------------------------------------------
            mainFile = Path.Combine(srcFolder, "main.cpp");
            if (!File.Exists(mainFile))
            {
                File.WriteAllText(mainFile, Strings.mainCpp);
            }

            // copy makefile ----------------------------------------------------------------------
            File.WriteAllText(makefile, project.selectedConfiguration.makefile);
        }
    }
}
