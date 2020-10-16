using log4net;
using log4net.Appender;
//using log4net.Config;
using log4net.Core;
using log4net.Repository.Hierarchy;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using ViewModel;
using VisualTeensy.Properties;
using vtCore;
using vtCore.Interfaces;

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

            SetupData setupData = new SetupData();

            if (Settings.Default.FirstStart)
            {
                log.Info("First startup");
                new StartupSettingsView(new StartupSettingsVM(setupData)).ShowDialog();

                setupData.projectBaseDefault = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source");
                setupData.uplPjrcBase.path = Helpers.findTyToolsFolder();
                setupData.uplTyBase.path = setupData.arduinoTools;
                setupData.uplCLIBase.path = Helpers.findCLIFolder();
                setupData.uplJLinkBase.path = Helpers.findJLinkFolder();
                setupData.makeExeBase.path = Directory.GetCurrentDirectory();
                //setupData.tdLibBase = Path.Combine(Helpers.getSketchbookFolder() ?? "", "libraries");
                setupData.tdLibBase = Path.Combine(setupData.arduinoCoreBase ?? "", "libraries");

                setupData.isColoredOutput = true;
                setupData.colorCore = Color.FromArgb(255, 187, 206, 251);
                setupData.colorUserLib = Color.FromArgb(255, 206, 244, 253);
                setupData.colorUserSrc = Color.FromArgb(255, 100, 149, 237);
                setupData.colorOk = Color.FromArgb(255, 179, 255, 179);
                setupData.colorLink = Color.FromArgb(255, 255, 255, 202);
                setupData.colorErr = Color.FromArgb(255, 255, 159, 159);

                Settings.Default.FirstStart = false;
            }
            else
            {
                setupData.arduinoBase = Settings.Default.arduinoBase;

                setupData.projectBaseDefault = Settings.Default.projectBaseDefault;
                setupData.uplPjrcBase.path = Settings.Default.uplPjrcBase;
                setupData.uplTyBase.path = Settings.Default.uplTyBase;
                setupData.uplCLIBase.path = Settings.Default.uplCLIBase;
                setupData.uplJLinkBase.path = Settings.Default.uplJLinkBase;
                setupData.makeExeBase.path = Settings.Default.makeExePath;
                setupData.tdLibBase = Path.Combine(setupData.arduinoCoreBase ?? "", "libraries");

                setupData.isColoredOutput = Settings.Default.ColorEnabled;
                setupData.colorCore = Settings.Default.ColCore;
                setupData.colorUserLib = Settings.Default.ColLib;
                setupData.colorUserSrc = Settings.Default.ColSrc;
                setupData.colorOk = Settings.Default.ColOk;
                setupData.colorLink = Settings.Default.ColLink;
                setupData.colorErr = Settings.Default.ColErr;
            }

            setupData.mru.load(Settings.Default.mruString);
            Helpers.arduinoPath = setupData.arduinoBase;

            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("VisualTeensy.Embedded.makefile_make")))
            {
                setupData.makefile_fixed = reader.ReadToEnd();
            }
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("VisualTeensy.Embedded.makefile_builder")))
            {
                setupData.makefile_builder = reader.ReadToEnd();
            }



            return setupData;
        }

        void saveSetup(SetupData setupData)
        {
            Settings.Default.arduinoBase = setupData.arduinoBase;
            Settings.Default.projectBaseDefault = setupData.projectBaseDefault;
            Settings.Default.uplPjrcBase = setupData.uplPjrcBase.path;
            Settings.Default.uplTyBase = setupData.uplTyBase.path;
            Settings.Default.uplCLIBase = setupData.uplCLIBase.path;
            Settings.Default.uplJLinkBase = setupData.uplJLinkBase.path;
            Settings.Default.makeExePath = setupData.makeExeBase.path;
            Settings.Default.libBase = setupData.tdLibBase;
            Settings.Default.debugSupport = setupData.debugSupportDefault;
            Settings.Default.ColorEnabled = setupData.isColoredOutput;
            Settings.Default.ColCore = setupData.colorCore;
            Settings.Default.ColLib = setupData.colorUserLib;
            Settings.Default.ColSrc = setupData.colorUserSrc;
            Settings.Default.ColOk = setupData.colorOk;
            Settings.Default.ColLink = setupData.colorLink;
            Settings.Default.ColErr = setupData.colorErr;
            Settings.Default.mruString = setupData.mru.ToString();

            Settings.Default.mainWinBounds = new Rect(mainWin.Left, mainWin.Top, mainWin.Width, mainWin.Height);

            Settings.Default.Save();

        }

        static public MainWindow mainWin { get; private set; }


        protected override void OnStartup(StartupEventArgs e)
        {
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            // create applicaton folder <user>\AppData\Local\VisualTeensy (if not yet existing)
            Directory.CreateDirectory(SetupData.vtAppFolder);

            // setup logger, log into application folder
            log4net.Config.XmlConfigurator.Configure();
            var repository = (Hierarchy)LogManager.GetRepository();
            repository.Threshold = Level.All;
            var fa = repository.Root.Appenders.OfType<FileAppender>().FirstOrDefault();
            fa.File = Path.Combine(SetupData.vtAppFolder, "VisualTeensy.log");
            fa.ActivateOptions();

            var v = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            log.Info($"------------------------------------------");
            log.Info($"Startup v{v.Major}.{v.Minor} ({v.Revision})");

            base.OnStartup(e);

            try
            {



                var setup = loadSetup();
                if (setup.errors.Count > 0)
                {
                    string errors = "";
                    setup.errors.ForEach(err => errors += (err + '\n'));
                    log.Error(errors);

                    MessageBox.Show($"Setting errors found!\n{errors}", caption: "VisualTeensy", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                var libManager = Factory.makeLibManager(setup);
                var project = Factory.makeProject(setup, libManager);

                if (!string.IsNullOrWhiteSpace(Settings.Default.lastProject))
                {
                    project.openProject(Settings.Default.lastProject);
                }
                else
                {
                    project.newProject();
                }

                var mainVM = new MainVM(project, libManager, setup);

                mainWin = new MainWindow()
                {
                    DataContext = mainVM,
                    Left = Settings.Default.mainWinBounds.Left,
                    Top = Settings.Default.mainWinBounds.Top,
                    Width = Settings.Default.mainWinBounds.Width,
                    Height = Settings.Default.mainWinBounds.Height,
                };

                mainWin.ShowDialog();

                Settings.Default.lastProject = project.path;
                saveSetup(setup);

                log.Info("Closed");

                Shutdown();
            }
            catch (Exception ex)
            {
                log.Fatal("Unhandled exception", ex);
                MessageBox.Show(ex.Message + "\n" + ex.ToString(), "VisualTeensy, unhandled Exception");
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
