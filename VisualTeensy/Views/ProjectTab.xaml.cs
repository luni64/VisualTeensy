using VisualTeensy;
using VisualTeensy.Model;
using System.Windows;
using System.Windows.Controls;
using ViewModel;
using System.Linq;
using System.Windows.Navigation;
using System.Diagnostics;

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
            var mvm = DataContext as ProjectTabVM;            
            var dlg = new SaveProjectWin(new SaveWinVM(mvm.model));

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


        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

    }
}
