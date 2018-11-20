using System;
using System.Linq;
using vtCore;

namespace VisualTeensyCLI
{
    class Program
    {
        public static event EventHandler<ConsoleKey> menuEvent;

        static void onMenuEvent(ConsoleKey key)
        {
            menuEvent?.Invoke(null, key);
        }

        static void Main(string[] args)
        {
            SetupData setup = SetupData.getDefault();
            LibManager libManager = new LibManager(setup);
            var project = new Project(setup, libManager);

            project.newProject();


            var cfg = project.configurations.First();

            Console.CursorVisible = false;
            var mainMenu = new Menu(1, 1);
            var detailsMenu = new Menu(25, 1);

            mainMenu.Add("Board", null);


            foreach (var os in cfg.boards[0].optionSets)
            {
                mainMenu.Add(os.name, os);
            }



            mainMenu.display();

            mainMenu.selectedIndex = 0;

          
            while (true)
            {
                var keyInfo = Console.ReadKey(true);
                onMenuEvent(keyInfo.Key);


                var os = mainMenu.selectedObject as IOptionSet;
                if (os != null)
                {
                    detailsMenu.Clear();
                    foreach (var option in os.options)
                    {
                        detailsMenu.Add(option.name, option);
                    }
                    detailsMenu.display();
                }
            }

        }


        static void genOptions(int index)
        {

        }
    }
}
