using System.Collections.Generic;

namespace vtCore.Interfaces
{
    public interface ICodeGenerator
    {
        IReadOnlyList<ITask> getTasks(IProject project, LibManager libManager, SetupData setup);
    }
}
