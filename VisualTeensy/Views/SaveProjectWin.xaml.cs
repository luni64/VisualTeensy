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

namespace Board2Make
{
    /// <summary>
    /// Interaction logic for SaveProjectWin.xaml
    /// </summary>
    public partial class SaveProjectWin : Window
    {
        public SaveProjectWin(SaveWinVM vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
