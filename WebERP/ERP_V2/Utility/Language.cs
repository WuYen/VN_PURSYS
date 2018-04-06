using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_V2.Utility
{
    public class Language
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
}