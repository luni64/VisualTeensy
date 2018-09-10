using Board2Make.Model;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ViewModel
{
    public class ViewModel : BaseViewModel, IDataErrorInfo
    {
        public RelayCommand cmdGenerate { get; private set; }
        void doGenerate(object o)
        {
            Message("Generate");

            //if (model.data.projectBaseError == null)
            //{
            //    string projectPath = model.data.projectBase;
            //    string settingsPath = Path.Combine(projectPath, ".vscode");
            //    string srcPath = Path.Combine(projectPath, "src");

            //    Directory.CreateDirectory(settingsPath);
            //    Directory.CreateDirectory(srcPath);

            //    string makefilePath = Path.Combine(projectPath, "makefile");
            //    using (TextWriter writer = new StreamWriter(makefilePath))
            //    {
            //        writer.Write(makefile);
            //    }

            //    string taskFilePath = Path.Combine(settingsPath, "tasks.json");
            //    using (TextWriter writer = new StreamWriter(taskFilePath))
            //    {
            //        writer.Write(taskFile);
            //    }

            //    string propFilePath = Path.Combine(settingsPath, "c_cpp_properties.json");
            //    using (TextWriter writer = new StreamWriter(propFilePath))
            //    {
            //        writer.Write(propFile);
            //    }

            //    string mainPath = Path.Combine(srcPath, "main.cpp");

            //    using (TextWriter writer = new StreamWriter(mainPath))
            //    {
            //        writer.Write(mainCpp);
            //    }
            //}
        }

        public RelayCommand cmdClose { get; private set; }
        void doClose(object o)
        {
            model.saveSettings();
        }



        const string mainCpp =
            "#include \"arduino.h\"\n\n" +
            "void setup()\n" +
            "{\n" +
            "\tpinMode(LED_BUILTIN,OUTPUT);\n" +
            "}\n\n" +

            "void loop()\n" +
            "{\n" +
                "\tdigitalWriteFast(LED_BUILTIN,!digitalReadFast(LED_BUILTIN));\n" +
                "\tdelay(250);\n" +
            "}\n";


        #region IDataErrorInfo ------------------------------------------------



        public string Error => throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                string error;

                switch (columnName)
                {
                    case "projectPath":
                        error = model.data.projectBaseError;
                        break;

                    case "projectName":
                        error = model.data.projectNameError;
                        break;

                    case "arduinoPath":
                        error = quickSetup ? model.data.arduinoBaseError : null;
                        break;

                    case "boardTxtPath":
                        error = model.data.boardTxtPathError;
                        break;

                    case "corePath":
                        error = quickSetup ? null : model.data.corePathError;
                        break;

                    case "compilerPath":
                        error = model.data.compilerPathError;
                        break;

                    case "makePath":
                        error = model.data.makeExePathError;
                        break;

                    case "uploadTyPath":
                        error = model.data.uplTyBaseError;
                        break;

                    case "boardVMs":
                        error = null;
                        break;

                    default:
                        error = null;
                        break;
                }
                return error;


            }
        }
        #endregion


        #region Properties ------------------------------------------------------
        public String makefile => model.data.makefile;
        public String propFile => model.data.propsFile;
        public String taskFile => model.data.tasks_json;

        public String makeFileName => Path.Combine(projectPath ?? "", "makefile");
        public String propFileName => Path.Combine(projectPath ?? "", ".vscode", "c_cpp_properties.json");
        public String taskFileName => Path.Combine(projectPath ?? "", ".vscode", "tasks.json");


        public String projectPath
        {
            get => model.data.projectBase;
            set
            {
                if (value != model.data.projectBase)
                {
                    model.data.projectBase = value.Trim();
                    OnPropertyChanged();
                    OnPropertyChanged("makeFileName");
                    OnPropertyChanged("propFileName");
                    OnPropertyChanged("taskFileName");
                }
            }
        }

        public string projectName
        {
            get => model.data.projectName;
            set
            {
                if (value != model.data.projectName)
                {
                    model.data.projectName = value.Trim().Replace(" ", "_");
                    OnPropertyChanged();
                    updateFiles();
                }
            }
        }

        public String arduinoPath
        {
            get => model.data.arduinoBase;
            set
            {
                if (value != model.data.arduinoBase)
                {
                    model.data.arduinoBase = value.Trim();
                    OnPropertyChanged();
                    updateBoards();
                }
            }
        }
        public String boardTxtPath
        {
            get => model.data.boardTxtPath;
            set
            {
                if (value != model.data.boardTxtPath)
                {
                    model.data.boardTxtPath = value.Trim();
                    OnPropertyChanged();
                    updateBoards();
                }
            }
        }
        public bool copyBoardTxt
        {
            get => model.data.copyBoardTxt;
            set
            {
                if (value != model.data.copyBoardTxt)
                {
                    model.data.copyBoardTxt = value;
                    OnPropertyChanged();
                    updateFiles();
                }
            }
        }

        public bool copyCore
        {
            get => model.data.copyCore;
            set
            {
                if (value != model.data.copyCore)
                {
                    model.data.copyCore = value;
                    OnPropertyChanged();
                    updateFiles();
                }
            }
        }

        public String corePath
        {
            get => model.data.coreBase;
            set
            {
                if (value != model.data.coreBase)
                {
                    model.data.coreBase = value.Trim();
                    OnPropertyChanged();
                    updateFiles();
                }
            }
        }
        public String compilerPath
        {
            get => model.data.compilerBase;
            set
            {
                if (value != model.data.compilerBase)
                {
                    model.data.compilerBase = value.Trim();
                    OnPropertyChanged();
                    updateFiles();
                }
            }
        }
        public String makePath
        {
            get => model.data.makeExePath;
            set
            {
                if (value != model.data.makeExePath)
                {
                    model.data.makeExePath = value.Trim();
                    OnPropertyChanged();
                    updateFiles();
                }
            }
        }


        public String uploadTyPath
        {
            get => model.data.uplTyBase;
            set
            {
                if (value != model.data.uplTyBase)
                {
                    model.data.uplTyBase = value;
                    OnPropertyChanged();
                }
            }
        }

        public String uploadPjrcPath
        {
            get => _uploadPjrcPath;
            set => SetProperty(ref _uploadPjrcPath, value);
        }
        string _uploadPjrcPath;

        public bool quickSetup
        {
            get => model.data.fromArduino;
            set
            {
                if (model.data.fromArduino != value)
                {
                    model.data.fromArduino = value;
                    updateBoards();
                    OnPropertyChanged("");
                }
            }
        }


        public String outputFilename
        {
            get => _outputFilename;
            set
            {
                if (_outputFilename != value)
                {
                    _outputFilename = value;
                    RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\lunOptics\\Board2Make");
                    key.SetValue("output", _outputFilename);
                }
            }
        }
        String _outputFilename;

        public String Title => "lunOptics - VisualTeensy V0.1";

        public ObservableCollection<BoardVM> boardVMs { get; } = new ObservableCollection<BoardVM>();

        public BoardVM selectedBoard
        {
            get => _selectedBoard;
            set
            {
                if (value != _selectedBoard)
                {
                    _selectedBoard = value;
                    OnPropertyChanged();
                    updateFiles();
                }
            }
        }
        BoardVM _selectedBoard;


        #endregion

        public void updateFiles()
        {

            model.generateFiles(selectedBoard?.board);
            OnPropertyChanged("makefile");
            OnPropertyChanged("propFile");
            OnPropertyChanged("taskFile");
        }


        public void updateBoards()
        {
            model.parseBoardsTxt();

            foreach (var boardVM in boardVMs)  // remove old event handlers
            {
                foreach (var optionSetVM in boardVM.optionSetVMs)
                {
                    optionSetVM.PropertyChanged -= (s, e) => updateFiles();
                }
            }
            boardVMs.Clear();

            foreach (var board in model.boards)
            {
                var boardVM = new BoardVM(board);
                boardVMs.Add(boardVM);
                foreach (var optionSetVM in boardVM.optionSetVMs)
                {
                    optionSetVM.PropertyChanged += (s, e) => updateFiles();
                }
            }
            selectedBoard = boardVMs.FirstOrDefault();
        }


        public ViewModel()
        {
            if (Debugger.IsAttached)
            {
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            }

            cmdGenerate = new RelayCommand(doGenerate,o=>model.data.projectBaseError == null && !String.IsNullOrWhiteSpace(model.data.makefile) && !String.IsNullOrWhiteSpace(model.data.tasks_json) && !String.IsNullOrWhiteSpace(model.data.propsFile));
            cmdClose = new RelayCommand(doClose);


            quickSetup = true;
            updateBoards();
        }


        public event EventHandler<string> MessageHandler;
        protected void Message(string message)
        {
            MessageHandler?.Invoke(this, message);
        }


        public Model model = new Model();
    }
}


