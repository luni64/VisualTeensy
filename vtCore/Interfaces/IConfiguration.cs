using System.Collections.Generic;

namespace vtCore.Interfaces
{
    public enum SetupTypes
    {
        quick, expert
    }

    public interface IConfiguration
    {
        bool isOk { get; }
             
        SetupTypes setupType { get; set; }
        string name { get; set; }
        string guid { get; }
        
        CheckedPath coreBase { get; }       
        LibStrategy coreStrategy { get; set; }
        string core { get; }
               
        StdLibType stdLib { get; set; }

        string compiler { get; }
        CheckedPath compilerBase { get; }
        string makefileExtension { get; set; }

        List<IBoard> boards { get; }
        IBoard selectedBoard { get; set; }
        List<IProjectLibrary> sharedLibs { get; }
        List<IProjectLibrary> localLibs { get; }
       
        void parseBoardsTxt(string bt);

        string makefile { get; }
    }
}
