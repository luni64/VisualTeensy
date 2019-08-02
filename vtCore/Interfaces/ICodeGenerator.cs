using System.Collections.Generic;

namespace vtCore
{
    public interface ICodeGenerator
    {
        IReadOnlyList<ITask> getTasks(IProject project, LibManager libManager, SetupData setup);
    }
}
