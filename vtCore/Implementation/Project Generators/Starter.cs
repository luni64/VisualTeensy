using System.Diagnostics;

namespace vtCore
{
    public static class Starter
    {
        static public void start_vsCode(string folder, string file)
        {
            var vsCode = new Process();
            vsCode.StartInfo.FileName = "cmd";
            vsCode.StartInfo.Arguments = $"/c code \"{folder}\" {file}";
            vsCode.StartInfo.WorkingDirectory = folder;
            vsCode.StartInfo.UseShellExecute = false;
            vsCode.StartInfo.CreateNoWindow = true;
            vsCode.Start();
            return;
        }
        static public void start_atom(string folder, string file)
        {
            var vsCode = new Process();
            vsCode.StartInfo.FileName = "cmd";
            vsCode.StartInfo.Arguments = $"/c atom \"{folder}\"";
            vsCode.StartInfo.WorkingDirectory = folder;
            vsCode.StartInfo.UseShellExecute = false;
            vsCode.StartInfo.CreateNoWindow = true;
            vsCode.Start();
            return;
        }
    }
}
