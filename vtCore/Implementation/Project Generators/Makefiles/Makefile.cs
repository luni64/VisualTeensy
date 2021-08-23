using log4net;
using System.Reflection;
using vtCore.Interfaces;

namespace vtCore
{
    public class Makefile
    {
        public static string generate(IProject project, LibManager libManager, SetupData setup)
        {
            log.Info($"generate, mode: {project.buildSystem}");

            if (project.buildSystem == BuildSystem.makefile)
            {                
                return Makefile_Make.generate(project, libManager, setup);
            }
            else
            {
                return Makefile_Builder.generate(project, libManager, setup);
            }
        }

        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}
