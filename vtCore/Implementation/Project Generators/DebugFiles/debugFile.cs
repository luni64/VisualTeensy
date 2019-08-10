using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using vtCore.Interfaces;

namespace vtCore
{    
    public class DebugFile
    {
        public static Dictionary<string, (string, string)> seggerDebugTargets = new Dictionary<string, (string, string)>
        {//    Board ID     Segger Device     SVD File
            { "teensyLC", ("MKL26Z64xxx4",   "xx1.svd") },
            { "teensy31", ("MK20DX256xxx7",  "xx2.svd") },
            { "teensy35", ("MK64FX512xxx12", "MK64F12.svd") },
            { "teensy36", ("MK66FX1M0xxx18", "xx3.svd") },
            { "teensy40", ("MIMXRT1062xxx6A","xx4.svd") },
        };
        
        public static string generate(IProject project, SetupData setup)
        {
            if (project.debugSupport == DebugSupport.none) return null;
                       
            switch (project.target)
            {
                case Target.vsCode:
                    return DebugFile_vsCode.generate(project, setup);

                //case Target.atom:
                //    return TaskFile_ATOM.generate(project, setup);

                //case Target.sublimeText:
                //    return "TBD";

                //case Target.vsFolder:
                //    return "TBD";

                default:
                    return "Error";
            }
        }
    }
}
