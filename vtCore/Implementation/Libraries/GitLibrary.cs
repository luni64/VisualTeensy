using LibGit2Sharp;
using log4net;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using vtCore.Implementation;
using vtCore.Interfaces;

namespace vtCore
{
    internal class GitLibrary : Library, IProjectLibrary
    {
        public enum gitError
        {
            OK, folderExists, folderNoGitRepo, wrongGitRepo, canNotClone,
        }

        public GitLibrary(string repoURL, string target)
        {
            this.url = repoURL;
            this.targetFolder = target;

            alreadyCloned = (url == GitHelpers.getGitRemote(target));
        }

        public bool alreadyCloned { get; }
        public string targetFolder { get; set; }
        public string url { get; set; }

        public async Task<GitError> clone()
        {
            if (alreadyCloned) return GitError.OK;

            try
            {
                if (Directory.Exists(targetFolder))
                {
                    log.Info($"{targetFolder} doesn't contain a valid repository -> delete");
                    Directory.Delete(targetFolder, true);  

                }
                await Task.Run(() => Repository.Clone(url, targetFolder));
                return GitError.OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                return GitError.Unexpected;
            }
        }
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
    }
}

