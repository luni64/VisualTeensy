using System;
using System.ComponentModel;
using vtCore;
using Task = System.Threading.Tasks.Task;

namespace ViewModel
{
    public class SetupTabVM : BaseViewModel, IDataErrorInfo
    {
        public AsyncCommand cmdDownloadMake { get; private set; }
        async System.Threading.Tasks.Task doDownload()
        {
            await Task.Delay(1);
            return;

            //try
            //{
            //    const string url = "ftp://ftp.equation.com/make/64/make.exe";
            //    NetworkCredential credentials = new NetworkCredential("anonymous", "lutz.niggl@lunoptics.com");

            //    // Query size of the file to be downloaded
            //    WebRequest sizeRequest = WebRequest.Create(url);
            //    sizeRequest.Credentials = credentials;
            //    sizeRequest.Method = WebRequestMethods.Ftp.GetFileSize;

            //    var t = sizeRequest.GetResponseAsync();
            //    while(!t.IsCompleted)
            //    {
            //        Console.Write(".");
            //        await System.Threading.Tasks.Task.Delay(10);
            //    }


            //    int size = (int) (t.Result ).ContentLength;
            //    Console.WriteLine();
            //    Console.WriteLine(size);

            //    //progressBar1.Invoke(
            //    //    (MethodInvoker)(() => progressBar1.Maximum = size));

            //    // Download the file
            //    WebRequest request = WebRequest.Create(url);
            //    request.Credentials = credentials;
            //    request.Method = WebRequestMethods.Ftp.DownloadFile;

            //    var response  = await request.GetResponseAsync();
            //    var ftpStream = response.GetResponseStream();


            //    using (Stream fileStream = File.Create(@"C:\toolchain\test.exe"))
            //    {
            //        byte[] buffer = new byte[10240];
            //        int read;
            //        while ((read = ftpStream.Read(buffer, 0, buffer.Length)) > 0)
            //        {
            //            fileStream.Write(buffer, 0, read);
            //            int position = (int)fileStream.Position;
            //            //progressBar1.Invoke(
            //            //    (MethodInvoker)(() => progressBar1.Value = position));
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //   // MessageBox.Show(e.Message);
            //}
        }

        public string Error => throw new NotImplementedException();
        public string this[string columnName]
        {
            get
            {
                string error;

                switch (columnName)
                {
                    case "projectPathDefault":
                        error = project.pathError;
                        break;

                    case "makePath":
                        error = setup.makeExePathError;
                        break;

                    case "uploadTyPath":
                        error = setup.uplTyBaseError;
                        break;

                    case "uploadPjrcPath":
                        error = setup.uplPjrcBaseError;
                        break;

                    case "uploadCLIPath":
                        error = setup.uplCLIBaseError;
                        break;

                    default:
                        error = null;
                        break;
                }
                return error;
            }
        }

        public String arduinoBase
        {
            get => setup.arduinoBase;
            set
            {
                if (value != setup.arduinoBase)
                {
                    setup.arduinoBase = value;
                    OnPropertyChanged();
                }
            }
        }
        public String makePath
        {
            get => setup.makeExePath;
            set
            {
                if (value != setup.makeExePath)
                {
                    setup.makeExePath = value.Trim();
                    OnPropertyChanged();
                }
            }
        }
        public String uploadTyPath
        {
            get => setup.uplTyBase;
            set
            {
                if (value != setup.uplTyBase)
                {
                    setup.uplTyBase = value;
                    OnPropertyChanged();
                }
            }
        }
        public String uploadPjrcPath
        {
            get => setup.uplPjrcBase;
            set
            {
                if (value != setup.uplPjrcBase)
                {
                    setup.uplPjrcBase = value;
                    OnPropertyChanged();
                }
            }
        }
        public String uploadCLIPath
        {
            get => setup.uplCLIBase;
            set
            {
                if (value != setup.uplCLIBase)
                {
                    setup.uplCLIBase = value;
                    OnPropertyChanged();
                }
            }
        }
        public String projectBaseDefault
        {
            get => setup.projectBaseDefault;
            set
            {
                if (value != setup.projectBaseDefault)
                {
                    setup.projectBaseDefault = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool isTargetvsCode
        {
            get => project.target == Target.vsCode;
            set
            {
                if (value == true) project.target = Target.vsCode;
                OnPropertyChanged();
            }
        }
        public bool isTargetATOM
        {
            get => project.target == Target.atom;
            set
            {
                if (value == true) project.target = Target.atom;
                OnPropertyChanged();
            }
        }

        public bool isMakefileBuild
        {
            get => project.buildSystem == BuildSystem.makefile;
            set
            {
                if (value == true) project.buildSystem = BuildSystem.makefile;
                OnPropertyChanged();
            }
        }
        public bool isArduinoBuild
        {
            get => project.buildSystem == BuildSystem.arduino;
            set
            {
                if (value == true) project.buildSystem = BuildSystem.arduino;
                OnPropertyChanged();
            }
        }


        public SetupTabVM(IProject project, SetupData setup)
        {
            this.project = project;
            this.setup = setup;

            cmdDownloadMake = new AsyncCommand(doDownload);
        }


        SetupData setup;

        IProject project;


    }
}

