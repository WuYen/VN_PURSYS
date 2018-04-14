using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using ERP_V2.ViewModels.TR01M;
using ERP_V2.Models;
using System.Reflection;

namespace ERP_V2.Controllers
{
    public class TR01MController : BaseController
    {
        private TR01Service _Service = new TR01Service();

        // GET: TR01M
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GridTR01A(SearchViewModel search)
        {
            //var data = TR01Business.FromEntity(_Service.GetA(x => x.TR01A_ID == 26 || x.TR01A_ID == 27 || x.TR01A_ID == 23 || x.TR01A_ID == 28 || x.TR01A_ID == 35).ToList());

            List<TR01AModel> list = new List<TR01AModel>();
            using (var ctx = new PURSysEntities())
            {
                string sqlCmd = "Select * from TR01A ";
                string filterStr = "Where ";
                foreach (PropertyInfo propertyInfo in search.GetType().GetProperties())
                {
                    var value = propertyInfo.GetValue(search, null);
                    if (value != null && value.ToString().Length > 0)
                    {
                        var name = propertyInfo.Name;
                        if (name.Contains("ConfirmStatus"))
                        {
                            filterStr += "CFN_YN = '" + value + "' and ";
                        }
                        else if (name.Contains("StrDT"))
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
                    sqlCmd = sqlCmd + filterStr.Substring(0, filterStr.Length - 4) + " and PUR_NO > '2018'";
                }
                else
                {
                    var yyyyMM = DateTime.Now.ToString("yyyyMM");
                    //sqlCmd = $"Select * from TR01A where PUR_NO like '%{yyyyMM}%'";
                    sqlCmd = $"Select * from TR01A where PUR_NO > '2018' order by PUR_NO DESC";
                }

                var tempList = ctx.TR01A.SqlQuery(sqlCmd).OrderByDescending(x => x.PUR_NO).Take(100).ToList();
                list.AddRange(TR01Business.FromEntity(tempList));
            }

            return PartialView("_GridTR01A", list);
        }

        public ActionResult ApplyCloseAjax(int TR01A_ID)
        {
            PURSysEntities entity = new Models.PURSysEntities();
            var result = entity.TR01A.FirstOrDefault(x => x.TR01A_ID == TR01A_ID);
            result.CFN_YN = "P";
            result.CFN_U1 = UserInfo.Id;
            entity.SaveChanges();
            return new JsonResult { Data = new { IsSuccess = "T" }, JsonRequestBehavior = JsonRequestBehavior.DenyGet };
        }
        public ActionResult RejectApplyAjax(int TR01A_ID)
        {
            PURSysEntities entity = new Models.PURSysEntities();
            var result = entity.TR01A.FirstOrDefault(x => x.TR01A_ID == TR01A_ID);
            result.CFN_YN = "R";
            entity.SaveChanges();
            return new JsonResult { Data = new { IsSuccess = "T" }, JsonRequestBehavior = JsonRequestBehavior.DenyGet };
        }


        public ActionResult CloseConfirmAjax(int TR01A_ID)
        {
            PURSysEntities entity = new Models.PURSysEntities();
            var result = entity.TR01A.FirstOrDefault(x => x.TR01A_ID == TR01A_ID);
            result.CFN_YN = "Y";
            result.CFN_U2 = UserInfo.Id;
            //var aa = System.Threading.Thread.CurrentThread.CurrentCulture;
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            result.CFN_DT = DateTime.Now.ToString("yyyyMMdd");

            var itemList = entity.TR01B.Where(x => x.TR01A_ID == TR01A_ID).ToList();
            foreach (var item in itemList)
            {
                try
                {
                    item.CFN_QT = entity.TR01C.Where(x => x.TR01B_ID == item.TR01B_ID).Sum(x => x.ARR_QT);
                }
                catch (Exception ex)
                {
                    item.CFN_QT = 0;
                }

            }
            entity.SaveChanges();
            return new JsonResult { Data = new { IsSuccess = "T" }, JsonRequestBehavior = JsonRequestBehavior.DenyGet };
        }

        public ActionResult GridTR01B(int? TR01A_ID)
        {
            if (TR01A_ID.HasValue)
            {
                return PartialView("_GridTR01B", TR01Business.FromEntity(_Service.GetB(x => x.TR01A_ID == TR01A_ID).ToList()));
            }

            return PartialView("_GridTR01B");
        }

        public ActionResult GridTR01C(int? TR01B_ID)
        {
            if (TR01B_ID.HasValue)
            {
                var entity = new PURSysEntities();
                var TR01A_ID = entity.TR01B.First(x => x.TR01B_ID == TR01B_ID).TR01A_ID;
                var CFN_YN = entity.TR01A.First(x => x.TR01A_ID == TR01A_ID).CFN_YN;
                if (CFN_YN == "N" || CFN_YN == null || CFN_YN == "R")
                {
                    ViewBag.Enable = true;
                }
                else
                {
                    ViewBag.Enable = false;
                }
                return PartialView("_GridTR01C", TR01Business.FromEntity(_Service.GetC(x => x.TR01B_ID == TR01B_ID).ToList()));
            }
            return PartialView("_GridTR01C");
        }

        public ActionResult TR01CUpdate(int TR01B_ID, TR01CModel master)
        {
            //Validation
            List<string> errList = TR01Business.Validation(master, ModelState);

            //BeforeSave + Save         
            if (errList.Count == 0)
            {
                //BeforeSave
                var TR01A = TR01Business.BeforSave(master);
                TR01A.TR01B_ID = TR01B_ID;
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                TR01A.ARR_DT = (master.DtARR_DT != null ? master.DtARR_DT.Value.ToString("yyyyMMdd") : "");
                //Save
                if (errList.Count == 0)
                {
                    var result = _Service.Update(TR01A);
                    master = TR01Business.FromEntity(result.Data);

                    if (!string.IsNullOrWhiteSpace(result.Message))
                    {
                        errList.Add(result.Message);
                    }
                }
            }

            if (errList.Count > 0)
            {
                ViewData["ErrMsg"] = string.Join("<br />", errList);
                master.ModelState = ModelState;
                ViewData["ErrorData"] = master;
            }
            else
            {
                CheckStatus(TR01B_ID);

                ViewData["IsEdit"] = true;
                ViewData["Success"] = true;
            }
            return PartialView("_GridTR01C", TR01Business.FromEntity(_Service.GetC(x => x.TR01B_ID == TR01B_ID).ToList()));
        }

        private void CheckStatus(int TR01B_ID)
        {
            var entity = new Models.PURSysEntities();
            var TR01A_ID = entity.TR01B.First(x => x.TR01B_ID == TR01B_ID).TR01A_ID;
            var TR01A = entity.TR01A.First(x => x.TR01A_ID == TR01A_ID);
            var result = entity.Database.SqlQuery<int>($"select dbo.Get_Not_Receive_Count({TR01A_ID})").FirstOrDefault();
            if (result != 0)//原本可能是P 但是修改後 變成 N
            {
                TR01A.CFN_YN = "N";
            }
            entity.SaveChanges();

            if (TR01A.CFN_YN == "N" || TR01A.CFN_YN == null || TR01A.CFN_YN == "R")
            {
                ViewBag.Enable = true;
            }
            else
            {
                ViewBag.Enable = false;
            }

        }

        public ActionResult TR01CDelete(int TR01B_ID, int TR01C_ID)
        {
            var result = _Service.DeleteC(TR01C_ID);

            if (!string.IsNullOrWhiteSpace(result.Message))//失敗
            {
                ViewData["ErrMsg"] = result.Message;
            }
            else//成功
            {
                CheckStatus(TR01B_ID);
                ViewData["IsEdit"] = true;
                ViewData["Success"] = true;
            }
            return PartialView("_GridTR01C", TR01Business.FromEntity(_Service.GetC(x => x.TR01B_ID == TR01B_ID).ToList()));
        }

        public ActionResult EditForm(TR01CModel item)
        {
            if (item.TR01C_ID == 0)
            {
                item.DtARR_DT = DateTime.UtcNow.AddHours(07);

                var entity = new PURSysEntities();
                var TR01B = entity.TR01B.First(x => x.TR01B_ID == item.TR01B_ID);
                var tr01cList = entity.TR01C.Where(x => x.TR01B_ID == item.TR01B_ID).ToList();//.Sum(x => x.ARR_QT);
                var total = new decimal();
                if (tr01cList.Count > 0)
                {
                    total = tr01cList.Sum(x => x.ARR_QT);
                }
                item.ARR_QT = TR01B.PUR_QT - total;
                item.ARR_QT = decimal.Parse(item.ARR_QT.ToString("G29"));


                var TR01A = entity.TR01A.FirstOrDefault(x => x.TR01A_ID == TR01B.TR01A_ID);
                if (TR01A != null)
                {

                    item.INV_MY = TR01B.PUR_PR * item.ARR_QT;
                    if (TR01A.TAX_RT.HasValue)
                    {
                        item.INV_MY = item.INV_MY * Convert.ToDecimal(TR01A.TAX_RT.Value);
                    }
                    item.INV_MY = decimal.Parse(item.INV_MY.Value.ToString("G29"));

                    item.CUR_RT = entity.BA03A.First(x => x.BA03A_ID == TR01A.BA03A_ID).CUR_RT;
                    item.CUR_RT = decimal.Parse(item.CUR_RT.ToString("G29"));
                }
                ModelState.Clear();
            }
            return PartialView("_EditForm", item);
        }
    }
}