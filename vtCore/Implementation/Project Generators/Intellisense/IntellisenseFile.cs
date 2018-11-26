using System;
using System.Collections.Generic;
using System.Text;

namespace vtCore
{
    public static class IntellisenseFile
    {
        public static string generate(IProject project, LibManager libManager, SetupData setup)
        {
            switch (project.target)
            {
                case Target.vsCode:
                    if (project.buildSystem == BuildSystem.makefile)
                    {
                        return C_CPP_Props_make.generate(project,libManager);
                    }
                    else
                    {
                        return C_CPP_Props_builder.generate(project, libManager, setup);
                    }

                case Target.atom:
                    return "TBD";

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
