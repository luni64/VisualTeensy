using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using ViewModel;
using VisualTeensy;
using VisualTeensy.Model;
using VisualTeensy.Properties;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void loadSettings(out ProjectData projectData, out SetupData setupData)
        {
            if (Settings.Default.updateNeeded)
            {
                Settings.Default.Upgrade();
                Settings.Default.updateNeeded = false;
                Settings.Default.Save();
                Settings.Default.Reload();
            }

            setupData = Settings.Default.setupData;

            setupData = Settings.Default.setupData ?? SetupData.getDefault();
            projectData = Settings.Default.projectData ?? ProjectData.getDefault(setupData);

            FileHelpers.arduinoPath = setupData.arduinoBase;

            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("VisualTeensy.Embedded.makefile")))
            {
                setupData.makefile_fixed = reader.ReadToEnd();
            }
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");
            
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            try
            {
                loadSettings(out var projectData, out var setupData);

                var model = new Model(projectData, setupData);
                var mainVM = new MainVM(model);

                var mainWin = new MainWindow()
                { 
                    DataContext = mainVM,
                    Left = Settings.Default.mainWinBounds.Left,
                    Top = Settings.Default.mainWinBounds.Top,
                    Width = Settings.Default.mainWinBounds.Width,
                    Height = Settings.Default.mainWinBounds.Height,
                };

                mainWin.ShowDialog();

                // close open file display windows // hack, move elsewhere
                Current.Windows.OfType<FileDisplayWindow>().ToList().ForEach(w => w.Close());
                                 
                Settings.Default.mainWinBounds = new Rect(mainWin.Left, mainWin.Top, mainWin.Width, mainWin.Height);
                Settings.Default.projectData = projectData;
                Settings.Default.setupData = setupData;
                Settings.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());                
            }
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            String resourceName = "VisualTeensy.Embedded." + new AssemblyName(args.Name).Name + ".dll";

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                Byte[] assemblyData = new Byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }
    }
}

