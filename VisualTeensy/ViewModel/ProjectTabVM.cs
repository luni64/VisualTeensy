using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using VisualTeensy.Model;

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
                        error = model.project.pathError;
                        break;

                    case "arduinoBase":
                        error = model.setup.arduinoBaseError;
                        break;

                    case "boardTxtPath":
                        error = model.project.boardTxtPathError;
                        break;

                    case "corePath":
                        error = model.project.setupType == SetupTypes.expert ? model.project.corePathError : null;
                        break;

                    case "compilerPath":
                        error = model.project.compilerPathError;
                        break;

                    case "makePath":
                        error = model.setup.makeExePathError;
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
            //repositoryVM.update();

            Message("Generate");
        }

        public RelayCommand cmdClose { get; private set; }
        void doClose(object o)
        {
            // model.saveSettings();
        }

        #region Properties ------------------------------------------------------
        public RepositoryVM repositoryVM { get; }


        public String makefile => model.makefile;
        public String propFile => model.props_json;
        public String taskFile => model.tasks_json;
        public String settFile => model.vsSetup_json;

        public void update()
        {
            updateBoards();
            selectedBoard = boardVMs?.FirstOrDefault(b => b.board == model.project.selectedBoard) ?? boardVMs?.FirstOrDefault();
            OnPropertyChanged("");
        }


        public String projectPath
        {
            get => model.project.path;
            set
            {
                if (value != model.project.path)
                {
                    model.project.path = value.Trim();
                    selectedBoard = null;  //HACK, otherwise updateBoards will implicitely delete selectedBoard set by openProjectPath
                    //model.openProjectPath();
                    updateBoards();
                    OnPropertyChanged(""); // update all
                }
            }
        }

        public String projectDescription => Path.GetFileName(projectPath);

        public String arduinoBase
        {
            get => model.setup.arduinoBase;
            set
            {
                if (value != model.setup.arduinoBase)
                {
                    model.setup.arduinoBase = value.Trim();
                    Helpers.arduinoPath = model.setup.arduinoBase;

                    ///Hack
                    var board = selectedBoard?.board;
                    selectedBoard = null;
                    model.project.selectedBoard = board;

                    updateAll();
                    OnPropertyChanged("");
                }
            }
        }
        public String boardTxtPath
        {
            get => model.project.boardTxtPath;
            set
            {
                if (value != model.project.boardTxtPath)
                {
                    model.project.boardTxtPath = value.Trim();
                    updateAll();
                    OnPropertyChanged("");
                }
            }
        }
        public bool copyBoardTxt
        {
            get => model.project.copyBoardTxt;
            set
            {
                if (value != model.project.copyBoardTxt)
                {
                    model.project.copyBoardTxt = value;
                    OnPropertyChanged();
                    updateFiles();
                }
            }
        }

        public bool copyCore
        {
            get => model.project.copyCore;
            set
            {
                if (value != model.project.copyCore)
                {
                    model.project.copyCore = value;
                    OnPropertyChanged();
                    updateFiles();
                }
            }
        }

        public String corePath
        {
            get => model.project.coreBase;
            set
            {
                if (value != model.project.coreBase)
                {
                    model.project.coreBase = value.Trim();
                    OnPropertyChanged();
                    updateFiles();
                }
            }
        }
        public String compilerPath
        {
            get => model.project.compilerBase;
            set
            {
                if (value != model.project.compilerBase)
                {
                    model.project.compilerBase = value.Trim();
                    OnPropertyChanged();
                    updateFiles();
                }
            }
        }


        public bool quickSetup
        {
            get => model.project.setupType == SetupTypes.quick ? true : false;
            set
            {
                SetupTypes newType = value == true ? SetupTypes.quick : SetupTypes.expert;  // hack, valueConverter would be better
                if (model.project.setupType != newType)
                {
                    model.project.setupType = newType;
                    updateAll();
                    OnPropertyChanged("");
                }
            }
        }

        public ObservableCollection<BoardVM> boardVMs { get; } = new ObservableCollection<BoardVM>();

        public BoardVM selectedBoard
        {
            get => boardVMs.FirstOrDefault(b => b.board == model.project.selectedBoard);
            set
            {
                if (value != null && value.board != model.project.selectedBoard)  // value != null to avoid deleting model.selectedBoard by chance
                {
                    model.project.selectedBoard = value.board;
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
            model.project.parseBoardsTxt();
            updateBoards();
            updateFiles();
        }
                

        void updateBoards()
        {
            foreach (var boardVM in boardVMs)  // remove old event handlers
            {
                foreach (var optionSetVM in boardVM.optionSetVMs)
                {
                    optionSetVM.PropertyChanged -= onOptionSetChanged;
                    optionSetVM.selectedOption = null;
                }
            }

            boardVMs.Clear();

            foreach (var board in model.project.boards)
            {
                var boardVM = new BoardVM(board);
                boardVMs.Add(boardVM);
                foreach (var optionSetVM in boardVM.optionSetVMs)
                {
                    optionSetVM.PropertyChanged += onOptionSetChanged;  // can't use a simple lambda here since we have no chance to remove it :-(
                }
            } 
            selectedBoard = boardVMs?.FirstOrDefault(b => b.board == model.project.selectedBoard) ?? boardVMs?.FirstOrDefault();
        }

        private void onOptionSetChanged(object sender, PropertyChangedEventArgs e)
        {
            updateFiles();
        }

        public ProjectTabVM(Model model)
        {
            this.model = model;
            repositoryVM = new RepositoryVM(model.sharedLibraries);
            repositoryVM.PropertyChanged += RepositoryVM_PropertyChanged;

            cmdGenerate = new RelayCommand(doGenerate, o => model.project.pathError == null && !String.IsNullOrWhiteSpace(model.makefile) && !String.IsNullOrWhiteSpace(model.tasks_json) && !String.IsNullOrWhiteSpace(model.props_json));
            cmdClose = new RelayCommand(doClose);

            updateBoards();
        }

        public ProjectTabVM()
        {
            var sud = new SetupData();

            model = new Model(new ProjectData(sud), sud);

            model.project.setupType = SetupTypes.quick;
            model.project.path = @"c:\users\lutz\source";

            updateBoards();


        }

        private void RepositoryVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "libraries")
            {
                updateFiles();
            }
        }

        public event EventHandler<string> MessageHandler;
        protected void Message(string message)
        {
            MessageHandler?.Invoke(this, message);
        }
        public Model model;
    }
}


