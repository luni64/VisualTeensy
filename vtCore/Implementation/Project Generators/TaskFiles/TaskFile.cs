using log4net;
using System.Reflection;
using vtCore.Interfaces;

namespace vtCore
{
    public class TaskFile
    {
        public static string generate(IProject project, SetupData setup)
        {
            log.Info($"generate, target: {project.target} ");

            switch (project.target)
            {
                case Target.vsCode:
                    return TaskFile_vsCode.generate(project, setup);

                case Target.atom:
                    return TaskFile_ATOM.generate(project, setup);

                case Target.sublimeText:
                    return "TBD";

                case Target.vsFolder:
                    return "TBD";

                default:
                    return "Error";
            }
        }
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
    
}
