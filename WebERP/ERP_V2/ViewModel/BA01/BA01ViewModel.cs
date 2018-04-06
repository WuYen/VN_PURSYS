using System.Collections.Generic;
using System.Web.Mvc;


namespace ERP_V2.ViewModels.BA01
{
    public class BA01ViewModel { }

    public class IndexViewModel
    {
        public int? FocusKey { get; set; }

        public bool IsSuccess { get; set; }
    }

    public class Edit
    {
        public int MasterKey { get; set; }
        public BA01AViewModel Master { get;set;}

        public List<BA01BViewModel> DetailList { get; set; }
    }
}