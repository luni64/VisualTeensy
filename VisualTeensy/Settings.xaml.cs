using Microsoft.Win32;
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
using System.IO;

namespace BoardReader
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void browseInput(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();
            if (File.Exists(InputFilename.Text))
            {
                dlg.FileName = Path.GetFileName(InputFilename.Text);
                dlg.InitialDirectory = Path.GetDirectoryName(InputFilename.Text);
            }
            if (dlg.ShowDialog() != false)
            {
                InputFilename.Text = dlg.FileName;
            }

        }

        private void browseOutput(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog();
            if (File.Exists(OutputFilename.Text))
            {
                dlg.FileName = Path.GetFileName(OutputFilename.Text);
                dlg.InitialDirectory = Path.GetDirectoryName(OutputFilename.Text);
            }
            if (dlg.ShowDialog() != false)
            {
                OutputFilename.Text = dlg.FileName;
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
