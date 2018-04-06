using ERP_V2.Models;
using ERP_V2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace ERP_V2
{
    /// <summary>Current user Session Info</summary>
    public class UserInfo
    {
        public static string Id { get { return new SettingUserInfo().Info.Id; } }
        public static string Name { get { return new SettingUserInfo().Info.Name; } }
        public static string Permission { get { return new SettingUserInfo().Info.Permission; } }
        public static Language.Type LanguageType { get { return new SettingUserInfo().Info.LanguageType; } }

        //public static List<SYS_Permissions> PermissionsList { get { return new SettingUserInfo().Info.PermissionsList; } }

        //public static List<SYS_Permissions> AllPermissionsList { get { return new SettingUserInfo().Info.AllPermissionsList; } }
    }

    public class SettingUserInfo
    {
        public class DataStruct
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Permission { get; set; }
            public Language.Type LanguageType { get; set; }

            //public List<SYS_Permissions> PermissionsList { get; set; }

            //public List<SYS_Permissions> AllPermissionsList { get; set; }
        }

        public DataStruct Info
        {
            get
            {
                if (HttpContext.Current.Session["SettingUserInfo"] == null)
                {
                    HttpContext.Current.Session["SettingUserInfo"] = new DataStruct();
                }

                return (DataStruct)HttpContext.Current.Session["SettingUserInfo"];
            }
            set
            {
                HttpContext.Current.Session["SettingUserInfo"] = value;
            }
        }

    }
}