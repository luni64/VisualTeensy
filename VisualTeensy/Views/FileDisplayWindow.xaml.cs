using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace VisualTeensy
{
    /// <summary>
    /// Interaction logic for FileDisplayWindow.xaml
    /// </summary>
    public partial class FileDisplayWindow : Window
    {
        public FileDisplayWindow()
        {
            InitializeComponent();

            Rect bounds = Properties.Settings.Default.fileWinBounds;
            this.Left = bounds.Left;
            this.Top = bounds.Top;
            this.Width = bounds.Width;
            this.Height = bounds.Height;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Rect bounds = new Rect(this.Left, this.Top, this.Width, this.Height);
            Properties.Settings.Default.fileWinBounds = bounds;
            Properties.Settings.Default.Save();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            rbMakefile.IsChecked = true;
        }

        private void StackPanel_Checked(object sender, RoutedEventArgs e)
        {
            var path = (string)(e.OriginalSource as RadioButton)?.Tag;

            file.SetBinding(TextBox.TextProperty, new Binding(path)            
            {
                Source = DataContext,
                Mode = BindingMode.OneWay
            });            
        }
    }
}
