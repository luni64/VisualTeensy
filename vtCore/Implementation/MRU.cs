using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace vtCore
{
    public class MRU
    {
        public MRU(uint maxProject)
        {
            this.maxProjects = maxProject;
        }

        public void load(string mruString)
        {
            projects = mruString.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            while (projects.Count > maxProjects)
            {
                projects.RemoveAt(0);
            }
        }

        override public string ToString()
        {
            if (projects.Count == 0) return "";

            StringBuilder mruString = new StringBuilder(projects[0]);
            foreach (var project in projects.Skip(1))
            {
                mruString.Append($"|{project}");
            }
            return mruString.ToString();
        }

        public void AddProject(string project)
        {
            projects.Add(project);
            while (projects.Count > maxProjects)
            {
                projects.RemoveAt(0);
            }
        }

        public void RemoveFolder(string folder)
        {
            projects.Remove(folder);
        }

        private uint maxProjects;
        private List<string> projects = new List<string>();
    }
}
