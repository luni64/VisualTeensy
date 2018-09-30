using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using ViewModel;
using System.IO;

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
                    dialog.InitialDirectory = System.IO.Path.GetDirectoryName(vm.model.project.path);
                    dialog.DefaultFileName = System.IO.Path.GetFileName(vm.model.project.path);
                }
                catch { }

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    vm.cmdFileOpen.Execute(dialog.FileName);
                }
            }

        }
    }
}
