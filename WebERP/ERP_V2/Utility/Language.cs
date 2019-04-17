using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_V2.Utility
{
    public partial class Language
    {
        public enum Type { VN, CN, TW, EN }
        public static readonly Dictionary<Type, LanguageObject> TypeStr = new Dictionary<Type, LanguageObject>()
        {
            { Type.VN,new LanguageObject{ DisplayName="Việt Ngữ", Value="vi-VN" } },
            { Type.CN,new LanguageObject{ DisplayName="中文", Value="zh-CN" } },
            //{ Type.TW,new LanguageObject{ DisplayName="中文(繁)", Value="zh-TW" } },
            { Type.EN,new LanguageObject{ DisplayName="English", Value="en-US" } },
         };

        public class LanguageObject
        {
            public string DisplayName { get; set; }
            public string Value { get; set; }
        }

        public static readonly string VN = "vi-VN";
        public static readonly string TW = "zh-TW";
        public static readonly string US = "en-US";
        public static readonly string CN = "zh-CN";
    }

    public partial class Language
    {
        public static string UserName { get { return "使用者名稱"; } }
        public static string Account { get { return "帳號"; } }
        public static string Enabled { get { return "使否啟用"; } }
        public static string Remark { get { return "備註說明"; } }
        public static string Password { get { return "密碼"; } }
        public static string Required { get { return "必填"; } }
        public static string DataHasDeleted { get { return "資料已被其他使用者刪除"; } }

        public static string GroupNameCH { get { return "中文名稱"; } }
        public static string GroupNameVN { get { return "越文名稱"; } }
        
        public static string GroupNameEN { get { return "英文名稱"; } }
    }
}