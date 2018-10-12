using System.Linq;
using ViewModel;

namespace VisualTeensy.Model
{
    class libTransfer
    {
        public string name { get; set; }
    }

    public interface IRepository
    {
        string name { get; }
        ILookup<string, Library> libraries { get; }
    }


    public class RepositoryIndexJson : IRepository
    {
        public string name { get; }
        public string path { get; set; }
        public ILookup<string ,Library> libraries { get; }
      
        public RepositoryIndexJson(string name, string path)
        {
            this.name = name;
            this.path = path;

            libraries = LibraryReader.parseLibrary_Index_Json(path);
        }      
    }

    public class RepositoryLocal : IRepository
    {
        public string name { get; }
        public string path { get; }
        public ILookup<string, Library> libraries { get; }
                              
        public RepositoryLocal(string name, string path = null)
        {
            this.name = name;
            this.path = path;            
            libraries = LibraryReader.parseLibraryLocal(path);
        }      
    }
}


