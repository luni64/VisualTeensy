using System.Collections.Generic;

namespace vtCore
{
    public interface IOptionSet
    {       
        string name { get; }

        IEnumerable<IOption> options { get; }
        IOption selectedOption { get; set; }
    }
}
