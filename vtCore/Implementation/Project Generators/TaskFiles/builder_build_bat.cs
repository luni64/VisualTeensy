using System.IO;
using System.Text;

namespace vtCore
{
    public static class builder_build_bat
    {
        public static string generate(IProject project, SetupData setup)
        {
            var buildBat = new StringBuilder();

            buildBat.Append("SET wd=%cd%\n");
            

            buildBat.Append(Path.Combine(Directory.GetCurrentDirectory(), "Assets\\Arduino-Builder ^"));
            buildBat.Append("-verbose=1 ^\n");
            buildBat.Append("-build-path=%wd%/.vsTeensy/build ^\n");
            buildBat.Append($"-hardware=\"{setup.arduinoBase}\\hardware ^\n");
            buildBat.Append($"-tools=\"{setup.arduinoBase}\\tools-builder\" -tools=\"{setup.arduinoBase}\\hardware\\tools\\avr\" ^\n");
            buildBat.Append($"-fqbn={project.selectedConfiguration.selectedBoard.fqbn} ");
            buildBat.Append($"-libraries={setup.arduinoBase}\\libraries ");
            buildBat.Append($"-libraries={setup.arduinoBase}\\hardware\\teensy\\avr\\libraries ");
            buildBat.Append($"-libraries={Helpers.getSketchbookFolder()}\\libraries ");
            buildBat.Append($"{project.name}.ino");

            return buildBat.ToString();
        }
    }
}
