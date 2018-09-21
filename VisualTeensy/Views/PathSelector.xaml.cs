using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Windows;
using System.Windows.Controls;

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
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = isFolderDialog;
                dialog.AllowNonFileSystemItems = false;
                dialog.AddToMostRecentlyUsedList = true;
                try
                {
                    dialog.InitialDirectory = Path.GetDirectoryName(SelectedPath);
                    dialog.DefaultFileName = Path.GetFileName(SelectedPath);
                }
                catch { }

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    SelectedPath = dialog.FileName;
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


