using Board2Make;
using System;
using System.Reflection;
using System.Windows;


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


            var win = new MainWindow(); 
            win.Show();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            String resourceName = "Board2Make." + new AssemblyName(args.Name).Name + ".dll";

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

