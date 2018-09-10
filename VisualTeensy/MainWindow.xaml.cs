using Board2Make.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using ViewModel;

namespace Board2Make
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            var vm = DataContext as ViewModel.ViewModel;
            vm.MessageHandler += HandleMessages;
        }

        private void HandleMessages(object sender, string message)
        {
           switch (message)
            {
                case "Generate":
                    OpenOutput();
                    break;
            }
        }


        private void OpenOutput()
        {
           
            var mvm = DataContext as ViewModel.ViewModel;

            SetupData data = mvm.model.data;
          
            var vm = new SaveWinVM(data);

            var dlg = new SaveProjectWin(vm);
            dlg.ShowDialog();

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var mvm = DataContext as ViewModel.ViewModel;
            mvm.cmdClose.Execute(null);
        }
    }
}
