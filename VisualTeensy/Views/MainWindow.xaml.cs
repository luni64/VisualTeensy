using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using ViewModel;
using System.IO;
using System.Diagnostics;
using System.Windows.Navigation;

namespace VisualTeensy
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var mvm = DataContext as MainVM;
            mvm?.projecTabVM?.cmdClose.Execute(null);
        }

        private void openOutputClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainVM;
            var dlg = new SaveProjectWin(new SaveWinVM(vm.project));

            dlg.ShowDialog();
        }

        private void saveAs(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainVM;

            using (var dialog = new CommonSaveFileDialog())
            {                
                try
                {
                    dialog.InitialDirectory = System.IO.Path.GetDirectoryName(vm.project.path);
                    dialog.DefaultFileName = System.IO.Path.GetFileName(vm.project.path);                    
                }
                catch { }

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    Directory.CreateDirectory(dialog.FileName);
                    vm.project.path = dialog.FileName;
                    vm.project.generateFiles();
                    var dlg = new SaveProjectWin(new SaveWinVM(vm.project));
                    dlg.ShowDialog();
                }
            }
        }

        private void FileOpenClick(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as MainVM;

            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.AllowNonFileSystemItems = false;
                dialog.AddToMostRecentlyUsedList = true;
                try
                {
                    dialog.InitialDirectory = System.IO.Path.GetDirectoryName(vm.project.path);
                    dialog.DefaultFileName = System.IO.Path.GetFileName(vm.project.path);
                }
                catch { }

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    vm.cmdFileOpen.Execute(dialog.FileName);
                }
            }

        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            if (Directory.Exists(e.Uri.LocalPath))
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
            else MessageBox.Show($"Path {e.Uri.LocalPath} does not exist", "VisualTeensy", MessageBoxButton.OK, MessageBoxImage.Error);
        }

       
    }
}
