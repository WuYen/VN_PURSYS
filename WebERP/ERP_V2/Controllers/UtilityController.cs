using ERP_V2.Models;
using ERP_V2.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;

namespace ERP_V2.Controllers
{
    public class UtilityController : Controller
    {
        //private WSBI_DEVEntities _Entity = new WSBI_DEVEntities();
        // GET: Utility
        public ActionResult ProductPopup(string popupElementID)
        {
            var model = new ProductSelectViewModel
            {
                PopupElementID = popupElementID ?? "",
                //BA02AList = CacheCommonDataModule.GetBA02A()
                BA02AList = GetData("")
            };
            return PartialView("_ProductPopup", model);
        }

        public ActionResult ProductGrid(string selectedItem, string BA01A_ID, int? ignoreID, bool FirstPage = false)
        {
            var itemList = CacheCommonDataModule.GetBA02A();

            if (selectedItem != null && selectedItem.Length > 0)
            {
                ViewData["selectedItem"] = selectedItem;
            }
            else
            {
                ViewData["FirstPage"] = FirstPage;
            }

            return PartialView("_ProductGrid", GetData(BA01A_ID));
        }

        private List<ProductSelectModel> GetProductSelectModel(string BA01A_ID)
        {
            var dataList = new List<ProductSelectModel>();

            var productList = CacheCommonDataModule.GetBA02A();
            var incorporationList = CacheCommonDataModule.GetBA01A();

            PURSysEntities entity = new PURSysEntities();

            foreach (var item in productList)
            {
                var data = new ProductSelectModel()
                {
                    BA02A_ID = item.BA02A_ID,
                    ITM_NO = item.ITM_NO,
                    ITM_NM = item.ITM_NM,
                    //BA01A_ID = BA02B.BA01A_ID,
                    //INC_NM = incorporationList.First(x => x.BA01A_ID == BA02B.BA01A_ID).INC_NM,
                    //PUR_PR = BA02B.PUR_PR
                };
                data.TYP_ID = item.TYP_ID;
                dataList.Add(data);
                //var tempData = entity.BA02B.Where(x => x.BA02A_ID == item.BA02A_ID).ToList();
                //foreach (var BA02B in tempData)
                //{
                //    var data = new ProductSelectModel()
                //    {
                //        BA02A_ID = item.BA02A_ID,
                //        ITM_NO = item.ITM_NO,
                //        ITM_NM = item.ITM_NM,
                //        BA01A_ID = BA02B.BA01A_ID,
                //        INC_NM = incorporationList.First(x=>x.BA01A_ID == BA02B.BA01A_ID).INC_NM,
                //        PUR_PR = BA02B.PUR_PR
                //    };
                //    dataList.Add(data);
                //}
            }
            return dataList;
        }

        private DataTable GetData(string BA01A_ID)
        {
            var ds = new DataSet();

            using (var _Entity = new PURSysEntities())
            {
                var conn = _Entity.Database.Connection;
                var connectionState = conn.State;
                if (connectionState != ConnectionState.Open)
                {
                    conn.Open();
                }

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = $"select A.BA02A_ID,A.ITM_NO,A.ITM_NM,A.TYP_ID,B.PUR_PR,A.ITM_SP from BA02A as A left join BA02B as B on A.BA02A_ID = B.BA02A_ID where B.BA01A_ID = '{BA01A_ID}'";
                    cmd.CommandTimeout = 60;
                    using (var reader = cmd.ExecuteReader())
                    {
                        do
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            ds.Tables.Add(dt);
                        }
                        while (!reader.IsClosed);
                    }
                }
            }
            return ds.Tables[0];
        }

        public ActionResult KeepAlive()
        {
            try
            {
                var data = Session["SettingUserInfo"] as SettingUserInfo.DataStruct;

                if (data == null || string.IsNullOrWhiteSpace(UserInfo.Id))
                {
                    return Content("Dead");
                }
                else
                {
                    var newData = new SettingUserInfo.DataStruct()
                    {
                        Id = data.Id,
                        Name = data.Name,
                        LanguageType = data.LanguageType,
                        Permission = data.Permission
                    };

                    Session["SettingUserInfo"] = newData;
                    return Content("Alive");
                }
            }
            catch (System.Exception ex)
            {
                return Content("Dead");
            }
        }
    }
}