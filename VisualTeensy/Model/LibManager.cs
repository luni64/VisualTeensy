using System;
using System.Collections.Generic;

namespace VisualTeensy.Model
{
    public class LibManager
    {
        public List<PjrcLibs> repositories { get; }


        public LibManager(ProjectData project, SetupData setup)
        {
            repositories = new List<PjrcLibs>();
                repositories.Add(new PjrcLibs(project, setup));

            //if (!String.IsNullOrWhiteSpace(data.setup.arduinoBase))
            //{
            //}
        }
    }
}

