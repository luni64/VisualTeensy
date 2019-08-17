using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ViewModel;

namespace VisualTeensy
{
    /// <summary>
    /// Interaction logic for SaveProjectWin.xaml
    /// </summary>
    /// 

        

    public partial class StartupSettingsView : Window
    {
        public StartupSettingsView(StartupSettingsVM vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }

        //private void CloseClick(object sender, RoutedEventArgs e)
        //{
        //    this.Close();
        //}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{

        //}
    }
}
