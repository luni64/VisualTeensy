using System.Collections.Generic;

namespace Board2Make.Model
{
    public class vsBoard
    {
        public string type { get; set; }
        public Dictionary<string, string> options { get; set; } = new Dictionary<string, string>();        
    }

    public class vsData
    {
        public vsBoard board { get; set; }
        public string compiler { get; set; }
    }
}
