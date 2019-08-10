using System.Threading.Tasks;
using vtCore.Interfaces;

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

        readonly ITask task;
    }
}
