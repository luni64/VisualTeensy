using vtCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ViewModel
{
    public class SaveWinVM : BaseViewModel
    {
        public IReadOnlyList<TaskVM> taskVMs { get; }
        public AsyncCommand cmdSave { get; private set; }

        async Task doSave()
        {
            foreach (var task in taskVMs)
            {
                await task.action();
                OnPropertyChanged("tasks");
            }
        }
        
        public SaveWinVM(IProject project, LibManager libManager, SetupData setup)
        {
            cmdSave = new AsyncCommand(doSave);            
            taskVMs = CodeGenerator.getTasks(project, libManager, setup).Select(t => new TaskVM(t)).ToList();
        }
    }
}