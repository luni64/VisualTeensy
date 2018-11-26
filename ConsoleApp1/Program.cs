using System;
using System.Linq;
using vtCore;

namespace VisualTeensyCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            SetupData setup = SetupData.getDefault();
                                   
            var libManager = Factory.makeLibManager(setup);
            var project = Factory.makeProject(setup, libManager);

            project.newProject();

            var boards = project.selectedConfiguration.boards;

            foreach(var board in boards)
            {
                Console.WriteLine(board.name);
            }
            
            Console.WriteLine("\nMakefile-----------------------------------------\n");

            project.selectedConfiguration.selectedBoard = boards.Last();
            Console.WriteLine(Makefile.generate(project, libManager, setup));
                

            while (!Console.KeyAvailable) ;

        }


        static void genOptions(int index)
        {

        }
    }
}
