using System.IO;
using VisualTeensy.Model;
using Task = System.Threading.Tasks.Task;

namespace ViewModel
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using static System.Threading.Tasks.Task;

    public class DisplayText : BaseViewModel
    {
        public string text { get; set; }
        public string action { get; set; }
        public string OK => status ? "✔" : "";

        public bool status
        {
            get => _status;
            set { SetProperty(ref _status, value); OnPropertyChanged("OK"); }
        }
        bool _status;
    }

    public class SaveWinVM : BaseViewModel
    {
        public string error
        {
            get => _error;
            set => SetProperty(ref _error, value);
        }
        string _error;

        public AsyncCommand cmdSave { get; private set; }
        async Task doSave()
        {
            projectFolder.status = makefilePath.status = buildTaskPath.status = intellisensePath.status = boardDefintionPath.status =
                coreBase.status = mainCppPath.status = compilerBase.status = makeExePath.status = false;

            try
            {
                string vsCodeFolder = Path.Combine(project.path, ".vscode");
                string vsTeensyFolder = Path.Combine(project.path, ".vsteensy");
                string srcFolder = Path.Combine(project.path, "src");
                string libFolder = Path.Combine(project.path, "lib");
                string binCoreFolder = Path.Combine(project.path, "bin", "core");
                string binUserFolder = Path.Combine(project.path, "bin", "src");
                string binLibFolder = Path.Combine(project.path, "bin", "lib");

                Directory.CreateDirectory(vsCodeFolder);
                Directory.CreateDirectory(vsTeensyFolder);
                Directory.CreateDirectory(srcFolder);
                Directory.CreateDirectory(libFolder);

                if (Directory.Exists(binCoreFolder))
                {
                    Directory.Delete(binCoreFolder, true);
                }
                if (Directory.Exists(binUserFolder))
                {
                    Directory.Delete(binUserFolder, true);
                }
                if (Directory.Exists(binLibFolder))
                {
                    Directory.Delete(binLibFolder, true);
                }
            }
            catch (UnauthorizedAccessException)
            {
                error = "Access violation! Please make sure that you have enough space and sufficient access rights.";
            }
            catch (Exception e)
            {
                error = $"Error while setting up the project folder ({project.path}). {e.GetBaseException().Message}";
                return;
            }

            await Delay(50);
            projectFolder.status = true;

            await writeProjectFiles();
            await writeLibraries();



            if (configuration.setupType == SetupTypes.expert && configuration.copyCore)
            {
                await copyCoreFiles();
            }
            else
            {
                await Delay(50);
            }

            coreBase.status = true;

            writeMainCpp();
            await Delay(50);
            mainCppPath.status = true;

            if (configuration.setupType == SetupTypes.expert && configuration.copyBoardTxt)
            {
                copyBoardFile();
            }
            await Delay(50);
            boardDefintionPath.status = true;

            await Delay(50);
            compilerBase.status = true;

            await Delay(50);
            makeExePath.status = true;

            startVSCode(projectFolder.text);

            await Task.Delay(2000);
            System.Windows.Application.Current.Shutdown();

        }

        public void startVSCode(string folder)
        {
            var vsCode = new Process();
            vsCode.StartInfo.FileName = "cmd";
            vsCode.StartInfo.Arguments = $"/c code \"{folder}\" src/main.cpp";
            vsCode.StartInfo.WorkingDirectory = folder;
            vsCode.StartInfo.UseShellExecute = false;
            vsCode.StartInfo.CreateNoWindow = true;
            vsCode.Start();
            return;
        }

        public DisplayText projectFolder { get; }
        public DisplayText makefilePath { get; }
        public DisplayText buildTaskPath { get; }
        public DisplayText intellisensePath { get; }
        public DisplayText setupFilePath { get; }
        public DisplayText coreBase { get; }
        public DisplayText coreTarget { get; }
        public DisplayText libraries { get; }
        public DisplayText mainCppPath { get; }
        public DisplayText boardDefintionPath { get; }
        public DisplayText boardDefintionTarget { get; }
        public DisplayText compilerBase { get; }
        public DisplayText makeExePath { get; }

        public bool copyBoardTxt => configuration.copyBoardTxt && configuration.setupType == SetupTypes.expert;
        public bool copyCore => configuration.copyCore && configuration.setupType == SetupTypes.expert;

        public double perc
        {
            get => _perc;
            set => SetProperty(ref _perc, value);
        }
        double _perc;

        public bool showProg
        {
            get => _showProg;
            set => SetProperty(ref _showProg, value);
        }
        bool _showProg = false;

        private void copyBoardFile()
        {
            string SourcePath = configuration.boardTxtPath;
            string DestinationPath = Path.Combine(project.path, Path.GetFileName(SourcePath));
            File.Copy(SourcePath, DestinationPath, overwrite: true);
        }
        private async Task copyCoreFiles()
        {
            showProg = true;
            string SourcePath = configuration.coreBase;
            string DestinationPath = Path.Combine(project.path, "core");

            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*.*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
            }

            var files = Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories).ToList();
            double cnt = (double)files.Count / 100;

            for (int i = 0; i < files.Count; i++)
            {
                File.Copy(files[i], files[i].Replace(SourcePath, DestinationPath), overwrite: true);
                perc = (i + 1) / (double)cnt;
                await Delay(1);
            }
            showProg = false;
        }
        private async Task writeProjectFiles()
        {
            await writeFile(makefilePath, project.selectedConfiguration.makefile);
            await writeFile(buildTaskPath, project.tasks_json);
            await writeFile(intellisensePath, project.props_json);
            await writeFile(setupFilePath, project.vsSetup_json);
        }
        private async Task writeLibraries()
        {
            string libPath = Path.Combine(projectFolder.text, "lib");

            foreach (Library library in configuration.localLibs)
            {
                if (library.sourceType == Library.SourceType.local)
                {
                    DirectoryInfo source = new DirectoryInfo(library.source);
                    DirectoryInfo target = new DirectoryInfo(Path.Combine(libPath, library.path));
                    Helpers.copyFilesRecursively(source, target);
                }
                else
                {
                    Helpers.downloadLibrary(library, libPath);
                }
                await Task.Delay(1);
            }
        }
        
        async Task writeFile(DisplayText filename, string file)
        {
            using (TextWriter writer = new StreamWriter(filename.text))
            {
                await writer.WriteAsync(file);
            }
            await Delay(50);
            filename.status = true;
        }
        
        void writeMainCpp()
        {
            string mainCppPath = Path.Combine(project.path, "src", "main.cpp");

            if (!File.Exists(mainCppPath))
            {
                using (TextWriter writer = new StreamWriter(mainCppPath))
                {
                    writer.Write(Strings.mainCpp);

                }
            }
        }

        public SaveWinVM(Project project)
        {
            cmdSave = new AsyncCommand(doSave);

            this.project = project;
            this.configuration = project.selectedConfiguration;
            this.setup = project.setup;

            projectFolder = new DisplayText()
            {
                text = project.pathError == null ? project.path : "MISSING",
                action = Directory.Exists(project.path) ? "use existing" : "generate",
                status = false
            };

            makefilePath = new DisplayText()
            {
                text = Path.Combine(projectFolder.text, "makefile"),
                action = File.Exists(Path.Combine(projectFolder.text, "makefile")) ? "overwrite" : "generate",
                status = false
            };

            buildTaskPath = new DisplayText()
            {
                text = Path.Combine(projectFolder.text, ".vscode", "tasks.json"),
                action = File.Exists(Path.Combine(projectFolder.text, ".vscode", "tasks.json")) ? "overwrite" : "generate",
                status = false
            };

            intellisensePath = new DisplayText() { text = Path.Combine(projectFolder.text, ".vscode", "c_cpp_properties.json") };
            intellisensePath.action = File.Exists(intellisensePath.text) ? "overwrite" : "generate";

            setupFilePath = new DisplayText() { text = Path.Combine(projectFolder.text, ".vsteensy", "vsteensy.json") };
            setupFilePath.action = File.Exists(setupFilePath.text) ? "overwrite" : "generate";

            coreBase = new DisplayText() { text = configuration.coreBase };
            coreBase.action = configuration.copyCore ? "copy from" : "link to";
            coreTarget = new DisplayText() { text = Path.Combine(projectFolder.text, "core") };

            mainCppPath = new DisplayText() { text = Path.Combine(project.path, "src", "main.cpp") };
            mainCppPath.action = File.Exists(mainCppPath.text) ? "skip (exists)" : "generate";

            boardDefintionPath = new DisplayText() { text = configuration.boardTxtPath };
            boardDefintionPath.action = configuration.copyBoardTxt ? "copy from" : "link to";
            boardDefintionTarget = new DisplayText() { text = Path.Combine(projectFolder.text, "boards.txt") };

            compilerBase = new DisplayText() { text = configuration.compilerBase };
            makeExePath = new DisplayText() { text = setup.makeExePath };
        }

        // private SetupData data;
        private Configuration configuration;
        private SetupData setup;
        private Project project;
    }
}
