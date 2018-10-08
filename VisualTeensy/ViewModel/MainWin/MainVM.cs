using VisualTeensy.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            model.openFile(path as string);
            projecTabVM.update();
            ActionText = Directory.Exists(projectPath) ? "Update Project" : "Generate Project";
            OnPropertyChanged("");            
        }
        public RelayCommand cmdFileNew { get; set; }
        void doFileNew(object o)
        {
            model.newFile();
            projecTabVM.update();
            ActionText = "Generate Project";
            OnPropertyChanged("");           
        }

        public RelayCommand cmdClose { get; set; }
        void doClose(object o)
        {
            Application.Current.Shutdown();
        }

        public String ActionText { get; private set; }

        public String projectName => model.project.name;
        public String projectPath => model.project.path;


        public String Title
        {
            get
            {
                var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return $"VisualTeensy V{v.Major}.{v.Minor} - lunOptics";
            }
        }

        public MainVM(Model model = null)
        {
            this.model = model;

            projecTabVM = new ProjectTabVM(model);
            setupTabVM = new SetupTabVM(model);
            librariesTabVM = new LibrariesTabVM(model);

            cmdFileOpen = new RelayCommand(doFileOpen);
            cmdFileNew = new RelayCommand(doFileNew);
            cmdClose = new RelayCommand(doClose);                       
                       
            //setupTabVM.PropertyChanged += (s, e) => projecTabVM.updateFiles();            
        }

        public Model model { get; }
    }
}
