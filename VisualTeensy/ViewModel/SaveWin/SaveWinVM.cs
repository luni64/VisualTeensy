using log4net;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Data;
using vtCore;
using vtCore.Interfaces;

namespace ViewModel
{
    public class SaveWinVM : BaseViewModel
    {
        public AsyncCommand cmdSave { get; private set; }
        public ListCollectionView taskVMs { get; }

        public bool isReady { get; private set; } = true;
        
        async Task doSave()
        {
            isReady = false;
            OnPropertyChanged("isReady");

            taskVMs.MoveCurrentToFirst();

            while (!taskVMs.IsCurrentAfterLast)
            {
                var taskVM = (TaskVM)taskVMs.CurrentItem;
                log.Info(taskVM.title);

                await taskVM.action();
                await Task.Delay(50);
                log.Info($"{taskVM.title} done");

                taskVMs.MoveCurrentToNext();
                OnPropertyChanged("tasks");
            }
            //await Task.Delay(5000);
            //System.Windows.Application.Current.Shutdown();
        }

        public SaveWinVM(IProject project, LibManager libManager, SetupData setup)
        {
            cmdSave = new AsyncCommand(doSave);          

            var taskList = CodeGenerator.getTasks(project, libManager, setup).Select(t => new TaskVM(t)).ToList();
            taskVMs = new ListCollectionView(taskList);
            this.project = project;
        }

        private readonly IProject project;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

    }
}