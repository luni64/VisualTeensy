using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using vtCore.Interfaces;
using vtCore;


namespace ViewModel
{
    public class ProjectTabVM : BaseViewModel, IDataErrorInfo
    {
        #region IDataErrorInfo ------------------------------------------------

        public string Error => "ERR"; //throw new NotImplementedException();

        public string this[string columnName]
        {
            get
            {
                string error;

                switch (columnName)
                {
                    //case "projectPath":
                    //    error = project.pathError;
                    //    break;

                    case "arduinoBase":
                        error = setup.arduinoBaseError;
                        break;

                    //case "boardTxtPath":
                    //    error = project.selectedConfiguration.boardTxtPathError;
                    //    break;

                    case "corePath":
                        error = project.selectedConfiguration.setupType == SetupTypes.expert ? project.selectedConfiguration.coreBase.error : null;
                        break;

                    case "compilerPath":
                        error = project.selectedConfiguration.compilerBase.error;
                        break;

                    //case "makePath":
                    //    error = project.setup.makeExePathError;
                    //    break;

                    case "boardVMs":
                        error = project.selectedConfiguration.selectedBoard == null ? "No board selected" : null;
                        break;

                    default:
                        error = null;
                        break;
                }
                return error;


            }
        }
        #endregion

        #region Commands ------------------------------------------------------

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
        #endregion

        #region Properties ----------------------------------------------------
        public ObservableCollection<BoardVM> boardVMs { get; } = new ObservableCollection<BoardVM>();
        public BoardVM selectedBoard
        {
            get => boardVMs.FirstOrDefault(b => b.board == project.selectedConfiguration.selectedBoard);
            set
            {
                if (value != null && value.board != project.selectedConfiguration.selectedBoard)  // value != null to avoid deleting model.selectedBoard by chance
                {
                    project.selectedConfiguration.selectedBoard = value.board;
                    OnPropertyChanged();
                    updateFiles();
                }
            }
        }


        public bool isMakefileBuild =>  project.buildSystem == BuildSystem.makefile;
        //    set
        //    {
        //        if (value != project.isMakefileBuild)
        //        {
        //            project.isMakefileBuild = value;
        //            OnPropertyChanged();
        //        }
        //    }
        //}

        public bool useInoFiles
        {
            get => project.useInoFiles;
            set
            {
                project.useInoFiles = value;
                updateAll();
                OnPropertyChanged("");
            }
        }

        public bool quickSetup
        {
            get => project.selectedConfiguration.setupType == SetupTypes.quick ? true : false;
            set
            {
                SetupTypes newType = value == true ? SetupTypes.quick : SetupTypes.expert;  // hack, valueConverter would be better
                if (project.selectedConfiguration.setupType != newType)
                {
                    project.selectedConfiguration.setupType = newType;
                    updateAll();
                    OnPropertyChanged("");
                }
            }
        }

        public bool hasDebugSupport
        {
            get => project.debugSupport != DebugSupport.none;
            set
            {
                project.debugSupport = value ? DebugSupport.cortex_debug : DebugSupport.none;
                updateFiles();
                OnPropertyChanged("");
            }
        }

        public string makefileExtension
        {
            get => project.selectedConfiguration.makefileExtension;
            set
            {
                if (value != project.selectedConfiguration.makefileExtension)
                {
                    project.selectedConfiguration.makefileExtension = value;
                    OnPropertyChanged();
                    updateFiles();
                }
            }
        }

        public String makefile
        {
            get => _makefile;
            set => SetProperty(ref _makefile, value);
        }
        string _makefile;

        public String taskFile
        {
            get => _taskFile;
            set => SetProperty(ref _taskFile, value);
        }
        string _taskFile;

        public String debugFile
        {
            get => _debugFile;
            set => SetProperty(ref _debugFile, value);
        }
        string _debugFile;

        public String propFile
        {
            get => _propFile;
            set => SetProperty(ref _propFile, value);
        }
        string _propFile;

        public String settFile
        {
            get => _settFile;
            set => SetProperty(ref _settFile, value);
        }
        string _settFile;

