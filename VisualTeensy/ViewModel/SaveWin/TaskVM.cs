using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vtCore;

namespace ViewModel
{
    public class TaskVM : BaseViewModel
    {
        public string title => task.title;
        public string description => task.description;
        public string status => task.status;
        

        public async Task action()
        {
            await task.action();            
            OnPropertyChanged("status");
        }

        public TaskVM(ITask task)
        {
            this.task = task;
        }
               

        ITask task;
    }
}
