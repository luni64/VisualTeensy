using VisualTeensy.Model;
using System.IO;
using Task = System.Threading.Tasks.Task;

namespace ViewModel
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Web.Script.Serialization;
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
                string vsCodeFolder = Path.Combine(data.projectBase, ".vscode");
                string srcFolder = Path.Combine(data.projectBase, "src");
                string binCoreFolder = Path.Combine(data.projectBase, "bin", "core");
                string binUserFolder = Path.Combine(data.projectBase, "bin", "src");

                Directory.CreateDirectory(vsCodeFolder);
                Directory.CreateDirectory(srcFolder);
                if (Directory.Exists(binCoreFolder))
                {
                    Directory.Delete(binCoreFolder, true);
                }

                if (Directory.Exists(binUserFolder))
                {
                    Directory.Delete(binUserFolder, true);
                }
            }
            catch (UnauthorizedAccessException)
            {
                error = "Access violation! Please make sure that you have enough space and sufficient access rights.";
            }
            catch (Exception e)
            {
                error = $"Error while setting up the project folder ({data.projectBase}). {e.GetBaseException().Message}";
                return;
            }


            await Delay(50);
            projectFolder.status = true;

            await writeProjectFiles();

            if (!data.fromArduino && data.copyCore)
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

            if (!data.fromArduino && data.copyBoardTxt)
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
        public DisplayText mainCppPath { get; }
        public DisplayText boardDefintionPath { get; }
        public DisplayText boardDefintionTarget { get; }
        public DisplayText compilerBase { get; }
        public DisplayText makeExePath { get; }

        public bool copyBoardTxt => data.copyBoardTxt && !data.fromArduino;
        public bool copyCore => data.copyCore && !data.fromArduino;

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
            string SourcePath = data.boardTxtPath;
            string DestinationPath = Path.Combine(data.projectBase, Path.GetFileName(SourcePath));
            File.Copy(SourcePath, DestinationPath, overwrite: true);
        }
        private async Task copyCoreFiles()
        {
            showProg = true;
            string SourcePath = data.coreBase;
            string DestinationPath = Path.Combine(data.projectBase, "core");

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
            //string DestinationPath = Path.Combine(makefilePath.text);
            //using (TextWriter writer = new StreamWriter(DestinationPath))
            //{
            //    await writer.WriteAsync(data.makefile);
            //}
            //await Delay(50);
            //makefilePath.status = true;

            //DestinationPath = Path.Combine(buildTaskPath.text);
            //using (TextWriter writer = new StreamWriter(DestinationPath))
            //{
            //    await writer.WriteAsync(data.tasks_json);
            //}
            //await Delay(50);
            //buildTaskPath.status = true;

            //DestinationPath = Path.Combine(intellisensePath.text);
            //using (TextWriter writer = new StreamWriter(DestinationPath))
            //{
            //    await writer.WriteAsync(data.propsFile);
            //}
            //await Delay(50);
            //intellisensePath.status = true;
            await writeFile(makefilePath, data.makefile);
            await writeFile(buildTaskPath, data.tasks_json);
            await writeFile(intellisensePath, data.props_json);
            await writeFile(setupFilePath, data.vsSetup_json);

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
            string mainCppPath = Path.Combine(data.projectBase, "src", "main.cpp");

            if (!File.Exists(mainCppPath))
            {
                using (TextWriter writer = new StreamWriter(mainCppPath))
                {
                    writer.Write(Strings.mainCpp);

                }
            }
        }

        public SaveWinVM(SetupData data)
        {
            cmdSave = new AsyncCommand(doSave);
            this.data = data;

            projectFolder = new DisplayText()
            {
                text = data.projectBaseError == null ? data.projectBase : "MISSING",
                action = Directory.Exists(data.projectBase) ? "use existing" : "generate",
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

            setupFilePath = new DisplayText() { text = Path.Combine(projectFolder.text, ".vscode", "visual_teensy.json") };
            setupFilePath.action = File.Exists(setupFilePath.text) ? "overwrite" : "generate";


            coreBase = new DisplayText() { text = data.coreBase };
            coreBase.action = data.copyCore ? "copy from" : "link to";
            coreTarget = new DisplayText() { text = Path.Combine(projectFolder.text, "core") };

            mainCppPath = new DisplayText() { text = Path.Combine(data.projectBase, "src", "main.cpp") };
            mainCppPath.action = File.Exists(mainCppPath.text) ? "skip (exists)" : "generate";

            boardDefintionPath = new DisplayText() { text = data.boardTxtPath };
            boardDefintionPath.action = data.copyBoardTxt ? "copy from" : "link to";
            boardDefintionTarget = new DisplayText() { text = Path.Combine(projectFolder.text, "boards.txt") };

            compilerBase = new DisplayText() { text = data.compilerBase };
            makeExePath = new DisplayText() { text = data.makeExePath };
        }

        private SetupData data;
    }
}
