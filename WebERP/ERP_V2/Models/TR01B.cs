//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERP_V2.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TR01B
    {
        public int TR01B_ID { get; set; }
        public int TR01A_ID { get; set; }
        public int BA02A_ID { get; set; }
        public Nullable<decimal> PUR_PR { get; set; }
        public decimal PUR_QT { get; set; }
        public string REM_MM { get; set; }
        public System.DateTime CREATE_DATE { get; set; }
        public string CREATE_USER { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public string UPDATE_USER { get; set; }
        public string REL_NO { get; set; }
        public Nullable<decimal> CFN_QT { get; set; }
    }
}
