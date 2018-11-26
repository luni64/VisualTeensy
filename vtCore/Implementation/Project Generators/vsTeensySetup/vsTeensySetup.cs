using Newtonsoft.Json;

namespace vtCore
{
    static public class ProjectSettings
    {
        static public string generate(IProject project)
        {
            return JsonConvert.SerializeObject(new projectTransferData(project), Formatting.Indented);
        }
    }

}


