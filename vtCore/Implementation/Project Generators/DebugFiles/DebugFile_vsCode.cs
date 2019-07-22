using Newtonsoft.Json;
using System.Collections.Generic;

namespace vtCore
{
    static class DebugFile_vsCode
    {
        static public string generate(IProject project, SetupData setup)
        {
            IConfiguration cfg = project.selectedConfiguration;
            (string target, string svd) = DebugFile.seggerDebugTargets.TryGetValue(cfg.selectedBoard.id, out (string, string) value) ? value : ("unknown", "unknown");
            string compilerBase =(cfg.setupType == SetupTypes.quick ? setup.arduinoCompiler : cfg.compilerBase.shortPath).Replace('\\', '/'); 
            //Helpers.getShortPath(cfg.setupType == SetupTypes.quick ? setup.arduinoCompiler : cfg.compilerBase).Replace('\\', '/'); ;

            var launchJson = new
            {
                version = "0.2.0",
                configurations = new[]
                {
                  new
                  {
                      name = "Cortex Debug - ATTACH",
                      cwd = "${workspaceRoot}",
                      executable = ".vsteensy/build/" + project.name?.Replace(" ", "_") + ".elf",
                      request = "attach",
                      type = "cortex-debug",
                      servertype = "jlink",
                      device = target,
                      svdFile= svd,
                      armToolchainPath= compilerBase + "/bin",
                  },
                  new
                  {
                      name = "Cortex Debug - LAUNCH",
                      cwd = "${workspaceRoot}",
                      executable = ".vsteensy/build/" + project.name?.Replace(" ", "_") + ".elf",
                      request = "launch",
                      type = "cortex-debug",
                      servertype = "jlink",
                      device = target,
                      svdFile= svd,
                      armToolchainPath= compilerBase + "/bin",
                  },
                }
            };


            return JsonConvert.SerializeObject(launchJson, Formatting.Indented);
        }
    }
}
