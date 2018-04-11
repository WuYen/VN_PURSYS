using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_V2.Services.BA01;
using ERP_V2.Models;
using ERP_V2.ViewModels.BA01;
using DevExpress.Web.Mvc;

namespace ERP_V2.Controllers
{
    public class BA01Controller : Controller
    {
        private BA01Service _Service = new BA01Service();

        /// <summary>List all BA01A</summary>
        /// <param name="key">Edit cancel set focused row</param>
        /// <returns></returns>
        public ActionResult Index(int? key)
        {
            var model = new IndexViewModel() { FocusKey = key, IsSuccess = false };

            var tempData = GetTempData();
            if (tempData != null)
            {
                model.IsSuccess = tempData.IsSuccess;
                model.FocusKey = tempData.Key;
            }

            return View(model);
        }

        /// <summary>show Grid</summary>
        /// <param name="key">focus row's key</param>
        /// <returns></returns>
        public ActionResult MasterGrid(int? key)
        {
            if (key.HasValue)
            {
                ViewData["selectedItem"] = key.Value;
            }
            return PartialView("_MasterGrid", GetBA01AList());
        }

        public ActionResult MasterDetailGrid(int key)
        {
            ViewData["key"] = key;
            return PartialView("_MasterDetailGrid", GetBA01BList(key));
        }

        public ActionResult MasterDelete(int BA01A_ID)
        {
            var errMsg = "";
            bool isSuccess = true;
            var BA01A = _Service.GetM(x => x.BA01A_ID == BA01A_ID);
            if (BA01A != null)
            {
                BA01A.UPDATE_USER = UserInfo.Id;
                BA01A.UPDATE_DATE = DateTime.Now;

                errMsg = _Service.DeleteM(BA01A);
            }

            if (errMsg.Length > 0 || BA01A == null)
            {
                isSuccess = false;
                ViewData["EditError"] = "Delete Fail <br>" + errMsg;
            }
            ViewData["IsSuccess"] = isSuccess;
            return PartialView("_MasterGrid", GetBA01AList());
        }

        public ActionResult Edit(int key)
        {
            var model = new Edit();
            if (key > 0)
            {
                model.MasterKey = key;//use in detail grid

                var temp = _Service.GetMD(key);
                model.Master = new BA01AViewModel(temp.Master);

                var dataCollection = new List<BA01BViewModel>();
                foreach (var item in temp.DetaiList)
                {
                    dataCollection.Add(new BA01BViewModel(item));
                }
                model.DetailList = dataCollection;
            }
            else
            {
                model.MasterKey = 0;
                model.Master = new BA01AViewModel();
                model.DetailList = new List<BA01BViewModel>();
            }
            return View("Edit", model);
        }

        public ActionResult DetailGrid(int key)
        {
            return PartialView("_DetailGrid", GetBA01BList(key));
        }

        /// <summary>Both master and detail has change</summary>
        /// <param name="batchData">contain updata、insert、delete list</param>
        /// <param name="master"></param>
        /// <returns></returns>
        public ActionResult DetailGridBatchUpdate(MVCxGridViewBatchUpdateValues<BA01BViewModel, int> updateValues, BA01AViewModel master)
        {
            int errorCount = 0;
            //Validation           
            ValidateMaster(master);

            for (int i = 0; i < updateValues.Insert.Count; i++)
            {
                if (updateValues.IsValid(updateValues.Insert[i]) == false)
                {
                    errorCount++;
                }
            }

            for (int i = 0; i < updateValues.Update.Count; i++)
            {
                if (updateValues.IsValid(updateValues.Update[i]) == false)
                {
                    errorCount++;
                }
            }
            string errMsg = "";
            //Save
            if (errorCount == 0 && ModelState.IsValid)
            {
                var BA01A = MasterToEntity(master);

                var addList = new List<BA01B>();
                foreach (var item in updateValues.Insert)
                {
                    var BA01B = new BA01B();
                    item.CREATE_USER = UserInfo.Id;
                    item.CREATE_DATE = DateTime.Now;
                    item.ToDomain(BA01B);
                    addList.Add(BA01B);
                }

                var updateList = new List<BA01B>();
                foreach (var item in updateValues.Update)
                {
                    var BA01B = new BA01B();
                    BA01B = _Service.GetD(x => x.BA01B_ID == item.BA01B_ID);
                    item.BA01A_ID = BA01B.BA01A_ID;
                    item.CREATE_USER = BA01B.CREATE_USER;
                    item.CREATE_DATE = BA01B.CREATE_DATE;
                    item.UPDATE_USER = UserInfo.Id;
                    item.UPDATE_DATE = DateTime.Now;
                    item.ToDomain(BA01B);
                    updateList.Add(BA01B);
                }

                errMsg = _Service.UpdateMD(BA01A, addList, updateList, updateValues.DeleteKeys);
            }

            //Result
            if (errorCount > 0 || errMsg.Length > 0 || !ModelState.IsValid)
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
                ViewData["EditErrorMsg"] = Resources.Resource.FailureStr + "<br />" + errMsg;
                ViewData["DeleteIDList"] = deleteIDStrList;
                ViewData["MasterForm"] = ReadViewHelper.PartialView(this, "_MasterForm", master);

                return PartialView("_DetailGrid", GetBA01BList(master.BA01A_ID));
            }
            else
            {
                CacheHelper.Invalidate("BA01A");
                SetTempData(master.BA01A_ID);
                return RedirectToAction("Index");
            }
        }

