using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewModel;
using VisualTeensy.Model;

namespace VisualTeensy
{
    /// <summary>
    /// Interaction logic for LibInfoWin.xaml
    /// </summary>
    public partial class LibInfoWin : Window
    {
        public LibInfoWin(LibraryVM vm)
        {
            InitializeComponent();

            DataContext = vm;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //if (Directory.Exists(e.Uri.LocalPath))
            //{
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
            //}
            //else MessageBox.Show($"Path {e.Uri.LocalPath} does not exist", "VisualTeensy", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
