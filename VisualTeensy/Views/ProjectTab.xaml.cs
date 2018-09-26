using VisualTeensy;
using VisualTeensy.Model;
using System.Windows;
using System.Windows.Controls;
using ViewModel;
using System.Linq;

namespace VisualTeensy
{
    /// <summary>
    /// Interaction logic for ProjectTab.xaml
    /// </summary>
    public partial class ProjectTab : UserControl
    {
        public ProjectTab()
        {
            InitializeComponent();
        }

        private void handleMessages(object sender, string message)
        {
            switch (message)
            {
                case "Generate":
                    openOutput();
                    break;
            }
        }


        private void openOutput()
        {
            var mvm = DataContext as ViewModel.ProjectTabVM;

            SetupData data = mvm.model.data;

            var vm = new SaveWinVM(data);

            var dlg = new SaveProjectWin(vm);
            dlg.ShowDialog();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ViewModel.ProjectTabVM;
            if (vm != null) vm.MessageHandler += handleMessages;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as ViewModel.ProjectTabVM;
            if (vm != null) vm.MessageHandler -= handleMessages;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var fdm = Application.Current.Windows.OfType<FileDisplayWindow>().FirstOrDefault();
            if (fdm == null)
            {
                new FileDisplayWindow() { DataContext = this.DataContext }.Show();
            }
            else
            {
                fdm.Close();
            }
        }
    }
}
