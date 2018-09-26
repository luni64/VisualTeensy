using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualTeensy.Model;

namespace ViewModel
{
    public class LibManagerVM : BaseViewModel
    {
        public ObservableCollection<Library> libs { get;  }


        public LibManagerVM(Model model)
        {
            this.model = model;

            libs = new ObservableCollection<Library>();
            foreach(var lib in model.libManager.libraries)
            {
                libs.Add(lib);
            }
        }


        Model model;
    }
}
