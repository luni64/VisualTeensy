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

        CheckedPath coreBase { get; }
        bool localCore { get; set; }
        bool copyCore { get; set; }

        string core { get; }

        //string boardTxtPath { get; set; }
        //bool copyBoardTxt { get; set; }
        
        CheckedPath compilerBase { get; }
        string makefileExtension { get; set; }

        List<IBoard> boards { get; }
        IBoard selectedBoard { get; set; }

        List<Library> sharedLibs { get; }
        List<Library> localLibs { get; }
        //List<Library> projectLibs { get; }

        void parseBoardsTxt(string bt);

        string makefile { get; }
    }
}