        public String arduinoBase
        {
            get => setup.arduinoBase;
            set
            {
                if (value != setup.arduinoBase)
                {
                    setup.arduinoBase = value.Trim();
                    Helpers.arduinoPath = setup.arduinoBase;

                    ///Hack
                    var board = selectedBoard?.board;
                    selectedBoard = null;
                    project.selectedConfiguration.selectedBoard = board;

                    updateAll();
                    OnPropertyChanged("");
                }
            }
        }
        //public String boardTxtPath
        //{
        //    get => project.selectedConfiguration.boardTxtPath;
        //    set
        //    {
        //        if (value != project.selectedConfiguration.boardTxtPath)
        //        {
        //            project.selectedConfiguration.boardTxtPath = value.Trim();
        //            updateAll();
        //            OnPropertyChanged("");
        //        }
        //    }
        //}
        //public bool copyBoardTxt
        //{
        //    get => project.selectedConfiguration.copyBoardTxt;
        //    set
        //    {
        //        if (value != project.selectedConfiguration.copyBoardTxt)
        //        {
        //            project.selectedConfiguration.copyBoardTxt = value;
        //            OnPropertyChanged();
        //            updateFiles();
        //        }
        //    }
        //}

        public bool copyCore
        {
            get => project.selectedConfiguration.copyCore;
            set
            {
                if (value != project.selectedConfiguration.copyCore)
                {
                    project.selectedConfiguration.copyCore = value;
                    OnPropertyChanged();
                    updateFiles();
                }
            }
        }
        public String corePath
        {
            get => project.selectedConfiguration.coreBase.path;
            set
            {
                if (value != project.selectedConfiguration.coreBase.path)
                {
                    project.selectedConfiguration.coreBase.path = value.Trim();
                    updateAll();
                    OnPropertyChanged("");
                }
            }
        }
        public String compilerPath
        {
            get => project.selectedConfiguration.compilerBase.path;
            set
            {
                if (value != project.selectedConfiguration.compilerBase.path)
                {
                    project.selectedConfiguration.compilerBase.path = value.Trim();
                    OnPropertyChanged();
                    updateFiles();
                }
            }
        }

        #endregion

        private void onOptionSetChanged(object sender, PropertyChangedEventArgs e)
        {
            updateFiles();
        }

        public void update()
        {
            updateBoards();
            selectedBoard = boardVMs?.FirstOrDefault(b => b.board == project.selectedConfiguration.selectedBoard) ?? boardVMs?.FirstOrDefault();
            OnPropertyChanged("");
        }

        public ProjectTabVM(IProject project, LibManager libManager, SetupData setup)
        {
            this.project = project;
            this.libManager = libManager;
            this.setup = setup;

            makefile = Makefile.generate(project, libManager, setup);
            taskFile = TaskFile.generate(project, setup);
            propFile = IntellisenseFile.generate(project, libManager, setup);
            settFile = ProjectSettings.generate(project);
            debugFile = DebugFile.generate(project, setup);

            cmdGenerate = new RelayCommand(doGenerate);//, o => project.pathError == null && !String.IsNullOrWhiteSpace(project.selectedConfiguration.makefile) && !String.IsNullOrWhiteSpace(project.tasks_json) && !String.IsNullOrWhiteSpace(project.props_json));
            cmdClose = new RelayCommand(doClose);

            updateBoards();
        }

        internal void updateAll()
        {
            project.selectedConfiguration.parseBoardsTxt(null); //ERRORR!!!!! fix 
            updateBoards();
            updateFiles();
        }
        internal void updateFiles()
        {
           

            makefile = Makefile.generate(project, libManager, setup);
            taskFile = TaskFile.generate(project, setup);
            propFile = IntellisenseFile.generate(project, libManager, setup);
            settFile = ProjectSettings.generate(project);
            debugFile = DebugFile.generate(project, setup);

            //OnPropertyChanged("makefile");
            // OnPropertyChanged("propFile");
            //  OnPropertyChanged("taskFile");
            OnPropertyChanged("settFile");
        }
        private void updateBoards()
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

            foreach (var board in project.selectedConfiguration.boards)
            {
                var boardVM = new BoardVM(board);
                boardVMs.Add(boardVM);
                foreach (var optionSetVM in boardVM.optionSetVMs)
                {
                    optionSetVM.PropertyChanged += onOptionSetChanged;  // can't use a simple lambda here since we have no chance to remove it :-(
                }
            }
            selectedBoard = boardVMs?.FirstOrDefault(b => b.board == project.selectedConfiguration.selectedBoard) ?? boardVMs?.FirstOrDefault();
        }

        public event EventHandler<string> MessageHandler;
        protected void Message(string message)
        {
            MessageHandler?.Invoke(this, message);
        }
        public IProject project;
        LibManager libManager;
        SetupData setup;
    }
}


