using System;

namespace vtCore
{
    public interface ITask
    {
        string title { get; }
        string description { get; }
        string status { get; }
        Action action { get; }      
    }
}
