namespace vtCore
{
    public class Makefile
    {
        public static string generate(IProject project, LibManager libManager, SetupData setup)
        {
            if (project.buildSystem == BuildSystem.makefile)
            {
                return Makefile_Make.generate(project, libManager, setup);
            }
            else
            {
                return Makefile_Builder.generate(project, libManager, setup);
            }
        }
    }
}
