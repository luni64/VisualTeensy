using System.Windows;
using ViewModel;

namespace VisualTeensy
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
