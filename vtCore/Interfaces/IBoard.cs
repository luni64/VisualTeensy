using System;
using System.Collections.Generic;

namespace vtCore.Interfaces
{
    public interface IBoard
    {
        string name { get; }
        string id { get; }
        string fqbn { get; }


        IEnumerable<IOptionSet> optionSets { get; }        

        string core { get; }
        Dictionary<String, String> getAllOptions();        
    }
}
