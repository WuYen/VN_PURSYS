using DevExpress.Web.Mvc;
using ERP_V2.Models;
using ERP_V2.Utility;
using ERP_V2.ViewModel.TR01V2;
using ERP_V2.ViewModels.TR01V2;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace ERP_V2.Controllers
{
    public class TR01V2Controller : Controller
    {
        private TR01Service _Service = new TR01Service();
        // GET: TR01V2
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SearchGrid(SearchViewModel model)
        {
            List<TR01AViewModel> list = new List<TR01AViewModel>();
            using (var ctx = new PURSysEntities())
            {
                string sqlCmd = "Select * from TR01A ";
                string filterStr = "Where ";
                foreach (PropertyInfo propertyInfo in model.GetType().GetProperties())
                {
                    var value = propertyInfo.GetValue(model, null);
                    if (value != null && value.ToString().Length > 0)
                    {
                        var name = propertyInfo.Name;

                        if (name.Contains("StrDT"))
                        {
                            if (name.Contains("Beg"))
                            {
                                filterStr += "PUR_DT" + " >= '" + value + "' and ";
                            }
                            else
                            {
                                filterStr += "PUR_DT" + " <= '" + value + "' and ";
                            }
                        }
                        else if (propertyInfo.PropertyType == typeof(int))
                        {
                            var key = value.ToString();
                            if (key != "0")
                            {
                                filterStr += name.Remove(0, 2) + " = '" + key + "' and ";
                            }
                        }
                        else if (propertyInfo.PropertyType == typeof(string))
                        {
                            filterStr += name.Remove(0, 2) + " = '" + value + "' and ";
                        }
                    }
                }
                if (filterStr.Length > 6)
                {
                    sqlCmd = sqlCmd + filterStr.Substring(0, filterStr.Length - 4); ;
                }
                else
                {
                    //sqlCmd = "Select  * from TR01A ";
                    var yyyyMM = DateTime.Now.ToString("yyyyMM");
                    //sqlCmd = $"Select * from TR01A where PUR_NO like '%{yyyyMM}%'";
                    sqlCmd = $"Select top 30 * from TR01A order by PUR_NO DESC";
                }

                var tempList = ctx.TR01A.SqlQuery(sqlCmd).OrderByDescending(x => x.PUR_NO).Take(100).ToList();
                list.AddRange(TR01Business.FromEntity(tempList));
            }

            return PartialView("_SearchGrid", list);
        }

        public ActionResult DetailGrid(string key)
        {
            int.TryParse(key, out int keyValue);
            return PartialView("_DetailGrid", GetTR01BList(keyValue));
        }

        /// <summary>Both master and detail has change</summary>
        /// <param name="batchData">contain updata、insert、delete list</param>
        /// <param name="master"></param>
        /// <returns></returns>
        public ActionResult DetailGridBatchUpdate(MVCxGridViewBatchUpdateValues<TR01BViewModel, int> updateValues, TR01AViewModel master, string DateBeg, string DateEnd, string Tax)
        {
            //Validation           
            string errMsg = "";
            if (UserInfo.LanguageType == Language.Type.VN)
            {
                master.DtPUR_DT = DateTime.Parse(DateBeg);
                master.DtEXP_DT = DateTime.Parse(DateEnd);
                ModelState.Clear();
                TryValidateModel(master);
            }
            var isValid = TR01Business.Validation(master, updateValues, ModelState);
            ModelState.Remove("DtPUR_DT");
            ModelState.Remove("DtEXP_DT");
            if (!TR01Business.ValidateRemove(master, updateValues.Update))
            {
                isValid = false;
                errMsg = "資料已被其他人異動過";
            }
            if (master.CFN_YN == "Y")
            {
                errMsg += "資料已確認  不可修改";
            }
            else if (master.CFN_YN == "P")
            {
                errMsg += "資料申請中  不可修改";
            }
            else
            {
                //Save
                if (isValid)
                {
                    var TR01A = TR01Business.BeforSave(master);
                    if (UserInfo.LanguageType == Language.Type.VN)
                    {
                        TR01A.PUR_DT = DateBeg.Replace("/", "");//master.DtPUR_DT.Value.ToString("yyyyddMM");
                        TR01A.EXP_DT = DateEnd.Replace("/", "");//master.DtEXP_DT.Value.ToString("yyyyddMM");
                        System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Language.US);
                        TR01A.TAX_RT = double.Parse(Tax);
                    }
                    else
                    {
                        TR01A.PUR_DT = master.DtPUR_DT.Value.ToString("yyyyMMdd");
                        TR01A.EXP_DT = master.DtEXP_DT.Value.ToString("yyyyMMdd");
                    }
                    var addList = TR01Business.BeforSave(updateValues.Insert, EditType.AddNew);
                    var updateList = TR01Business.BeforSave(updateValues.Update, EditType.Update);
                    var result = _Service.Update(TR01A, addList, updateList, updateValues.DeleteKeys);
                    errMsg = result.Message;
                    master = TR01Business.FromEntity(result.Data);
                }
            }



            //Error handling
            if (errMsg.Length > 0 || !isValid)
            {
                for (int i = 0; i < updateValues.Insert.Count; i++)
                {
                    ModelState.AddModelError($"Insert[{i}].IsValid", "Error");
                }

                for (int i = 0; i < updateValues.Update.Count; i++)
                {
                    ModelState.AddModelError($"Update[{i}].IsValid", "Error");
                }

                string deleteIDStrList = "";
                for (int i = 0; i < updateValues.DeleteKeys.Count; i++)
                {
                    updateValues.SetErrorText(updateValues.DeleteKeys[i], "Unable to delete!");
                    deleteIDStrList += updateValues.DeleteKeys[i] + ",";
                }

                ViewData["DeleteIDList"] = deleteIDStrList;
                ViewData["ErrMsg"] = Resources.Resource.FailureStr + "<br />" + errMsg;
            }

            ViewData["Key"] = master.TR01A_ID;
            var BA01BList = new Services.BA01.BA01Service().GetAllD(master.BA01A_ID).ToList();
            ViewBag.Data = BA01BList;
            //var BA01B = BA01BList.FirstOrDefault(x => x.BA01B_ID == master.BA01B_ID);
            //if (BA01B != null)
            //{
            //    master.TEL_NO = BA01B.TEL_NO;
            //    master.TEL_EX = BA01B.TEL_EX;
            //}
            ViewData["CFN_YN"] = master.CFN_YN;
            if (master.CFN_YN == "Y") 
            {
                ViewData["IsConfirm"] = true;
            }
            else
            {
                ViewData["IsConfirm"] = false;
            }
            ViewData["MasterForm"] = ReadViewHelper.PartialView(this, "_MasterForm", master);
            return PartialView("_DetailGrid", GetTR01BList(master.TR01A_ID));
        }

        /// <summary>Only master data change</summary>
        /// <param name="master">主檔資料</param>
        /// <param name="key">JS keyOnPage的 value</param>
        /// <param name="Deletekey">要刪除的key</param>
        /// <param name="Reload_TF">控制是否單純刷新資料</param>
        /// <returns></returns>
        public ActionResult DetailGridCustomUpdate(TR01AViewModel master, string key, string Deletekey, string Reload_TF, string Lock, string DateBeg, string DateEnd, string Tax)
        {
            var errMsg = "";
            if (Lock != null)
            {
                var entities = new PURSysEntities();
                var keyInt = int.Parse(key);
                var data = entities.TR01A.First(x => x.TR01A_ID == keyInt);
                if (Lock == "Y")
                {
                    data.CFN_YN = "N";
                }
                else
                {
                    data.CFN_YN = "Y";
                }
                entities.SaveChanges();
            }

            if (Reload_TF == "T")//load畫面 => 新增/查詢/取消
            {
                int.TryParse(key, out int keyValue);
                if (keyValue > 0)
                {
                    master = TR01Business.FromEntity(_Service.GetA(keyValue));
                }
                else
                {
                    master.DtPUR_DT = DateTime.UtcNow.AddHours(07);
                    master.CPN_NM = "劲亨金属表面处理工業有限公司   Công ty Jingheng";
                    master.TAX_RT = CacheCommonDataModule.GetTaxRate().First().Value;
                }
                ModelState.Clear();
            }
            else // 修改(新增、修改) / 刪除 
            {

                if (master.CFN_YN == "Y")
                {
                    errMsg += "資料已確認  不可修改";
                    int.TryParse(key, out int keyValue);
                    master = TR01Business.FromEntity(_Service.GetA(keyValue));
                }
                else if (master.CFN_YN == "P")
                {
                    errMsg += "資料申請中  不可修改";
                    int.TryParse(key, out int keyValue);
                    master = TR01Business.FromEntity(_Service.GetA(keyValue));
                }
                else
                {
                    if (int.TryParse(Deletekey, out int keyValue))// 刪除
                    {
                        if (keyValue > 0)
                        {
                            errMsg = "查無此筆刪除資料";
                            var count = _Service.GetA(x => x.TR01A_ID == keyValue).Count();
                            if (count == 1)
                            {
                                errMsg = _Service.Delete(keyValue);//刪除成功回傳空字串
                            }
                            if (errMsg.Length > 0)
                            {
                                ViewData["ErrMsg"] = "Delete Fail";
                                master = TR01Business.FromEntity(_Service.GetA(keyValue));//取回刪除失敗的資料回畫面顯示
                            }
                        }
                        ModelState.Clear();
                    }
                    else// Save 新增、修改 data
                    {
                        //Help.ClearError(ModelState, "DtPUR_DT");
                        //Help.ClearError(ModelState, "DtEXP_DT");
                        if (UserInfo.LanguageType == Language.Type.VN)
                        {
                            master.DtPUR_DT = DateTime.Parse(DateBeg);
                            master.DtEXP_DT = DateTime.Parse(DateEnd);
                            ModelState.Clear();
                            TryValidateModel(master);
                        }

                        var isValid = TR01Business.Validation(master, ModelState);
                        if (!TR01Business.ValidateRemove(master, null))
                        {
                            isValid = false;
                            errMsg = "資料已被其他人異動過";
                        }
                        if (isValid)// Save
                        {
                            var TR01A = TR01Business.BeforSave(master);
                            if (UserInfo.LanguageType == Language.Type.VN)
                            {
                                TR01A.PUR_DT = DateBeg.Replace("/", "");//master.DtPUR_DT.Value.ToString("yyyyddMM");
                                TR01A.EXP_DT = DateEnd.Replace("/", "");//master.DtEXP_DT.Value.ToString("yyyyddMM");
                                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Language.US);
                                TR01A.TAX_RT = double.Parse(Tax);
                            }
                            else
                            {
                                TR01A.PUR_DT = master.DtPUR_DT.Value.ToString("yyyyMMdd");
                                TR01A.EXP_DT = master.DtEXP_DT.Value.ToString("yyyyMMdd");
                            }
                            var result = _Service.Update(TR01A);
                            errMsg = result.Message;
                            master = TR01Business.FromEntity(result.Data);
                        }

                        if (errMsg.Length > 0 || !isValid)// Result
                        {
                            errMsg += Resources.Resource.FailureStr;

                        }
                    }
                }
            }
            ViewData["ErrMsg"] = errMsg;
            ViewData["Key"] = master.TR01A_ID;


            var BA01BList = new Services.BA01.BA01Service().GetAllD(master.BA01A_ID).ToList();
            ViewBag.Data = BA01BList;
            ViewData["CFN_YN"] = master.CFN_YN;
            if (master.CFN_YN == "Y")
            {
                ViewData["IsConfirm"] = true;
            }
            else
            {
                ViewData["IsConfirm"] = false;
            }

            ViewData["MasterForm"] = ReadViewHelper.PartialView(this, "_MasterForm", master);
            return PartialView("_DetailGrid", GetTR01BList(master.TR01A_ID));
        }

        private List<TR01BViewModel> GetTR01BList(int TR01A_ID)
        {
            var dataList = _Service.GetB(TR01A_ID);
            if (TR01A_ID > 0)
            {
                ViewData["TaxRate"] = _Service.GetA(TR01A_ID).TAX_RT;
            }
            return TR01Business.FromEntity(dataList);
        }

        public ActionResult ComboBoxContact(int? key)
        {
            if (key.HasValue)
            {
                var entity = new PURSysEntities();
                var BA01A = entity.BA01A.FirstOrDefault(x => x.BA01A_ID == key.Value);
                if (BA01A != null)
                {
                    ViewData["ADD_DR"] = BA01A.ADD_DR;
                    ViewData["FAX_NO"] = BA01A.FAX_NO;
                    ViewData["BA03A_ID"] = BA01A.BA03A_ID;
                    var BA01BList = new Services.BA01.BA01Service().GetAllD(key.Value).ToList();
                    var BA01B = BA01BList.FirstOrDefault(x => x.BA01A_ID == key.Value);
                    if (BA01B != null)
                    {
                        ViewData["TEL_NO"] = BA01B.TEL_NO;
                        ViewData["TEL_EX"] = BA01B.TEL_EX;
                    }
                    ViewData["CUR_RT"] = CacheCommonDataModule.GetBA03A().First(x => x.BA03A_ID == BA01A.BA03A_ID).CUR_RT;
                }
                else
                {
                    ViewData["ADD_DR"] = "";
                    ViewData["FAX_NO"] = "";
                    ViewData["BA03A_ID"] = "";
                }

                ViewBag.Data = entity.BA01B.Where(x => x.BA01A_ID == key.Value).ToList();
            }
            else
            {
                ViewBag.Data = new List<BA01B>();
            }
            return PartialView("_ComboBoxContact");
        }
    }
}