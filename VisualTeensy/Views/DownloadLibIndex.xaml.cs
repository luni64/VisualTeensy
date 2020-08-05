using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ViewModel;

namespace VisualTeensy
{
    /// <summary>
    /// Interaction logic for SaveProjectWin.xaml
    /// </summary>
    public partial class DownloadLibIndexWin : Window
    {
        public DownloadLibIndexWin(SaveWinVM vm)
        {
            InitializeComponent();
            this.DataContext = vm;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
               

        //private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var listView = sender as ListView;

        //    if (listView != null)
        //    {
        //        var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(listView, 0);
        //     //   var scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
        //        scrollViewer.ScrollToBottom();
        //    }
        //}

        //private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var lb = (ListBox)sender;
        //    lb.ScrollIntoView(lb.SelectedItem);
        //}
    }
}
