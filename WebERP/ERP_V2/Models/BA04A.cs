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
    
    public partial class BA04A
    {
        public int BA04A_ID { get; set; }
        public string TYP_TW { get; set; }
        public string TYP_CN { get; set; }
        public string TYP_US { get; set; }
        public string TYP_VN { get; set; }
        public Nullable<int> SEQ_NO { get; set; }
        public System.DateTime CREATE_DATE { get; set; }
        public string CREATE_USER { get; set; }
        public Nullable<System.DateTime> UPDATE_DATE { get; set; }
        public string UPDATE_USER { get; set; }
    }
}
