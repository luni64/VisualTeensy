using System;
using System.Collections.Generic;
using System.Linq;

using static System.Console;

namespace VisualTeensyCLI
{
    class Menu
    {
        public Menu(int x, int y)
        {
            this.x = x;
            this.y = y;

            Program.menuEvent += Program_menuEvent1;
        }

        private void Program_menuEvent1(object sender, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.DownArrow:
                    next();
                    break;
                case ConsoleKey.UpArrow:
                    prev();
                    break;

                case ConsoleKey.Escape:
                    return;
            }
        }

       

        public void Clear()
        {
            var empty = new String(' ', width);

            for (int i = 0; i < entries.Count; i++)
            {
                showEntry(empty, i, false);
            }

            entries.Clear();
        }


        public void next()
        {
            if (selectedIndex == entries.Count - 1)
            {
                selectedIndex = 0;
            }
            else
            {
                selectedIndex = selectedIndex + 1;
            }
        }

        public void prev()
        {
            if (selectedIndex == 0)
            {
                selectedIndex = entries.Count-1;
            }
            else
            {
                selectedIndex = selectedIndex - 1;
            }
        }

        public int selectedIndex
        {
            get => _selectedIndex;
            set
            {
                if (value != _selectedIndex)
                {
                    showEntry(_selectedIndex, false);

                    _selectedIndex = value;
                    showEntry(_selectedIndex, true);
                }
            }
        }

        public object selectedObject => entries[selectedIndex].o;

        int _selectedIndex = -1;

        public void Add(string entry, object obj)
        {
            entries.Add(new entry
            {
                e = entry,
                o = obj
            });
        }

        public void select(int index)
        {
            SetCursorPosition(x, y + index);
        }

        public void showEntry(int index, bool highlight)
        {
            if (index < 0 || index >= entries.Count)
            {
                return;
            }

            showEntry(entries[index].e,index,highlight);
        }


        public void showEntry(string s, int index, bool highlight)
        {
            SetCursorPosition(x, y + index);

            BackgroundColor = highlight ? ConsoleColor.White : ConsoleColor.Black;
            ForegroundColor = highlight ? ConsoleColor.Red : ConsoleColor.White;

            WriteLine(s.PadRight(width).PadLeft(width + 1, highlight ? '>' : ' '));
        }

        public void display()
        {
            width = entries.Max(e => e.e.Length)+1;

            int r = y;

            for (int i = 0; i < entries.Count; i++)
            {
                showEntry(i, false);
            }
        }


        List<entry> entries = new List<entry>();

        int x, y;
        int width;


        private class entry
        {
            public string e { get; set; }
            public object o { get; set; }
        }
    }
}
