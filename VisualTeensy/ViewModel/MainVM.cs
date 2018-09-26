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
        public SetupTabVM setupTabVM { get; }
        public ProjectTabVM projecTabVM { get; }

        public String Title
        {
            get
            {
                var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return $"lunOptics - VisualTeensy v{v.Major}.{v.Minor}";
            }
        }

        public MainVM(Model model = null)
        {
            if (Debugger.IsAttached)
            {
                CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            }

            
            projecTabVM = new ProjectTabVM(model);
            setupTabVM = new SetupTabVM(model);
                       
            setupTabVM.PropertyChanged += (s, e) => projecTabVM.updateFiles();            
        }
    }
}
