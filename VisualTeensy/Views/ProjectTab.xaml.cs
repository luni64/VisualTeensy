using VisualTeensy;
using VisualTeensy.Model;
using System.Windows;
using System.Windows.Controls;
using ViewModel;
using System.Linq;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Windows.Data;

namespace VisualTeensy
{
    /// <summary>
    /// Interaction logic for ProjectTab.xaml
    /// </summary>
    public partial class ProjectTab : UserControl
    {
        public ProjectTab()
        {
            InitializeComponent();

            Loaded += (s, e) => (DataContext as ProjectTabVM)?.OnPropertyChanged("");
        }      

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var fdm = Application.Current.Windows.OfType<FileDisplayWindow>().FirstOrDefault();
            if (fdm == null)
            {
                new FileDisplayWindow() { DataContext = this.DataContext }.Show();
            }
            else
            {
                fdm.Close();
            }
        }

        private void StackPanel_Checked(object sender, RoutedEventArgs e)
        {
            if (DataContext == null) return;

            var path = (string)(e.OriginalSource as RadioButton)?.Tag;

            file.SetBinding(TextBox.TextProperty, new Binding(path)
            {
                Source = DataContext,
                Mode = BindingMode.OneWay
            });
        }


        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

    }
}
