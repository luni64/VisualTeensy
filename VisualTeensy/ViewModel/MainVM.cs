using VisualTeensy.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ViewModel
{
    class MainVM : BaseViewModel
    {
        public RelayCommand cmdFileOpen { get; set; }
        void doFileOpen(object path)
        {
            model.project.path = path as string;            
            model.openProjectPath();

            projecTabVM.update();
          

            //projecTabVM.projectPath = path as string;
            //OnPropertyChanged("Title");
        }
        public RelayCommand cmdFileNew { get; set; }
        void doFileNew(object o)
        {
          //  projecTabVM.project
        }

        public SetupTabVM setupTabVM { get; }
        public ProjectTabVM projecTabVM { get; }

        public String Title
        {
            get
            {
                var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return $"{model.project.path} - VisualTeensy v{v.Major}.{v.Minor} (lunOptics)";
            }
        }

        public MainVM(Model model = null)
        {
            cmdFileOpen = new RelayCommand(doFileOpen);
            cmdFileNew = new RelayCommand(doFileNew);

            this.model = model;
            
            projecTabVM = new ProjectTabVM(model);
            setupTabVM = new SetupTabVM(model);
                       
            setupTabVM.PropertyChanged += (s, e) => projecTabVM.updateFiles();            
        }

        public Model model { get; }
    }
}
