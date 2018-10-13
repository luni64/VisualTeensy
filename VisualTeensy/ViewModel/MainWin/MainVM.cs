using VisualTeensy.Model;
using System.IO;
using System.Windows;

namespace ViewModel
{
    class MainVM : BaseViewModel
    {
        public SetupTabVM setupTabVM { get; }
        public ProjectTabVM projecTabVM { get; }
        public LibrariesTabVM librariesTabVM { get; }

        public RelayCommand cmdFileOpen { get; set; }
        void doFileOpen(object path)
        {            
            project.openProject(path as string);
            projecTabVM.update();
            librariesTabVM.update();
            ActionText = Directory.Exists(projectPath) ? "Update Project" : "Generate Project";
            OnPropertyChanged("");            
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
                return $"VisualTeensy V{v.Major}.{v.Minor}.{v.Build} - lunOptics";
            }
        }

        public MainVM(Project project = null)
        {
            this.project = project;

            projecTabVM = new ProjectTabVM(project);
            setupTabVM = new SetupTabVM(project);
            librariesTabVM = new LibrariesTabVM(project);

            cmdFileOpen = new RelayCommand(doFileOpen);
            cmdFileNew = new RelayCommand(doFileNew);
            cmdClose = new RelayCommand(doClose);                       
                       
            //setupTabVM.PropertyChanged += (s, e) => projecTabVM.updateFiles();            
        }

        public Project project { get; }
    }
}
