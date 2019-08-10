using Newtonsoft.Json;
using vtCore.Interfaces;

namespace vtCore
{
    static public class ProjectSettings
    {
        static public string generate(IProject project)
        {
            return JsonConvert.SerializeObject(new ProjectTransferData(project), Formatting.Indented);
        }
    }

}


