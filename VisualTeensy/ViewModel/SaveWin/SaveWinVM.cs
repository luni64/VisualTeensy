using vtCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Data;

namespace ViewModel
{
    public class SaveWinVM : BaseViewModel
    {
      public ListCollectionView taskVMs { get; }

      //  public IReadOnlyList<TaskVM> taskVMs { get; }
        public AsyncCommand cmdSave { get; private set; }

        async Task doSave()
        {
            taskVMs.MoveCurrentToFirst();

            while (!taskVMs.IsCurrentAfterLast)
            {
                await ((TaskVM)taskVMs.CurrentItem).action();
                await Task.Delay(250);
                taskVMs.MoveCurrentToNext();
                OnPropertyChanged("tasks");
            }

            //foreach (TaskVM task in taskVMs)
            //{
            //    await task.action();

            //    OnPropertyChanged("tasks");
            //}
        }
        
        public SaveWinVM(IProject project, LibManager libManager, SetupData setup)
        {
            cmdSave = new AsyncCommand(doSave);            
            //taskVMs = CodeGenerator.getTasks(project, libManager, setup).Select(t => new TaskVM(t)).ToList();

            var tl = CodeGenerator.getTasks(project, libManager, setup).Select(t => new TaskVM(t)).ToList();

            taskVMs = new ListCollectionView(tl);
        }
    }
}