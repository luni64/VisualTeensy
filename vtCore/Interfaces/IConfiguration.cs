using System.Collections.Generic;

namespace vtCore
{
    public enum SetupTypes
    {
        quick, expert
    }

    public interface IConfiguration
    {
        SetupTypes setupType { get; set; }
        string name { get; set; }
        string guid { get; }
        
        string coreBase { get; set; }
        bool copyCore { get; set; }

        string boardTxtPath { get; set; }
        bool copyBoardTxt { get; set; }
        
        string compilerBase { get; set; }
        string makefileExtension { get; set; }

        List<IBoard> boards { get; }
        IBoard selectedBoard { get; set; }

        List<Library> sharedLibs { get; }
        List<Library> localLibs { get; }

        void parseBoardsTxt(string bt);

        string makefile { get; }
    }
}
