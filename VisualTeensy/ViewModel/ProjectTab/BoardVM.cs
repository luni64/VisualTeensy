using vtCore;
using System;
using System.Collections.Generic;
using System.Linq;


namespace ViewModel
{
    public class BoardVM : BaseViewModel
    {
        public IBoard board { get; }
        public String boardName => board.name;
        public List<OptionSetVM> optionSetVMs { get; }

        //public String makefile
        //{
        //    get => _makefile;
        //    set => SetProperty(ref _makefile, value);
        //}
        //String _makefile;

        public BoardVM(IBoard board)
        {
            this.board = board;
            optionSetVMs = new List<OptionSetVM>(board.optionSets.Select(b => new OptionSetVM(b)));
        }
    }
}
