namespace vtCore
{
    public class TaskFile
    {
        public static string generate(IProject project, LibManager libManager, SetupData setup)
        {
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
    }
}
