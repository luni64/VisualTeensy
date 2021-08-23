using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace vtCore.Implementation
{
    internal class GitHelpers
    {
        public static string getGitRemote(string folder)
        {
            folder += @"\.git\";
                        
            if (Repository.Discover(folder) == folder)  // folder is git repo
            {
                using (var repo = new Repository(folder))
                {   
                    return repo.Config.GetValueOrDefault<string>("remote.origin.url"); 
                }
            }
            return null;
        }
    }
}
