using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using vtCore.Interfaces;

namespace vtCore
{
    static class TaskFile_vsCode
    {
        class task
        {
            public string label { get; set; }
            public Group group { get; set; } = new Group();
            public string command { get; set; }
            public List<string> args { get; set; }
        }

        static public string generate(IProject project, SetupData setup)
        {
            string make = Path.Combine(setup.makeExeBase.path ?? "Error", "make.exe").Replace('\\', '/');
            string j = project.buildSystem == BuildSystem.makefile ? "-j" : "";
            string O = project.buildSystem == BuildSystem.makefile ? "-Otarget" : "";

            var tasks = new List<task>()
            {    
                 new task
                 {
                     command = make,
                     label = "Build",
                     args = new List<string> { "all", j, O },
                 },
                 new task
                 {
                     command = make,
                     label = "Clean",
                     args = new List<string> { "clean" },
                 }
            };

            if (!String.IsNullOrWhiteSpace(setup.uplPjrcBase.path))
            {
                tasks.Add(new task
                {
                    command = make,
                    label = "Upload (teensy.exe)",
                    args = new List<string> { "upload", j, O }
                });
            }
            if (!String.IsNullOrWhiteSpace(setup.uplTyBase.path))
            {
                tasks.Add(new task
                {
                    command = make,
                    label = "Upload (TyCommander)",
                    args = new List<string> { "uploadTy", j, O }
                });
            }
            if (!String.IsNullOrWhiteSpace(setup.uplJLinkBase.path))
            {
                tasks.Add(new task
                {
                    command = make,
                    label = "Upload (JLink)",
                    args = new List<string> { "uploadJLink", j, O }
                });
            }
            if (!String.IsNullOrWhiteSpace(setup.uplCLIBase.path))
            {
                tasks.Add(new task
                {
                    command = make,
                    label = "Upload (CLI)",
                    args = new List<string> { "uploadCLI", j, O }
                });
            }

            var tasklist = new
            {                
                tasks,
                version = "2.0.0",
                type = "shell",
                problemMatcher = "$gcc",
                presentation = new
                {
                    echo = true,
                    clear = true,
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
