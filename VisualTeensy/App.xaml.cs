using System;
using System.IO;
using System.Reflection;
using System.Windows;
using ViewModel;
using VisualTeensy;

namespace WpfApplication1
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;


            using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("VisualTeensy.Embedded.makefile")))
            {
                string s = reader.ReadToEnd();
            }

            new MainWindow
            {
                DataContext = new MainVM()
            }
            .Show();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            String resourceName = "VisualTeensy.EmbeddedDlls." + new AssemblyName(args.Name).Name + ".dll";

            var x = Assembly.GetExecutingAssembly().GetManifestResourceNames();

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                Byte[] assemblyData = new Byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }

        }
    }
}

