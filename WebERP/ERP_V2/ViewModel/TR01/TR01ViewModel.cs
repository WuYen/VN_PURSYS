using ERP_V2.Models;
using System.Collections.Generic;
using System.Web.Mvc;


namespace ERP_V2.ViewModels.TR01
{
    public class TR01ViewModel { }

    public class IndexViewModel
    {
        public int? FocusKey { get; set; }

        public bool IsSuccess { get; set; }
    }

    public class Edit
    {
        public int MasterKey { get; set; }
        public TR01AViewModel Master { get;set;}

        public MasterForm MasterForm { get; set; }
        //public BA01.BA01BViewModel

        public List<TR01BViewModel> DetailList { get; set; }
    }
    public class MasterForm
    {
        public TR01AViewModel Master { get; set; }
        public BA01B Contact { get; set; }
    }
}