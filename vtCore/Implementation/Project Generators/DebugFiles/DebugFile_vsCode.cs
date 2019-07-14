using Newtonsoft.Json;
using System.Collections.Generic;

namespace vtCore
{
    static class DebugFile_vsCode
    {
        static public string generate(IProject project, SetupData setup)
        {
            string make = setup.makeExePath.Replace('\\', '/');


            var launchJson = new
            {
                version = "0.2.0",

                configurations = new[]
                {
                  new
                  {
                      name = "Cortex Debug",
                      cwd = "${workspaceRoot}",
                      executable = ".vsteensy/build/debug.elf",
                      request = "attach",
                      type = "cortex-debug",
                      servertype = "jlink",
                      device = "MK64FX512xxx12",
                      svdFile= "c:\\MK64F12.svd",
                      armToolchainPath= "C:\\toolchain\\gcc\\gcc-arm-none-eabi-5_4-2016q3-20160926-win32\\bin",
                    }
            }


            };


            return JsonConvert.SerializeObject(launchJson, Formatting.Indented);
        }
    }
}
