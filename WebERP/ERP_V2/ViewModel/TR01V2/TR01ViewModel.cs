using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_V2.ViewModel.TR01V2
{
    public class TR01ViewModel
    {
    }
    public class SearchViewModel
    {
        //單號、日期、採購廠商(BA01)
        public string S_PUR_NO { get; set; }

        public int S_BA01A_ID { get; set; }

        public DateTime? DTBeg { get; set; }
        public string StrDTBeg
        {
            get
            {
                string result = "";
                if (this.DTBeg.HasValue)
                {
                    result = this.DTBeg.Value.ToString("yyyyMMdd");
                }
                return result;
            }
        }

        public DateTime? DTEnd { get; set; }
        public string StrDTEnd
        {
            get
            {
                string result = "";
                if (this.DTEnd.HasValue)
                {
                    result = this.DTEnd.Value.ToString("yyyyMMdd");
                }
                return result;
            }
        }
    }
}