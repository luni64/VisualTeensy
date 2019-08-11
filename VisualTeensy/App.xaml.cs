﻿using log4net;
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

            var setupData = new SetupData();

            //var setupData = SetupData.getDefault();


            setupData.arduinoBase = String.IsNullOrWhiteSpace(Settings.Default.arduinoBase) ? Helpers.findArduinoFolder()?.Trim() : Settings.Default.arduinoBase;
            Helpers.arduinoPath = setupData.arduinoBase;

            setupData.projectBaseDefault = String.IsNullOrWhiteSpace(Settings.Default.projectBaseDefault) ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "source") : Settings.Default.projectBaseDefault;
           // setupData.uplPjrcBase.path = String.IsNullOrWhiteSpace(Settings.Default.uplPjrcBase) ? setupData.arduinoTools : Settings.Default.uplPjrcBase;
            setupData.uplTyBase.path = String.IsNullOrWhiteSpace(Settings.Default.uplTyBase) ? Helpers.findTyToolsFolder() : Settings.Default.uplTyBase;
            setupData.uplCLIBase.path = String.IsNullOrWhiteSpace(Settings.Default.uplCLIBase) ? Helpers.findCLIFolder() : Settings.Default.uplCLIBase;
            setupData.uplJLinkBase.path = String.IsNullOrWhiteSpace(Settings.Default.uplJLinkBase) ? Helpers.findJLinkFolder() : Settings.Default.uplJLinkBase;
            setupData.makeExeBase.path = String.IsNullOrWhiteSpace(Settings.Default.makeExePath) ? Directory.GetCurrentDirectory() : Settings.Default.makeExePath;
            setupData.tdLibBase = String.IsNullOrWhiteSpace(Settings.Default.libBase) ? Path.Combine(Helpers.getSketchbookFolder() ?? "", "libraries") : Settings.Default.libBase;
            setupData.debugSupportDefault = Settings.Default.debugSupport;

          

            setupData.isColoredOutput = Settings.Default.ColorEnabled;
            setupData.colorCore = Settings.Default.ColCore.IsEmpty ? Color.BlueViolet : Settings.Default.ColCore;
            setupData.colorUserLib = Settings.Default.ColLib.IsEmpty ? Color.CadetBlue : Settings.Default.ColLib;
            setupData.colorUserSrc = Settings.Default.ColSrc.IsEmpty ? Color.CornflowerBlue : Settings.Default.ColSrc;
            setupData.colorOk = Settings.Default.ColOk.IsEmpty ? Color.DarkGreen : Settings.Default.ColOk;
            setupData.colorLink = Settings.Default.ColLink.IsEmpty ? Color.Yellow : Settings.Default.ColLink;
            setupData.colorErr = Settings.Default.ColErr.IsEmpty ? Color.Red : Settings.Default.ColErr;


            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("VisualTeensy.Embedded.makefile_make")))
            {
                setupData.makefile_fixed = reader.ReadToEnd();
            }
            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("VisualTeensy.Embedded.makefile_builder")))
            {
                setupData.makefile_builder = reader.ReadToEnd();
            }
          //  Helpers.arduinoPath = setupData.arduinoBase;

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
        }

        static public MainWindow mainWin { get; private set; }


        protected override void OnStartup(StartupEventArgs e)
        {
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Directory.CreateDirectory(appData);

            
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

            // overwrite local lib index if  older than 7 days 
            string libIndexTarget = appData + "/VisualTeensy/library_index.json";
            bool overwrite = !File.Exists(libIndexTarget);
            overwrite |= (DateTime.Now - File.GetLastWriteTime(libIndexTarget)) > TimeSpan.FromDays(7);
            overwrite &= Uri.TryCreate("http://downloads.arduino.cc/libraries/library_index.json", UriKind.Absolute, out var libIndexSource);
            if (overwrite)
            {
                log.Info($"Download {libIndexSource} to {libIndexTarget}");
                Task.Run(() => Helpers.downloadFile(libIndexSource, libIndexTarget, TimeSpan.FromDays(7)));
                log.Info($"Download done");
            }
            
            try
            {
                var setup = loadSetup();               
                setup.tdLibBase = setup.arduinoBoardsTxt != null?  Path.Combine(Path.GetDirectoryName(setup.arduinoBoardsTxt), "libraries") : null;
               
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

                mainWin = new MainWindow()
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
