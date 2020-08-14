using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


namespace vtCore
{
    public class MruList
    {
        public List<string> projects;

        public MruList(int maxProject)
        {
            this.maxProjects = maxProject;
        }

        public void load(string mruString)
        {
            projects = mruString
                .Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(p => Directory.Exists(p))
                .Take(maxProjects)
                .ToList();
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
            if (!Directory.Exists(project)) return;

            if (projects.Contains(project))  // move entry to top
            {
                projects.Remove(project);
                projects.Insert(0, project);
            }
            else
            {
                projects.Insert(0, project);
                if (projects.Count > maxProjects)
                {
                    projects.RemoveAt(projects.Count - 1);
                }
            }
        }

        public void RemoveProject(string project)
        {
            projects.Remove(project);
        }

        private int maxProjects;
    }
}
