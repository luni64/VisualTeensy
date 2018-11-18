using System;
using vtCore;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            SetupData setup = SetupData.getDefault();
            LibManager libManager = new LibManager(setup);
            var project = new Project(setup, libManager);

            project.newProject();


            foreach (var cfg in project.configurations)
            {
                Console.WriteLine(cfg.name);
                foreach (var board in cfg.boards)
                {
                    Console.WriteLine($" {board.name}");
                    foreach (var os in board.optionSets)
                    {
                        Console.WriteLine($"  {os.name}");
                        foreach (var option in os.options)
                        {
                            Console.WriteLine($"    {option.name}");
                        }
                    }
                }
            }


            while (!Console.KeyAvailable)
            {
                ;
            }
        }
    }
}
