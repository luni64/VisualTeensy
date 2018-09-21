using Board2Make.Model;
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


        public String makePath
        {
            get => data.makeExePath;
            set
            {
                if (value != data.makeExePath)
                {
                    data.makeExePath = value.Trim();
                    OnPropertyChanged();
                }
            }
        }


        public String uploadTyPath
        {
            get => data.uplTyBase;
            set
            {
                if (value != data.uplTyBase)
                {
                    data.uplTyBase = value;
                    OnPropertyChanged();
                }
            }
        }
        public String uploadPjrcPath
        {
            get => data.uplPjrcBase;
            set
            {
                if (value != data.uplPjrcBase)
                {
                    data.uplPjrcBase = value;
                    OnPropertyChanged();
                }
            }
        }
        public String projectBaseDefault
        {
            get => data.projectBaseDefault;
            set
            {
                if (value != data.projectBaseDefault)
                {
                    data.projectBaseDefault = value;
                    OnPropertyChanged();
                }
            }
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
                        error = data.projectBaseError;
                        break;

                    case "makePath":
                        error = data.makeExePathError;
                        break;

                    case "uploadTyPath":
                        error = data.uplTyBaseError;
                        break;

                    case "uploadPjrcPath":
                        error = data.uplPjrcBaseError;
                        break;

                    default:
                        error = null;
                        break;
                }
                return error;
            }
        }

        public SetupTabVM(Model model)
        {
            this.data = model.data;

            cmdDownloadMake = new AsyncCommand(doDownload);
        }


        SetupData data;
    }
}

