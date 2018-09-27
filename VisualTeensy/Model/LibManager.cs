using System;
using System.Collections.Generic;

namespace VisualTeensy.Model
{
    public class LibManager
    {
        public List<PjrcLibs> repositories { get; }


        public LibManager(SetupData data)
        {
            repositories = new List<PjrcLibs>();

            if (!String.IsNullOrWhiteSpace(data.arduinoBase))
            {
                repositories.Add(new PjrcLibs(data));
            }
        }
    }
}

