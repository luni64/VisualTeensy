using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using ViewModel;
using VisualTeensy;
using VisualTeensy.Model;

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

            try
            {
                string makefile = null;
                using (var reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("VisualTeensy.Embedded.makefile")))
                {
                    makefile = reader.ReadToEnd();
                }

                var data = new SetupData { makefile_fixed = makefile };
                var model = new Model(data);
                var mainVM = new MainVM(model);

                new MainWindow { DataContext = mainVM }.Show();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            String resourceName = "VisualTeensy.Embedded." + new AssemblyName(args.Name).Name + ".dll";

            //var x = Assembly.GetExecutingAssembly().GetManifestResourceNames().ToList();

            //x.ForEach(y => Console.WriteLine(y));



            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                Byte[] assemblyData = new Byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }

        }
    }
}

