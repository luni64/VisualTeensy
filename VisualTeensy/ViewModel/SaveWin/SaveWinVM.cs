using vtCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Data;
using log4net;
using System.Reflection;

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
                var task = (TaskVM)taskVMs.CurrentItem;
                log.Info(task.title);

                await task.action();
                await Task.Delay(250);
                log.Info($"{task.title} done");

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



        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}