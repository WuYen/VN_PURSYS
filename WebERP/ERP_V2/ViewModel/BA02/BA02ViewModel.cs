using System.Collections.Generic;
using System.Web.Mvc;


namespace ERP_V2.ViewModels.BA02
{
    public class BA02ViewModel { }

    public class IndexViewModel
    {
        public int? FocusKey { get; set; }

        public bool IsSuccess { get; set; }
    }

    public class Edit
    {
        public int MasterKey { get; set; }
        public BA02AViewModel Master { get;set;}

        public List<BA02BViewModel> DetailList { get; set; }
    }
}