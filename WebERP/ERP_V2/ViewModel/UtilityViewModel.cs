using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERP_V2.Models;
using System.Data;

namespace ERP_V2.ViewModels
{
    public class UtilityViewModel
    {
    }

    public class ProductSelectViewModel
    {
        public string PopupElementID { get; set; }
        public DataTable BA02AList { get; set; }
    }

    public class ProductSelectModel
    {
        public int BA02A_ID { get; set; }
        public string ITM_NO { get; set; }
        public string ITM_NM { get; set; }
        public string ITM_SP { get; set; }
        public int? TYP_ID { get; set; }
        public decimal PUR_PR { get; set; }
        public int BA01A_ID { get; set; }

        public string INC_NM { get; set; }
    }

}