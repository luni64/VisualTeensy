using VisualTeensy.Model;
using System;
using System.ComponentModel;
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
                        error = project.setup.makeExePathError;
                        break;

                    case "uploadTyPath":
                        error = project.setup.uplTyBaseError;
                        break;

                    case "uploadPjrcPath":
                        error = project.setup.uplPjrcBaseError;
                        break;

                    case "uploadCLIPath":
                        error = project.setup.uplCLIBaseError;
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
            get => project.setup.arduinoBase;
            set
            {
                if (value != project.setup.arduinoBase)
                {
                    project.setup.arduinoBase = value;
                    OnPropertyChanged();
                }
            }
        }
        public String makePath
        {
            get => project.setup.makeExePath;
            set
            {
                if (value != project.setup.makeExePath)
                {
                    project.setup.makeExePath = value.Trim();
                    OnPropertyChanged();
                }
            }
        }
        public String uploadTyPath
        {
            get => project.setup.uplTyBase;
            set
            {
                if (value != project.setup.uplTyBase)
                {
                    project.setup.uplTyBase = value;
                    OnPropertyChanged();
                }
            }
        }
        public String uploadPjrcPath
        {
            get => project.setup.uplPjrcBase;
            set
            {
                if (value != project.setup.uplPjrcBase)
                {
                    project.setup.uplPjrcBase = value;
                    OnPropertyChanged();
                }
            }
        }
        public String uploadCLIPath
        {
            get => project.setup.uplCLIBase;
            set
            {
                if (value != project.setup.uplCLIBase)
                {
                    project.setup.uplCLIBase = value;
                    OnPropertyChanged();
                }
            }
        }
        public String projectBaseDefault
        {
            get => project.setup.projectBaseDefault;
            set
            {
                if (value != project.setup.projectBaseDefault)
                {
                    project.setup.projectBaseDefault = value;
                    OnPropertyChanged();
                }
            }
        }


        public SetupTabVM(Project project)
        {
            this.project = project;          
           // this.setup = project.setup;

            cmdDownloadMake = new AsyncCommand(doDownload);
        }

        
        //SetupData setup;

        Project project;

        
    }
}

