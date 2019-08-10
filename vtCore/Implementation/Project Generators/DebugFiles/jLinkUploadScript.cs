using System.IO;
using System.Text;
using vtCore.Interfaces;

namespace vtCore
{
    public static class JLinkUploadScript
    {
        public static string generate(IProject project, SetupData setup)
        {
            IConfiguration cfg = project.selectedConfiguration;
            (string target, string svd) debug = DebugFile.seggerDebugTargets.TryGetValue(cfg.selectedBoard.id, out (string, string) value) ? value : ("unknown", "unknown");
            string hexFile = $".vsteensy/build/{project.cleanName}.hex";

            var buildBat = new StringBuilder();

            buildBat.Append($"device {debug.target}\n");
            buildBat.Append($"si 1\n");
            buildBat.Append($"speed 4000\n");
            buildBat.Append($"r\n");
            buildBat.Append($"loadfile {hexFile}\n");
            buildBat.Append($"rnh\n");
            buildBat.Append($"exit\n");

            return buildBat.ToString();
        }
    }
}
