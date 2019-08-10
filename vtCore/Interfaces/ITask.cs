using System;
using System.Threading.Tasks;

namespace vtCore.Interfaces
{
    public interface ITask
    {
        string title { get; }
        string description { get; }
        string status { get; }
        Func<Task> action { get; }      
    }
}
