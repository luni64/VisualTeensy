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

           
        }

        

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var mvm = DataContext as ViewModel.MainVM;
            mvm.projecTabVM.cmdClose.Execute(null);
        }
    }
}
