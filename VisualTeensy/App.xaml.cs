using log4net;
using log4net.Appender;
//using log4net.Config;
using log4net.Core;
using log4net.Repository.Hierarchy;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using ViewModel;
using VisualTeensy.Properties;
using vtCore;

namespace VisualTeensy
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        SetupData loadSetup()
        {
            if (Settings.Default.updateNeeded)
            {
                log.Info("Update settings to new version");
                Settings.Default.Upgrade();
                Settings.Default.updateNeeded = false;
                Settings.Default.Save();
                Settings.Default.Reload();
            }

            var setupData = new SetupData();


            setupData.arduinoBase = String.IsNullOrWhiteSpace(Settings.Default.arduinoBase) ? Helpers.findArduinoFolder()?.Trim() : Settings.Default.arduinoBase;
            Helpers.arduinoPath = setupData.arduinoBase;

            setupData.projectBaseDefault = String.IsNullOrWhiteSpace(Settings.Default.projectBaseDefault) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source") : Settings.Default.projectBaseDefault;
            setupData.uplPjrcBase = String.IsNullOrWhiteSpace(Settings.Default.uplPjrcBase) ? setupData.arduinoTools : Settings.Default.uplPjrcBase;
            setupData.uplTyBase = String.IsNullOrWhiteSpace(Settings.Default.uplTyBase) ? Helpers.findTyToolsFolder() : Settings.Default.uplTyBase;
            setupData.uplCLIBase = String.IsNullOrWhiteSpace(Settings.Default.uplCLIBase) ? Helpers.findCLIFolder() : Settings.Default.uplCLIBase;
            setupData.makeExePath = String.IsNullOrWhiteSpace(Settings.Default.makeExePath) ? Path.Combine(Directory.GetCurrentDirectory(), "make.exe") : Settings.Default.makeExePath;
            setupData.libBase = String.IsNullOrWhiteSpace(Settings.Default.libBase) ? Path.Combine(Helpers.getSketchbookFolder() ?? "", "libraries") : Settings.Default.libBase;

            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("VisualTeensy.Embedded.makefile_make")))
            {
                setupData.makefile_fixed = reader.ReadToEnd();
            }
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("VisualTeensy.Embedded.makefile_builder")))
            {
                setupData.makefile_builder = reader.ReadToEnd();
            }
            Helpers.arduinoPath = setupData.arduinoBase;

            return setupData;
        }

        void saveSetup(SetupData setupData)
        {
            Settings.Default.arduinoBase = setupData.arduinoBase;
            Settings.Default.projectBaseDefault = setupData.projectBaseDefault;
            Settings.Default.uplPjrcBase = setupData.uplPjrcBase;
            Settings.Default.uplTyBase = setupData.uplTyBase;
            Settings.Default.uplCLIBase = setupData.uplCLIBase;
            Settings.Default.makeExePath = setupData.makeExePath;
            Settings.Default.libBase = setupData.libBase;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            log4net.Config.XmlConfigurator.Configure();
            var repository = (Hierarchy)LogManager.GetRepository();
            repository.Threshold = Level.All;
            var fa = repository.Root.Appenders.OfType<FileAppender>().FirstOrDefault();
            fa.File = Path.Combine(Path.GetTempPath(), "VisualTeensy.log");
            fa.ActivateOptions();
            
            var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            log.Info($"------------------------------------------");
            log.Info($"Startup v{v.Major}.{v.Minor} ({v.Revision})");

            base.OnStartup(e);
            
            try
            {
                var setup = loadSetup();               
                setup.libBase = setup.arduinoBoardsTxt != null?  Path.Combine(Path.GetDirectoryName(setup.arduinoBoardsTxt), "libraries") : null;
               

                var  libManager = Factory.makeLibManager(setup);
                var project =  Factory.makeProject(setup, libManager);

                if (!string.IsNullOrWhiteSpace(Settings.Default.lastProject))
                {
                    project.openProject(Settings.Default.lastProject);
                }
                else
                {
                    project.newProject();
                }

                var mainVM = new MainVM(project, libManager, setup);

                var mainWin = new MainWindow()
                {
                    DataContext = mainVM,
                    Left = Settings.Default.mainWinBounds.Left,
                    Top = Settings.Default.mainWinBounds.Top,
                    Width = Settings.Default.mainWinBounds.Width,
                    Height = Settings.Default.mainWinBounds.Height,
                };

                mainWin.ShowDialog();

                saveSetup(setup);

                Settings.Default.mainWinBounds = new Rect(mainWin.Left, mainWin.Top, mainWin.Width, mainWin.Height);
                Settings.Default.lastProject = project.path;
                Settings.Default.Save();

                log.Info("Closed");
            }
            catch (Exception ex)
            {
                log.Fatal("Unhandled exception", ex);
                MessageBox.Show(ex.Message + "\n" + ex.ToString());
            }
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}

//private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
//{
//    String resourceName = "VisualTeensy.Embedded." + new AssemblyName(args.Name).Name + ".dll";

//    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
//    {
//        Byte[] assemblyData = new Byte[stream.Length];
//        stream.Read(assemblyData, 0, assemblyData.Length);
//        return Assembly.Load(assemblyData);
//    }
//}
