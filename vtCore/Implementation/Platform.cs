using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace vtCore
{
    class Platform
    {
        public string name { get; }
        public override string ToString() => name;
            

        public string boards_txt_path { get; }
        public string platform_txt_path { get; }
        public string version { get; }

        public bool isValid { get; } = false;

        public List<IBoard> boards { get; }


        public Platform(string path)
        {
            boards_txt_path = Path.Combine(path, "boards.txt");
            platform_txt_path = Path.Combine(path, "platform.txt");

            if (File.Exists(boards_txt_path) && File.Exists(platform_txt_path))
            {
                isValid = true;
                name = Path.GetFileName(path);

            }
            else
            {
                foreach (var folder in Directory.GetDirectories(path))
                {
                    boards_txt_path = Path.Combine(folder, "boards.txt");
                    platform_txt_path = Path.Combine(folder, "platform.txt");

                    var b = File.Exists(boards_txt_path);

                    if (File.Exists(boards_txt_path) && File.Exists(platform_txt_path))
                    {
                        isValid = true;
                        name = Path.GetFileName(path);
                        break;
                    }
                }
            }

            if(isValid)
            {
                boards = BoardsTxt.parse(boards_txt_path).ToList();
            }

        }
    }


    class Package
    {
        public string name { get; }
        public override string ToString()
        {
            return name;
        }

        public IEnumerable<Platform> platforms { get; }

        public bool isValid { get; } = false;


        public Package(string path)
        {
            var hardwarePath = Path.Combine(path, "hardware");

            if (Directory.Exists(hardwarePath))
            {
                platforms = Directory.GetDirectories(hardwarePath)
                    .Select(a => new Platform(a))
                    .Where(a => a.isValid)
                    .ToList();

                isValid = platforms.Any();
                name = Path.GetFileName(path);
            }

        }
    }
}
