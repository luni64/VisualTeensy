using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualTeensy.Model
{
    public class LibManager
    {
        public List<Library> libraries { get; }

        public LibManager(SetupData data)
        {
            libraries = new List<Library>();

            if (!String.IsNullOrWhiteSpace(data.arduinoBase))
            {
                var libPath = Path.Combine(data.arduinoBase, "hardware", "teensy", "avr", "libraries");
                getLibs(@"C:\Arduino\arduino-1.8.5\hardware\teensy\avr\libraries");
            }

        }

        void getLibs(string folder)
        {
            foreach (var dir in Directory.GetDirectories(folder))
            {
                libraries.Add(new Library() { name = Path.GetFileName(dir) });
            }
        }
    }
}
