using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace VisualTeensy
{
    /// <summary>
    /// Interaction logic for SetupTab.xaml
    /// </summary>
    public partial class SetupTab : UserControl
    {
        public SetupTab()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Hyperlink_RequestNavigate_1(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void Rectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var x = sender as System.Windows.Shapes.Rectangle;
            var color = (System.Drawing.Color)x.Tag;

            var colorDialog = new System.Windows.Forms.ColorDialog()
            {
                SolidColorOnly = true,
                FullOpen = true,
                Color = color,
            };
            colorDialog.ShowDialog();

            var c = colorDialog.Color;
            x.Fill = new SolidColorBrush(Color.FromArgb(c.A, c.R, c.G, c.B));
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var dlg = new System.Windows.Forms.OpenFileDialog();
            if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var vm = DataContext as ViewModel.SetupTabVM;
                // vm.additionalFiles.Add(dlg.FileName);

                vm.cmdAddFile.Execute(dlg.FileName);

            }
        }
    }
}
