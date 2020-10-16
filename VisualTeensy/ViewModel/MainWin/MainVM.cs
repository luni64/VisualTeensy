using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using vtCore;
using vtCore.Interfaces;

namespace ViewModel
{
    public class MruItemVM
    {
        internal MruItemVM(string projectFolder, MainVM parent)
        {
            this.projectFolder = projectFolder;
            this.projectName = Path.GetFileName(projectFolder);
            this.parent = parent;
        }

        public string projectFolder { get; }
        public string projectName { get; }
        public MainVM parent { get; }
    }

    public class MainVM : BaseViewModel
    {
        public SetupTabVM setupTabVM { get; }
        public ProjectTabVM projecTabVM { get; }
        public LibrariesTabVM librariesTabVM { get; }

        public RelayCommand cmdFileOpen { get; set; }

        public ObservableCollection<MruItemVM> mruList { get; } = new ObservableCollection<MruItemVM>();

        void doFileOpen(object path)
        {
            string prj = path as string;
            if (prj != null)
            {
                project.openProject(prj);
                setup.mru.AddProject(prj);
                mruList.Clear();
                setup.mru.projects.ForEach(p => mruList.Add(new MruItemVM(p, this)));
                
                projecTabVM.update();
                librariesTabVM.update();
                ActionText = Directory.Exists(projectPath) ? "Update Project" : "Generate Project";
                OnPropertyChanged("");
            }
        }

        public RelayCommand cmdFileNew { get; set; }
        void doFileNew(object o)
        {
            project.newProject();
            projecTabVM.update();
            librariesTabVM.update();
            ActionText = "Generate Project";
            OnPropertyChanged("");
        }

        public RelayCommand cmdClose { get; set; }
        void doClose(object o)
        {
            Application.Current.Shutdown();
        }

        public string ActionText { get; private set; }

        public string projectName => project.name;
        public string projectPath => project.path;


        public string Title
        {
            get
            {
                var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return $"VisualTeensy V{v.Major}.{v.Minor}.{v.Build}.{v.Revision} - lunOptics";
            }
        }

        public MainVM(IProject project, LibManager libManager, SetupData setup)
        {
            this.project = project;
            this.libManager = libManager;
            this.setup = setup;

            projecTabVM = new ProjectTabVM(project, libManager, setup);
            setupTabVM = new SetupTabVM(project, setup);
            librariesTabVM = new LibrariesTabVM(project, libManager, setup);

            cmdFileOpen = new RelayCommand(doFileOpen);
            cmdFileNew = new RelayCommand(doFileNew);
            cmdClose = new RelayCommand(doClose);

            setupTabVM.PropertyChanged += (s, e) =>
            {
                projecTabVM.updateAll();
                projecTabVM.OnPropertyChanged("");
            };

            mruList = new ObservableCollection<MruItemVM>();
            foreach (var prj in setup.mru.projects)
            {
                mruList.Add(new MruItemVM(prj, this));
            }
            //var mruList = setup.mru.projects?.Select(p => new MruItemVM(p, this));
            //if(mruList != null)  mruList = new ObservableCollection<MruItemVM>(mruList);
          }

        public LibManager libManager { get; }
        public SetupData setup { get; }
        public IProject project { get; }
    }
}
