using System.Diagnostics;
using System.Windows.Controls;
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
    }
}
