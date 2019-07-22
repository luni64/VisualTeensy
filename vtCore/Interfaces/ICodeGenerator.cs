using System;
using System.Threading.Tasks;

namespace vtCore
{
    public interface ICodeGenerator
    {
         Task generate(IProject project, LibManager libManager, SetupData setup, IProgress<string> progressHandler);
    }
}
