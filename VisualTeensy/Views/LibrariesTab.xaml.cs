﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewModel;

namespace VisualTeensy
{
    /// <summary>
    /// Interaction logic for LibrariesTab.xaml
    /// </summary>
    public partial class LibrariesTab : UserControl
    {
        public LibrariesTab()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as LibrariesTabVM;

            Button b = (Button)sender;

            var t = b.DataContext as LibraryVM;

            //var x = lb.ItemContainerGenerator.ContainerFromItem((Button)e.OriginalSource);


            // (ListBoxItem)lb.ItemContainerGenerator.ContainerFromItem( ((Button)sender).DataContext);

            MessageBox.Show($"{t.selectedVersion.version}");
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //if (Directory.Exists(e.Uri.LocalPath))
            //{
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            //}
            //else MessageBox.Show($"Path {e.Uri.LocalPath} does not exist", "VisualTeensy", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void removeButtonClock(object sender, RoutedEventArgs e)
        {
            var button = ((Button)sender).DataContext;

            var dc = DataContext as LibrariesTabVM;

            dc.cmdDel.Execute(button);
        }
    }
}