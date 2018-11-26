using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace vtCore
{
    class TaskFile_atom_builder
    {
        static public string generate(IProject project, LibManager libManager, SetupData setup)
        {
          

            var buildBat = new StringBuilder();

            buildBat.Append("SET wd=%cd%\n");
            buildBat.Append("%\n");

            buildBat.Append(Path.Combine(Directory.GetCurrentDirectory(), "Assets\\Arduino-Builder "));
            buildBat.Append("-verbose=1 -build-path=%wd%/.vsTeensy/build ");            
            buildBat.Append($"-hardware={setup.arduinoBase}\\hardware ");
            buildBat.Append($"-tools={setup.arduinoBase}\\tools-builder -tools={setup.arduinoBase}\\hardware\\tools\\avr ");
            buildBat.Append($"-fqbn={project.selectedConfiguration.selectedBoard.fqbn} ");
            buildBat.Append($"-libraries={setup.arduinoBase}\\libraries ");
            buildBat.Append($"-libraries={setup.arduinoBase}\\hardware\\teensy\\avr\\libraries ");            
            buildBat.Append($"-libraries={Helpers.getSketchbookFolder()}\\libraries ");
            buildBat.Append($"{project.name}.ino");

            string s = buildBat.ToString();






            var tasklist = new List<Task>();

            tasklist.Add(new Task()
            {
                label = "Build (Arduino Builder)",
                group = new Group(),
                command = Path.Combine(setup.arduinoBase, "arduino-builder").Replace('\\', '/'),
                args = new List<string>
                {
                    "-verbose=0",
                    "-logger=human",
                    "-debug-level=0",
                    $"-hardware={setup.arduinoBase}\\hardware".Replace('\\', '/'),
                    "'-build-path=${workspaceFolder}/.vsTeensy/build'",
                    $"-tools={setup.arduinoBase}\\tools-builder".Replace('\\', '/'),
                    $"-tools={setup.arduinoBase}\\hardware\\tools\\avr".Replace('\\', '/'),
                    $"-fqbn={project.selectedConfiguration.selectedBoard.fqbn}",
                    $"-libraries={Path.Combine(setup.arduinoBase, "hardware", "teensy", "avr", "libraries")}".Replace('\\','/'),
                    $"-libraries={Path.Combine(setup.arduinoBase ,"libraries")}".Replace('\\','/'),
                    $"-libraries={Path.Combine(Helpers.getSketchbookFolder() ,"libraries")}".Replace('\\','/'),
                    $"{project.name}.ino"
                    }
            });

            tasklist.Add(new Task()
            {
                label = "Upload (PJRC)",
                group = new Group(),
                command = $"{setup.arduinoTools}/teensy_post_compile.exe".Replace('\\', '/'),
                args = new List<string>
                {
                    $"-test",
                    $"-reboot",
                    "'-path=${workspaceFolder}/.vsTeensy/build'",
                    $"-board={project.selectedConfiguration.selectedBoard.id}",
                    $"-tools={setup.arduinoTools}".Replace('\\', '/'),
                    $"-file='{project.name}.ino'",
                    },
            });

            var tasks = new tasksJson
            {
                tasks = tasklist,
                presentation = new Presentation()
            };
            return JsonConvert.SerializeObject(tasks, Formatting.Indented);
        }
    }
}

