using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Board2Make
{
    /// <summary>
    /// Interaction logic for PathSelector.xaml
    /// </summary> 
    public partial class PSelector : UserControl
    {
        public PSelector()
        {
            InitializeComponent();
        }

        private void OpenInput(object sender, RoutedEventArgs e)
        {
            if (isFolderDialog)
            {
                var dlg = new System.Windows.Forms.FolderBrowserDialog();

                dlg.SelectedPath = SelectedPath;

                //fileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SelectedPath = dlg.SelectedPath;
                }
            }
            else
            {
                var dlg = new System.Windows.Forms.OpenFileDialog();
                //fileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

                if (Directory.Exists(SelectedPath))
                {

                    dlg.InitialDirectory = Path.GetDirectoryName(SelectedPath);

                    dlg.FileName = Path.GetFileName(SelectedPath);

                }



                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SelectedPath = dlg.FileName;
                }
            }

        }

        #region Dependency Properties

        public static readonly DependencyProperty SelectedPathProperty = DependencyProperty.Register("SelectedPath", typeof(string), typeof(PSelector), new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true });
        public string SelectedPath
        {
            get => (string)GetValue(SelectedPathProperty);
            set => SetValue(SelectedPathProperty, value);
        }


        public static readonly DependencyProperty isFolderDialogProperty = DependencyProperty.Register("isFolderDialog", typeof(bool), typeof(PSelector), new FrameworkPropertyMetadata());
        public bool isFolderDialog
        {
            get => (bool)GetValue(isFolderDialogProperty);
            set => SetValue(isFolderDialogProperty, value);
        }

        #endregion


    }
}


