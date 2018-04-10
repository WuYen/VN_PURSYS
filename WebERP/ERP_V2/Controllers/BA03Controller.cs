using ERP_V2.Services.BA03;
using ERP_V2.Utility;
using ERP_V2.ViewModels.BA03;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_V2.Models;

namespace ERP_V2.Controllers
{
    public class BA03Controller : Controller
    {
        private BA03Service _Service;
        public BA03Controller()
        {
            this._Service = new BA03Service();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MasterGrid()
        {
            return PartialView("_MasterGrid", GetBA03AList());
        }

        public ActionResult EditForm(ErrorData<BA03AViewModel> item)
        {
            return PartialView("_EditForm", item);
        }

        public ActionResult AddNew(BA03AViewModel data)
        {
            SaveData(data, EditType.AddNew);
            return PartialView("_MasterGrid", GetBA03AList());
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Update(BA03AViewModel data)
        {
            SaveData(data, EditType.Update);
            return PartialView("_MasterGrid", GetBA03AList());
        }

        public ActionResult Delete(int BA03A_ID)
        {
            var errMsg = _Service.Delete(BA03A_ID);

            if (errMsg.Length > 0)
            {
                ViewData["IsSuccess"] = false;
                ViewData["EditError"] = errMsg;
            }
            else
            {
                ViewData["IsSuccess"] = true;
            }

            return PartialView("_MasterGrid", GetBA03AList());
        }

        private List<BA03AViewModel> GetBA03AList()
        {
            var dataList = _Service.GetAll().ToList();

            var dataCollection = new List<BA03AViewModel>();

            if (dataList.Count > 0)
            {
                foreach (var item in dataList)
                {
                    dataCollection.Add(new BA03AViewModel(item));
                }
            }

            return dataCollection;
        }

        private void SaveData(BA03AViewModel data, EditType type)
        {
            var errMsg = "";
            //Validation
            //foreach (var item in DDMHelper.ValidateData("BA03A", data))
            //{
            //    ModelState.AddModelError(item.Key, item.Value);
            //}

            //save data
            if (ModelState.IsValid)
            {
                var BA03A = new BA03A();
                if (type == EditType.AddNew)
                {
                    data.CREATE_USER = UserInfo.Id;
                    data.CREATE_DATE = DateTime.Now;
                    data.ToDomain(BA03A);
                    errMsg = _Service.Create(BA03A);

                }
                else
                {
                    BA03A = _Service.Get(data.BA03A_ID);
                    data.CREATE_USER = BA03A.CREATE_USER;
                    data.CREATE_DATE = BA03A.CREATE_DATE;
                    data.UPDATE_USER = UserInfo.Id;
                    data.UPDATE_DATE = DateTime.Now;
                    data.ToDomain(BA03A);
                    errMsg = _Service.Update(BA03A);
                }
            }

            //error handling
            if (errMsg.Length > 0 || !ModelState.IsValid)
            {
                var errorData = new ErrorData<BA03AViewModel>()
                {
                    ModelState = ModelState,
                    Data = data
                };
                ViewData["ErrorData"] = errorData;
                ViewData["IsSuccess"] = false;
                ViewData["EditError"] = "Fail please check error";
            }
            else
            {
                CacheHelper.Invalidate("BA03A");
                ViewData["IsSuccess"] = true;
            }
        }

        protected override void Dispose(bool disposing)
        {
            _Service.Dispose();
            base.Dispose(disposing);
        }
    }
}