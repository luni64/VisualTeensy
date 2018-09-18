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
        public SetupTabVM setupTabVM { get; }


        public RelayCommand cmdGenerate { get; private set; }
        void doGenerate(object obj)
        {   
            Message("Generate");
        }

        public RelayCommand cmdClose { get; private set; }
        void doClose(object o)
        {
            model.saveSettings();
        }


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

                    case "arduinoBase":
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
        public String propFile => model.data.props_json;
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

                    var t = model.readProjectConfig();

                    if(t != null)
                    {
                        var board = boardVMs.FirstOrDefault(b => b.boardName == t.board.name);
                        if (board != null)
                        {
                            foreach (var option in t.board.options)
                            {
                                var o = board.optionSetVMs.FirstOrDefault(os => os.name == option.Key);
                                if (o != null)
                                {
                                    o.selectedOption = o.options.FirstOrDefault(x => x.name == option.Value);
                                }
                            }
                            selectedBoard = board;
                        }
                    }



                    OnPropertyChanged();
                    OnPropertyChanged("makeFileName");
                    OnPropertyChanged("propFileName");
                    OnPropertyChanged("taskFileName");
                    updateFiles();
                }
            }
        }

        public string projectName
        {
            get => model.data.projectName;
            set
            {
                if (String.IsNullOrWhiteSpace(value))
                {
                    value = "newProject";
                }
                if (value != model.data.projectName)
                {
                    model.data.projectName = value.Trim().Replace(" ", "_");

                    OnPropertyChanged();
                    updateFiles();
                }
            }
        }

        public String arduinoBase
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

        public String Title
        {
            get
            {
                var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return $"lunOptics - VisualTeensy v{v.Major}.{v.Minor}";
            }
        }


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
            setupTabVM = new SetupTabVM(model.data);

            setupTabVM.PropertyChanged += (s, e) => updateFiles();

            if (Debugger.IsAttached)
            {
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            }

            cmdGenerate = new RelayCommand(doGenerate, o => model.data.projectBaseError == null && !String.IsNullOrWhiteSpace(model.data.makefile) && !String.IsNullOrWhiteSpace(model.data.tasks_json) && !String.IsNullOrWhiteSpace(model.data.props_json));
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


