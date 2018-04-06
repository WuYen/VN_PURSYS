using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_V2.ViewModels.TR01M
{
    public class TR01BModel : ERP_V2.ViewModels.TR01V2.TR01BViewModel
    {
        public decimal ARR_QT_Sum { get; set; }
        public string ARR_ST { get; set; }
        public string ITM_SP { get; set; }
        public decimal ReceiveMY { get; set; }
    }
}