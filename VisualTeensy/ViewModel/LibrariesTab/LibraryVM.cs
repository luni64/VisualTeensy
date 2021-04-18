using System.Collections.Generic;
using vtCore;
using System.Linq;
using vtCore.Interfaces;
using System;

namespace ViewModel
{
    public class LibraryVM : BaseViewModel
    {
        public string name => selectedVersion?.name;
        public string description => selectedVersion?.sentence;
        public IEnumerable<ILibrary> versions { get; }
        //public string parentRepository { get; }

        public ILibrary selectedVersion { get; set; }


        public LibraryVM(IEnumerable<ILibrary> lib/*, string repositoryName*/)
        {
            //parentRepository = string.IsNullOrWhiteSpace(repositoryName) ? "?" : repositoryName;
            this.versions = lib.OrderByDescending(v => v.v).ToList();


            selectedVersion = this.versions.FirstOrDefault();
        }

        public override string ToString() => name;



    }
}

    //public class SortPara : IComparable<SortPara>
    //{
    //    public List<int> numbers { get; set; }
    //    public string strNumbers { get; set; }
    //    public SortPara(string strNumbers)
    //    {
    //        this.strNumbers = strNumbers;
    //        numbers = strNumbers.Split(new char[] { '.' }).Select(x => int.Parse(x)).ToList();

    //    }
    //    public int CompareTo(SortPara other)
    //    {
    //        int shortest = this.numbers.Count < other.numbers.Count ? this.numbers.Count : other.numbers.Count;
    //        int results = 0;
    //        for (int i = 0; i < shortest; i++)
    //        {
    //            if (this.numbers[i] != other.numbers[i])
    //            {
    //                results = this.numbers[i].CompareTo(other.numbers[i]);
    //                break;
    //            }
    //        }
    //        return results;
    //    }
    //}


