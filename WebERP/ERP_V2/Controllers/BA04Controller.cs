using ERP_V2.Utility;
using ERP_V2.ViewModels.BA04;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_V2.Controllers
{
    public class BA04Controller : BaseController
    {
        private BA04Service _Service = new BA04Service();

        // GET: BA04
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MasterGrid()
        {
            return PartialView("_MasterGrid", GetList());
        }

        public ActionResult EditForm(BA04AModel model)
        {
            if (model.BA04A_ID == 0)//新增的時候初始化
            {
                BA04Business.SetInitValue(ref model);
                ModelState.Clear();
            }
            return PartialView("_EditForm", model);
        }


        public ActionResult Update(BA04AModel master)
        {
            //Validation
            List<string> errList = BA04Business.Validation(master, ModelState);

            //BeforeSave + 驗證DDM + Save         
            if (errList.Count == 0)
            {
                //BeforeSave
                var BA04A = BA04Business.BeforSave(master);

                //驗證DDM
                errList = BA04Business.Validation(master, ModelState, "BA04A");

                //Save
                if (errList.Count == 0)
                {
                    var result = _Service.Update(BA04A);
                    master = BA04Business.FromEntity(result.Data);

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
                CacheCommonDataModule.ResetBA04AList();
                ViewData["Success"] = true;
            }

            return PartialView("_MasterGrid", GetList());
        }

        /// <summary> Grid 刪除事件 </summary>
        /// <param name="BA04A_ID">要跟 Grid 上 KeyFieldName 名稱相同</param>
        /// <returns></returns>
        public ActionResult Delete(int BA04A_ID)
        {
            var result = _Service.Delete(BA04A_ID);

            if (!string.IsNullOrWhiteSpace(result.Message))//失敗
            {
                var errMsg = _Service.Delete(BA04A_ID).Message;
                ViewData["ErrMsg"] = errMsg;
            }
            else//成功
            {
                ViewData["Success"] = true;
            }
            return PartialView("_MasterGrid", GetList());
        }

        private List<BA04AModel> GetList()
        {
            return BA04Business.FromEntity(_Service.GetA(x=>x.BA04A_ID>0).ToList());
        }
    }
}