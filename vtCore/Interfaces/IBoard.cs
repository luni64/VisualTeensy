using System;
using System.Collections.Generic;

namespace vtCore
{
    public interface IBoard
    {
        String name { get; }

        IEnumerable<IOptionSet> optionSets { get; }
        

        string core { get; }
        Dictionary<String, String> getAllOptions();        
    }
}
