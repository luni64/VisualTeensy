using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VisualTeensy.Model
{
    static class FileContent
    {
        public static IEnumerable<Board> parse(string fileName)
        {
            IEnumerable<Board> boards = Enumerable.Empty<Board>();
            IEnumerable<Menu> menus = Enumerable.Empty<Menu>();

            if (!File.Exists(fileName)) return boards;
           
            try
            {
                var lines = readLines(fileName);

                var menuEntries = lines.Where(l => l.key[0] == "menu");
                menus = menuEntries.Select(e => new Menu(e.key[1], e.value));

                var boardEntries = lines.Where(l => l.key[0] != "menu").ToLookup(k => k.key[0]); // entries grouped by boardID
                boards = boardEntries.Select(e => new Board(menus, e));                               
            }
            catch
            {
                // parse error, return values will be empty enumerables
            }

            return boards;
        }

        private static List<Entry> readLines(string filename)
        {
            var lines = new List<Entry>();
            using (TextReader reader = new StreamReader(filename))
            {
                while (true)
                {
                    String line = reader.ReadLine();
                    if (line == null) break;

                    var cmdPos = line.IndexOf('#');
                    if (cmdPos != -1)
                    {
                        line = line.Substring(0, cmdPos);
                    }

                    if (line == "") continue;
                    lines.Add(new Entry(line));
                }
            }
            return lines;

        }
    }
}
