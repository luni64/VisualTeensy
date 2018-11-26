using Newtonsoft.Json;
using System.Collections.Generic;

namespace vtCore
{
    static class TaskFile_vsCode
    {
        static public string generate(IProject project, SetupData setup)
        {
            string make = setup.makeExePath.Replace('\\', '/');
            string j = project.buildSystem == BuildSystem.makefile ? "-j" : "";
            string O = project.buildSystem == BuildSystem.makefile ? "-Otarget" : "";

            var tasklist = new
            {
                tasks = new[]
                {
                new{
                    label = "Build",
                    group = new Group(),
                    command = make,
                    args = new List<string> { "all", j, O },
                },
               new{
                   label = "Upload (Teensy Uploader)",
                   group = new Group(),
                   command = make,
                   args = new List<string> { "upload", j, O },
               },
                new {
                    label = "Upload (TyCommander)",
                    group = new Group(),
                    command = make,
                    args = new List<string> { "uploadTy", j, O },
                },
                new {
                    label = "Upload (CLI)",
                    group = new Group(),
                    command = make,
                    args = new List<string> { "uploadCLI", j,O },
                },
                new{
                    label = "Clean",
                    group = new Group(),
                    command = make,
                    args = new List<string> { "clean" },
                } },
                version = "2.0.0",
                type = "shell",
                problemMatcher = "$gcc",
                presentation = new
                {
                    echo = true,
                    reveal = "always",
                    focus = false,
                    panel = "shared",
                    showReuseMessage = false,
                },
            };

            return JsonConvert.SerializeObject(tasklist, Formatting.Indented);
        }
    }
}
