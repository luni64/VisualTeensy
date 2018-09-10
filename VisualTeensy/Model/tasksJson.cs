using System.Collections.Generic;

namespace Board2Make.Model
{

    class Group
    {
        public string kind => "build";
        public bool isDefault => true;
    }

    class ProblemMatcher
    {
        public string Base => "$gcc";
    }

    class Presentation
    {
        public bool echo => true;
        public string reveal => "always";
        public bool focus => false;
        public string panel => "shared";
        public bool showReuseMessage => false;
    }

    class Task
    {
        public string label { get; set; }
        public Group group { get; set; }
        public string command { get; set; }
        public List<string> args { get; set; }
    }

    class tasksJson
    {
        public string version => "2.0.0";
        public string type => "shell";
        public Presentation presentation { get; set; }
        public string problemMatcher => "$gcc";
        public List<Task> tasks;
    }
}
