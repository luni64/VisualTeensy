using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using vtCore.Interfaces;

namespace vtCore
{
    static internal class BoardsTxt
    {
        public static IEnumerable<IBoard> parse(string fileName)
        {
            if (!File.Exists(fileName)) { return Enumerable.Empty<IBoard>(); }

            try
            {
                var lines = readLines(fileName);

                var menuEntries = lines.Where(l => l.key[0] == "menu");
                var menus = menuEntries.Select(e => new Menu(e.key[1], e.value));

                var boardEntries = lines.Where(l => l.key[0] != "menu").ToLookup(k => k.key[0]); // entries grouped by boardID

                return boardEntries.Select(e => new Board(menus, e));
            }
            catch 
            {
                return Enumerable.Empty<IBoard>();                
            }
        }

        private static List<Entry> readLines(string filename)
        {
            var lines = new List<Entry>();
            using (TextReader reader = new StreamReader(filename))
            {
                while (true)
                {
                    String line = reader.ReadLine();
                    if (line == null)
                    {
                        break;
                    }

                    var cmdPos = line.IndexOf('#');
                    if (cmdPos != -1)
                    {
                        line = line.Substring(0, cmdPos).Trim();
                    }

                    if (line == "")
                    {
                        continue;
                    }

                    lines.Add(new Entry(line));
                }
            }
            return lines;
        }
    }
}
