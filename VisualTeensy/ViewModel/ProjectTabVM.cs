using VisualTeensy.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ViewModel
{
    public class ProjectTabVM : BaseViewModel, IDataErrorInfo
    {
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

        #region Properties ------------------------------------------------------
        public LibManagerVM libManagerVM {get;}


        public String makefile => model.data.makefile;
        public String propFile => model.data.props_json;
        public String taskFile => model.data.tasks_json;
        public String settFile => model.data.vsSetup_json;

        public String projectPath
        {
            get => model.data.projectBase;
            set
            {
                if (value != model.data.projectBase)
                {
                    model.data.projectBase = value.Trim();
                    selectedBoard = null;  //HACK, otherwise updateBoards will implicitely delete selectedBoard set by openProjectPath
                    model.openProjectPath();
                    updateBoards();
                    OnPropertyChanged(""); // update all
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

                    ///Hack
                    var board = selectedBoard?.board;
                    selectedBoard = null;
                    model.selectedBoard = board;

                    updateAll();
                    OnPropertyChanged("");
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
                    updateAll();
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
                    updateAll();
                    OnPropertyChanged("");
                }
            }
        }

        public ObservableCollection<BoardVM> boardVMs { get; } = new ObservableCollection<BoardVM>();

        public BoardVM selectedBoard
        {
            get => boardVMs.FirstOrDefault(b => b.board == model.selectedBoard);
            set
            {
                if (value?.board != model.selectedBoard)
                {
                    model.selectedBoard = value?.board;
                    OnPropertyChanged();
                    updateFiles();
                }
            }
        }

        #endregion

        public void updateFiles()
        {
            model.generateFiles();
            OnPropertyChanged("makefile");
            OnPropertyChanged("propFile");
            OnPropertyChanged("taskFile");
            OnPropertyChanged("settFile");
        }

        void updateAll()
        {
            model.parseBoardsTxt();
            updateBoards();
        }

        void updateBoards()
        {
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

            selectedBoard = boardVMs?.FirstOrDefault(b => b.board == model.selectedBoard) ?? boardVMs?.FirstOrDefault();
        }

        public ProjectTabVM(Model model)
        {
            this.model = model;
            libManagerVM = new LibManagerVM(model);

            cmdGenerate = new RelayCommand(doGenerate, o => model.data.projectBaseError == null && !String.IsNullOrWhiteSpace(model.data.makefile) && !String.IsNullOrWhiteSpace(model.data.tasks_json) && !String.IsNullOrWhiteSpace(model.data.props_json));
            cmdClose = new RelayCommand(doClose);

            updateBoards();
        }


        public event EventHandler<string> MessageHandler;
        protected void Message(string message)
        {
            MessageHandler?.Invoke(this, message);
        }
        public Model model;
    }
}