        /// <summary>Only master data change</summary>
        /// <param name="master"></param>
        /// <returns></returns>
        public ActionResult DetailGridCustomUpdate(BA01AViewModel master)
        {
            string errMsg = "";
            //Validation
            ValidateMaster(master);

            //Save
            if (ModelState.IsValid)
            {
                var BA01A = MasterToEntity(master);

                var result = _Service.UpdateM(BA01A);
                errMsg = result.Message;
                master = new BA01AViewModel(result.Data);
            }

            //Result
            if (errMsg.Length > 0 || !ModelState.IsValid)
            {
                errMsg += Resources.Resource.FailureStr;
                ViewData["EditErrorMsg"] = errMsg;
                ViewData["MasterForm"] = ReadViewHelper.PartialView(this, "_MasterForm", master);
                return PartialView("_DetailGrid", GetBA01BList(master.BA01A_ID));
            }
            else
            {
                SetTempData(master.BA01A_ID);
                CacheHelper.Invalidate("BA01A");
                return RedirectToAction("Index");
            }
        }

        #region Private Funcitons
        private void ValidateMaster(BA01AViewModel master)
        {
        }

        private BA01A MasterToEntity(BA01AViewModel master)
        {
            var BA01A = new BA01A();
            if (master.BA01A_ID == 0)//Adding New
            {
                master.CREATE_USER = UserInfo.Id;
                master.CREATE_DATE = DateTime.Now;
                master.ToDomain(BA01A);
            }
            else//Update
            {
                BA01A = _Service.GetM(master.BA01A_ID);
                master.CREATE_USER = BA01A.CREATE_USER;
                master.CREATE_DATE = BA01A.CREATE_DATE;
                master.UPDATE_USER = UserInfo.Id;
                master.UPDATE_DATE = DateTime.Now;
                master.ToDomain(BA01A);
            }
            return BA01A;
        }

        //private BA01B DetailToDomain(BA01BViewModel detail)
        //{
        //    var BA01B = new BA01B();
        //    if (detail.BA01B_ID == 0)//Adding New
        //    {
        //        //BA01A_ID 在Service處理      
        //        detail.CREATE_USER = "Admin";
        //        detail.CREATE_DATE = DateTime.Now;
        //        detail.ToDomain(BA01B);
        //    }
        //    else//Update
        //    {
        //        BA01B = _Service.GetD(x => x.BA01B_ID == detail.BA01B_ID);
        //        detail.BA01A_ID = BA01B.BA01A_ID;
        //        detail.CREATE_USER = BA01B.CREATE_USER;
        //        detail.CREATE_DATE = BA01B.CREATE_DATE;
        //        detail.LASTUPDATE_USER = "Admin";
        //        detail.LASTUPDATE_DATE = DateTime.Now;
        //        detail.ToDomain(BA01B);
        //    }
        //    return BA01B;
        //}

        private TempModel GetTempData()
        {
            if (TempData["para"] != null)//Edit asve success
            {
                var tempModel = TempData["para"] as TempModel;
                return tempModel;
            }
            else
            {
                return null;
            }
        }

        private void SetTempData(int key)
        {
            TempModel tempModel = new TempModel()
            {
                Key = key,
                IsSuccess = true
            };
            TempData["para"] = tempModel;
        }

        public class TempModel
        {
            public int Key { get; set; }

            public bool IsSuccess { get; set; }
        }

        private List<BA01AViewModel> GetBA01AList()
        {
            var dataList = _Service.GetAllM().ToList();

            List<BA01AViewModel> dataCollection = new List<BA01AViewModel>();

            if (dataList.Count > 0)
            {
                foreach (var item in dataList)
                {
                    dataCollection.Add(new BA01AViewModel(item));
                }
            }

            return dataCollection;
        }

        private List<BA01BViewModel> GetBA01BList(int BA01A_ID)
        {
            var dataCollection = new List<BA01BViewModel>();
            var dataList = _Service.GetAllD(BA01A_ID).ToList();
            if (dataList.Count > 0)
            {
                foreach (var item in dataList)
                {
                    dataCollection.Add(new BA01BViewModel(item));
                }
            }

            return dataCollection.OrderByDescending(x => x.CREATE_DATE).ToList();
        }

        protected override void Dispose(bool disposing)
        {
            _Service.Dispose();
            base.Dispose(disposing);
        }
        #endregion Private Funcitons

    }
}