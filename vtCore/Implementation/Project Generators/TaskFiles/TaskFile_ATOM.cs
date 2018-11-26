using Newtonsoft.Json;

namespace vtCore
{
    static class TaskFile_ATOM
    {
        static public string generate(IProject project, SetupData setup)
        {
            string make = setup.makeExePath.Replace('\\', '/');
            string parallelExecute = project.buildSystem == BuildSystem.makefile ? "-j -Otarget" : "";

            var tasklist = new
            {
                patterns = new
                {
                    P1 = new { expression = "(path)\\(line)" }
                },

                commands = new[]
                {
                    new {
                        namspace = "process-palette",
                        action = "Build",
                        command = make,
                        arguments = new []{ "all", parallelExecute},
                        cwd = "{projectPath}",
                        saveOption = "all",
                        promptToSave = false,
                        keystroke = "ctrl-shift-b",
                        group = new Group(),
                        stream = true,
                        patterns = new  []{"default"},
                        menus = new[]{"vsTeensy" },
                    },
                    new {
                        namspace = "process-palette",
                        action = "Clean",
                        command = make,
                        arguments = new []{ "clean"},
                        cwd = "{projectPath}",
                        saveOption = "all",
                        promptToSave = false,
                        keystroke = "ctrl-shift-c",
                        group = new Group(),
                        stream = true,
                        patterns = new  []{"default"},
                        menus = new[]{"vsTeensy" },
                    },
                     new {
                        namspace = "process-palette",
                        action = "Upload (PJRC)",
                        command = make,
                        arguments = new []{ "upload", parallelExecute},
                        cwd = "{projectPath}",
                        saveOption = "all",
                        promptToSave = false,
                        keystroke = "ctrl-shift-p",
                        group = new Group(),
                        stream = true,
                        patterns = new  []{"default"},
                        menus = new[]{"vsTeensy" },
                    },
                      new {
                        namspace = "process-palette",
                        action = "Upload (TY-Tools)",
                        command = make,
                        arguments = new []{ "uploadTy", parallelExecute},
                        cwd = "{projectPath}",
                        saveOption = "all",
                        promptToSave = false,
                        keystroke = "ctrl-shift-u",
                        group = new Group(),
                        stream = true,
                        patterns = new  []{"default"},
                        menus = new[]{"vsTeensy" },
                    }
                }
            };
            return JsonConvert.SerializeObject(tasklist, Formatting.Indented);
        }

        //static public string generate(IProject project, SetupData setup)
        //{
        //    //log.Debug("enter");
        //    if (setup.makeExePathError != null)
        //    {
        //        return null;
        //    }

        //    string make = setup.makeExePath.Replace('\\', '/');

        //    string build_ToolsCson = File.ReadAllText("Assets/Atom-build-tools-make.cson");

        //    build_ToolsCson = build_ToolsCson.Replace("{PROJECT}", project.path);
        //    build_ToolsCson = build_ToolsCson.Replace("{SOURCE}", project.path + "\\.build-tools.cson");
        //    build_ToolsCson = build_ToolsCson.Replace("{MAKE}", make.Replace('/','\\'));

        //    return build_ToolsCson.Replace("\\", "\\\\"); 
        //}
    }
}
