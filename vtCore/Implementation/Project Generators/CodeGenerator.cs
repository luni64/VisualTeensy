using System.Collections.Generic;
using System.Linq;
using vtCore.Interfaces;

namespace vtCore
{
    public static class CodeGenerator
    {
        public static IReadOnlyList<ITask> getTasks(IProject project, LibManager libManager, SetupData setup)
        {
            var tasks = new List<ITask>();

            switch (project.target)
            {
                case Target.vsCode:
                    tasks.Add(new PrepareFolders(project));
                    tasks.Add(new GenerateSettings(project));
                    tasks.Add(new GenerateMakefile(project, libManager, setup));
                    tasks.Add(new GenerateTasks(project, setup));
                    tasks.Add(new GenerateIntellisense(project, libManager, setup)); // needs to be added after libraries (checks for existence)
                    if (project.buildSystem == BuildSystem.makefile)
                    {
                        tasks.Add(new CopyLibs(project));
                        switch(project.selectedConfiguration.coreStrategy)
                        {
                            case LibStrategy.copy: tasks.Add(new CopyCore(project)); break;
                            case LibStrategy.clone: tasks.Add(new CloneCore(project)); break;
                        }                      
                    }
                    if(project.debugSupport == DebugSupport.cortex_debug)
                    {
                        tasks.Add(new GenerateDebugSupport(project, setup));                            
                    }
                    if(setup.additionalFiles.Any())
                    {
                        tasks.Add(new CopyAdditionalFiles(project, setup));
                    }
                    tasks.Add(new CleanBinaries(project));
                    tasks.Add(new GenerateSketch(project));
                    break;

                case Target.atom:
                    break;
                case Target.sublimeText:
                    break;
            }

            return tasks;
        }  
    }
}
