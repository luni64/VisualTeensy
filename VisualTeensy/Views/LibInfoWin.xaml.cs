using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using ViewModel;

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
