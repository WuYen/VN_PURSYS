using ERP_V2.Models;
using ERP_V2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_V2
{
    public static class CacheCommonDataModule
    {
        ///// <summary> 取得單位類別 </summary>
        ///// <returns></returns>
        //public static List<PublicWCFService.VW_Unit_Type> GetVW_Unit_Type()
        //{
        //    var list = new List<PublicWCFService.VW_Unit_Type>();

        //    var cacheData = CacheHelper.Get("UnitType");

        //    if (cacheData == null)
        //    {
        //        var unitType = new PublicWCFService.PublicWCFServiceClient().GetVW_Unit_TypeList(UserInfo.TokenModel);
        //        CacheHelper.Set("UnitType", unitType);
        //    }

        //    list = CacheHelper.Get("UnitType") as List<PublicWCFService.VW_Unit_Type>;

        //    return list;
        //}

        /// <summary> 取得倉庫 </summary>
        /// <returns></returns>
        //public static List<FA16A> GetFA16A()
        //{
        //    var list = new List<FA16A>();

        //    var cacheData = CacheHelper.Get("FA16A");

        //    if (cacheData == null)
        //    {
        //        var entity = new WSBI_DEVEntities();                
        //        var FA16A = entity.FA16A.ToList();
        //        CacheHelper.Set("FA16A", FA16A);
        //    }

        //    list = CacheHelper.Get("FA16A") as List<FA16A>;

        //    return list;
        //}

        /// <summary> 取得廠商 </summary>
        /// <returns></returns>
        public static List<BA01A> GetBA01A()
        {
            var list = new List<BA01A>();

            var cacheData = CacheHelper.Get("BA01A");

            if (cacheData == null)
            {
                var entity = new PURSysEntities();
                var BA01A = entity.BA01A.ToList();
                CacheHelper.Set("BA01A", BA01A);
            }

            list = CacheHelper.Get("BA01A") as List<BA01A>;

            return list;
        }


        ///// <summary> 取得聯絡人 </summary>
        ///// <returns></returns>
        //public static List<BA01A> GetBA01A()
        //{
        //    var list = new List<BA01A>();

        //    var cacheData = CacheHelper.Get("BA01A");

        //    if (cacheData == null)
        //    {
        //        var entity = new PURSysEntities();
        //        var BA01A = entity.BA01A.ToList();
        //        CacheHelper.Set("BA01A", BA01A);
        //    }

        //    list = CacheHelper.Get("BA01A") as List<BA01A>;

        //    return list;
        //}

        /// <summary> 取得幣別 </summary>
        /// <returns></returns>
        public static List<BA03A> GetBA03A()
        {
            var list = new List<BA03A>();

            var cacheData = CacheHelper.Get("BA03A");

            if (cacheData == null)
            {
                var entity = new PURSysEntities();
                var BA03A = entity.BA03A.ToList();
                CacheHelper.Set("BA03A", BA03A);
            }

            list = CacheHelper.Get("BA03A") as List<BA03A>;

            return list;
        }

        public static Dictionary<string, int> GetTypeDic()
        {
            //1.五金2.辦公用品3.模具4.廚房用品5.油品6.機台設備7.機台配件
            var list = new Dictionary<string, int>();

            var cacheData = CacheHelper.Get("TypeDic");

            if (cacheData == null)
            {
                //list.Add("五金", 1);
                //list.Add("辦公用品", 2);
                //list.Add("模具", 3);
                //list.Add("廚房用品", 4);
                //list.Add("油品", 5);
                //list.Add("機台設備", 6);
                //list.Add("機台配件", 7);
                //list.Add("其他", 8);
                //list.Add("藥水", 9);
                var entity = new PURSysEntities();
                var BA04AList = entity.BA04A.OrderBy(x => x.SEQ_NO).ToList();
                foreach (BA04A item in BA04AList)
                {
                    var text = "";
                    switch (UserInfo.LanguageType)
                    {
                        case Language.Type.VN:
                            text = item.TYP_VN;
                            break;
                        case Language.Type.CN:
                            text = item.TYP_CN;
                            break;
                        case Language.Type.TW:
                            text = item.TYP_TW;
                            break;
                        case Language.Type.EN:
                            text = item.TYP_US;
                            break;
                    }
                    list.Add(text, item.BA04A_ID);
                }

                CacheHelper.Set("TypeDic", list);
            }

            list = CacheHelper.Get("TypeDic") as Dictionary<string, int>;

            return list;
        }

        public static List<BA04A> _BA04AList;
        public static Dictionary<string, int> GetBA04A(Language.Type type)
        {
            if (_BA04AList == null)
            {
                ResetBA04AList();
            }
            var list = new Dictionary<string, int>();
            foreach (BA04A item in _BA04AList)
            {
                var text = "";
                switch (type)
                {
                    case Language.Type.VN:
                        text = item.TYP_VN;
                        break;
                    case Language.Type.CN:
                        text = item.TYP_CN;
                        break;
                    case Language.Type.TW:
                        text = item.TYP_TW;
                        break;
                    case Language.Type.EN:
                        text = item.TYP_US;
                        break;
                }
                list.Add(text, item.BA04A_ID);
            }
            return list;
        }

        public static void ResetBA04AList()
        {
            var entity = new PURSysEntities();
            _BA04AList = entity.BA04A.OrderBy(x => x.SEQ_NO).ToList();
        }

        private static List<FN01> _FN01List;

        public static List<FN01> GetFN01List
        {
            get
            {
                if (_FN01List == null)
                {
                    ResetFN01List();
                }
                return _FN01List;
            }
        }
        public static string GetPageName(string ACT)
        {
            var text = "";
            if (_FN01List == null)
            {
                ResetFN01List();
            }
            var item = _FN01List.FirstOrDefault(x => x.ACT_RF.Contains(ACT));
            switch (ERP_V2.UserInfo.LanguageType)
            {
                case Language.Type.VN:
                    text = item.NAM_VN;
                    break;
                case Language.Type.CN:
                    text = item.NAM_CN;
                    break;
                case Language.Type.TW:
                    text = item.NAM_TW;
                    break;
                case Language.Type.EN:
                    text = item.NAM_US;
                    break;
            }

            return text;
        }
        public static void ResetFN01List()
        {
            var entity = new PURSysEntities();
            _FN01List = entity.FN01.ToList();
        }

        /// <summary> 取得幣別 </summary>
        /// <returns></returns>
        //public static List<VW_BA01A> GetVW_BA01A()
        //{
        //    var list = new List<VW_BA01A>();

        //    var cacheData = CacheHelper.Get("VW_BA01A");

        //    if (cacheData == null)
        //    {
        //        var entity = new PURSysEntities();
        //        var VW_BA01A = entity.VW_BA01A.ToList();
        //        CacheHelper.Set("VW_BA01A", VW_BA01A);
        //    }

        //    list = CacheHelper.Get("VW_BA01A") as List<VW_BA01A>;

        //    return list;
        //}

        //from c in Categories
        //join o in Products on c.CategoryID equals o.CategoryID into ps
        //from o in ps.DefaultIfEmpty()
        //select new { c.CategoryName, o.ProductName }

        ///// <summary> 取得單位 </summary>
        ///// <returns></returns>
        //public static List<FA24A> GetFA24A()
        //{
        //    var list = new List<FA24A>();

        //    var cacheData = CacheHelper.Get("FA24A");

        //    if (cacheData == null)
        //    {
        //        var entity = new WSBI_DEVEntities();
        //        var FA24A = entity.FA24A.ToList();
        //        CacheHelper.Set("FA24A", FA24A);
        //    }

        //    list = CacheHelper.Get("FA24A") as List<FA24A>;

        //    return list;
        //}

        ///// <summary> 取得存放位置 </summary>
        ///// <returns></returns>
        //public static List<FA25WCFService.FA25A> GetFA25A()
        //{
        //    var list = new List<FA25WCFService.FA25A>();

        //    var cacheData = CacheHelper.Get("FA25A");

        //    if (cacheData == null)
        //    {
        //        var FA25A = new FA25WCFService.FA25WCFServiceClient().GetAList(UserInfo.TokenModel, null);
        //        CacheHelper.Set("FA25A", FA25A);
        //    }

        //    list = CacheHelper.Get("FA25A") as List<FA25WCFService.FA25A>;

        //    return list;
        //}

        /// <summary> 取得Product</summary>
        /// <returns></returns>
        public static List<BA02A> GetBA02A()
        {
            var list = new List<BA02A>();

            var cacheData = CacheHelper.Get("FA11A");

            if (cacheData == null)
            {
                var entity = new PURSysEntities();
                var BA02A = entity.BA02A.ToList();
                CacheHelper.Set("BA02A", BA02A);
            }

            list = CacheHelper.Get("BA02A") as List<BA02A>;

            return list;
        }

        /// <summary> 稅率 </summary>
        /// <returns></returns>
        public static Dictionary<string, float> GetTaxRate()
        {
            var list = new Dictionary<string, float>();

            var cacheData = CacheHelper.Get("TaxRate");

            if (cacheData == null)
            {
                list.Add("0%", 1);
                list.Add("5%", 1.05f);
                list.Add("10%", 1.1f);

                CacheHelper.Set("TaxRate", list);
            }

            list = CacheHelper.Get("TaxRate") as Dictionary<string, float>;

            return list;
        }


        /// <summary> 取得Product(FA11A) </summary>
        /// <returns></returns>
        //public static List<VW_FA11A> GetVW_FA11A()
        //{
        //    var list = new List<VW_FA11A>();

        //    var cacheData = CacheHelper.Get("VW_FA11A");

        //    if (cacheData == null)
        //    {
        //        var entity = new WSBI_DEVEntities();
        //        var VW_FA11A = entity.VW_FA11A.ToList();

        //        CacheHelper.Set("VW_FA11A", VW_FA11A);
        //    }

        //    list = CacheHelper.Get("VW_FA11A") as List<VW_FA11A>;

        //    return list;
        //}

        ///// <summary> 取得Product(FA11A) </summary>
        ///// <returns></returns>
        //public static List<Models.VW_StockAdjust_Type> GetStockAdjust_Type()
        //{
        //    var list = new List<Models.VW_StockAdjust_Type>();

        //    var cacheData = CacheHelper.Get("StockAdjust_Type");

        //    if (cacheData == null)
        //    {
        //        var entity = new Models.WSBI_DEVEntities();
        //        var StockAdjust_Type = entity.VW_StockAdjust_Type.ToList();
        //        CacheHelper.Set("StockAdjust_Type", StockAdjust_Type);
        //    }

        //    list = CacheHelper.Get("StockAdjust_Type") as List<Models.VW_StockAdjust_Type>;

        //    return list;
        //}
    }
}