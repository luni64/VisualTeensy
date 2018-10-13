using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ViewModel;
using VisualTeensy.Model;

namespace VisualTeensy
{
    /// <summary>
    /// Interaction logic for LibrariesTab.xaml
    /// </summary>
    public partial class LibrariesTab : UserControl
    {
        public LibrariesTab()
        {
            InitializeComponent();
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

        private void removeButtonClick(object sender, RoutedEventArgs e)
        {
            var button = ((Button)sender).DataContext;

            var dc = DataContext as LibrariesTabVM;

            dc.cmdDel.Execute(button);
        }

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var library = ((TextBlock)sender).DataContext as Library;
            new LibInfoWin(new LibraryVM(Enumerable.Repeat(library, 1))).ShowDialog();
        }

        private void TextBlock_MouseDown2(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var versionedLibraryVM = ((TextBlock)sender).DataContext as LibraryVM;
            new LibInfoWin(versionedLibraryVM).ShowDialog();
        }
    }
}
