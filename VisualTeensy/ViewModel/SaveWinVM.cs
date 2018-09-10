using Board2Make.Model;
using System.IO;
using System.Threading.Tasks;

namespace ViewModel
{
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
        public DisplayText projectFolder { get; }
        public DisplayText makefilePath { get; }
        public DisplayText buildTaskPath { get; }
        public DisplayText intellisensePath { get; }
        public DisplayText coreBase { get; }
        public DisplayText coreTarget { get; }
        public DisplayText mainCppPath { get; }
        public DisplayText boardDefintionPath { get; }
        public DisplayText boardDefintionTarget { get; }
        public DisplayText compilerBase { get; }
        public DisplayText makeExePath { get; }

        public bool copyBoardTxt => data.copyBoardTxt && !data.fromArduino;
        public bool copyCore => data.copyCore && !data.fromArduino;

        async System.Threading.Tasks.Task copyCoreFiles()
        {
            showProg = true;
            string SourcePath = data.coreBase;
            string DestinationPath = Path.Combine(data.projectBase, "core");

            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(SourcePath, "*.*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(SourcePath, DestinationPath));
            }

            await Delay(1);

            //Copy all the files

            var files = Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories).ToList();
            double cnt = (double) files.Count/100;

            for (int i = 0; i < files.Count; i++)
            {
                File.Copy(files[i], files[i].Replace(SourcePath, DestinationPath), overwrite: true);
                perc = (i+1) / (double)cnt;
                await Delay(1);
            }
            showProg = false;
        }

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

        
        void copyBoardFile()
        {
            string SourcePath = data.boardTxtPath;
            string DestinationPath = Path.Combine(data.projectBase, Path.GetFileName(SourcePath));
            File.Copy(SourcePath, DestinationPath, overwrite: true);
        }

        async System.Threading.Tasks.Task writeProjectFiles()
        {
            string DestinationPath = Path.Combine(makefilePath.text);
            using (TextWriter writer = new StreamWriter(DestinationPath))
            {
                await writer.WriteAsync(data.makefile);
            }
            await Delay(50);
            makefilePath.status = true;

            DestinationPath = Path.Combine(buildTaskPath.text);
            using (TextWriter writer = new StreamWriter(DestinationPath))
            {
                await writer.WriteAsync(data.tasks_json);
            }
            await Delay(50);
            buildTaskPath.status = true;

            DestinationPath = Path.Combine(intellisensePath.text);
            using (TextWriter writer = new StreamWriter(DestinationPath))
            {
                await writer.WriteAsync(data.propsFile);
            }
            await Delay(50);
            intellisensePath.status = true;
        }

        void writeMainCpp()
        {
            string mainCppPath = Path.Combine(data.projectBase, "src", "main.cpp");

            if (!File.Exists(mainCppPath))
            {
                using (TextWriter writer = new StreamWriter(mainCppPath))
                {
                    writer.Write(mainCpp);

                }
            }
        }

        public AsyncCommand cmdSave { get; private set; }
        

        //   public RelayCommand cmdSave { get; private set; }
        void doSave(object o)
        {
            Directory.CreateDirectory(Path.Combine(data.projectBase, ".vscode"));
            Directory.CreateDirectory(Path.Combine(data.projectBase, "src"));
            projectFolder.status = true;
            //Thread.Sleep(200);


            //Thread.Sleep(200);

            writeMainCpp();
            mainCppPath.status = true;
            //Thread.Sleep(200);

            if (!data.fromArduino && data.copyBoardTxt)
            {
                copyBoardFile();
            }
            boardDefintionPath.status = true;

            if (!data.fromArduino && data.copyCore)
            {
                copyCoreFiles();
            }
            coreBase.status = true;

            compilerBase.status = true;
            makeExePath.status = true;
        }
        

        async System.Threading.Tasks.Task doSSave()
        {
            projectFolder.status = makefilePath.status = buildTaskPath.status = intellisensePath.status = boardDefintionPath.status = 
                coreBase.status = mainCppPath.status = compilerBase.status = makeExePath.status = false;

            Directory.CreateDirectory(Path.Combine(data.projectBase, ".vscode"));
            Directory.CreateDirectory(Path.Combine(data.projectBase, "src"));
            await Delay(50);
            projectFolder.status = true;

            await writeProjectFiles();

            if (!data.fromArduino && data.copyCore)
            {
                await copyCoreFiles();
            }
            else await Delay(50);                        
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
        }

        public SaveWinVM(SetupData data)
        {
            // cmdSave = new RelayCommand(doSave);

            cmdSave = new AsyncCommand(doSSave);
            this.data = data;

            projectFolder = new DisplayText()
            {
                text = data.projectBaseError == null ? data.projectBase : "MISSING",
                action = Directory.Exists(data.projectBase) ? "overwrite" : "generate",
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


        SetupData data;

        const string mainCpp =
            "#include \"Arduino.h\"\n\n" +

            "void setup()\n" +
            "{\n" +
            "  pinMode(LED_BUILTIN,OUTPUT);\n" +
            "}\n\n" +

            "void loop()\n" +
            "{\n" +
            "  digitalWriteFast(LED_BUILTIN,!digitalReadFast(LED_BUILTIN));\n" +
            "  delay(100);\n" +
            "}\n";



    }
}
