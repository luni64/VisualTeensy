using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using ViewModel;

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

            //Loaded += (s, e) =>
            //{
            //    var dc = DataContext as ProjectTabVM;
            //    if (dc != null)
            //    {
            //       // dc.update();
            //        //dc.OnPropertyChanged("");
            //    }
            //};
        }

        //private void openOutputClick(object sender, RoutedEventArgs e)
        //{
        //    var vm = DataContext as ProjectTabVM;
        //    var dlg = new SaveProjectWin(new SaveWinVM(vm.project, vm.lib));

        //    dlg.ShowDialog();
        //}

        private void StackPanel_Checked(object sender, RoutedEventArgs e)
        {
            if (DataContext == null) return;

            var path = (string)(e.OriginalSource as RadioButton)?.Tag;

            file.SetBinding(TextBox.TextProperty, new Binding(path)
            {
                Source = DataContext,
                Mode = BindingMode.OneWay
            });
        }


        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

    }
}
