using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_V2.Reports.TR01
{
    public class TR01ReportModel
    {
        /// <summary> 公司 </summary>
        public string Company { get; set; }
        /// <summary> 供應商 </summary>
        public string Provider { get; set; }

        /// <summary> 聯絡人 </summary>
        public string Contact { get; set; }

        /// <summary> 地址 </summary>
        public string Address { get; set; }
        /// <summary> 電話 </summary>
        public string TelNo { get; set; }
        /// <summary> 傳真 </summary>
        public string FAX { get; set; }
        /// <summary> 訂單編號 </summary>
        public string OrderNO { get; set; }
        /// <summary> 日期 </summary>        
        public string Date { get; set; }
        /// <summary> 商品 </summary>
        public List<Detail> DetaiList { get; set; }

        public decimal TotalSummaryPrice { get; set; }

        public decimal TaxRate { get; set; }

        public decimal Price { get; set; }

        public string MoneyInChinese { get; set; }

        public string Unit { get; set; }
    }

    public class Detail
    {
        public int SeqNO { get; set; }

        public string ItemNO { get; set; }
        public string Spec { get; set; }
        public string Date { get; set; }
        public decimal Qty { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string RelativeNO { get; set; }

        public string REM { get; set; }

    }
}