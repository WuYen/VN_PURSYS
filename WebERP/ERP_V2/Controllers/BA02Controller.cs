using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_V2.Services.BA02;
using ERP_V2.ViewModels.BA02;
using DevExpress.Web.Mvc;
using ERP_V2.Models;

namespace ERP_V2.Controllers
{
    public class BA02Controller : Controller
    {
        private BA02Service _Service;

        public BA02Controller()
        {
            _Service = new BA02Service();
        }

        // GET: BA02
        /// <summary>List all BA02A</summary>
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
        public ActionResult MasterGrid(int? key, string type)
        {
            if (key.HasValue)
            {
                ViewData["selectedItem"] = key.Value;
            }
            var list = GetBA02AList();
            if (int.TryParse(type, out int typeInt) && typeInt != 0)
            {
                list = list.Where(x => x.TYP_ID == typeInt).ToList();
            }
            return PartialView("_MasterGrid", list);
        }

        public ActionResult MasterDelete(int BA02A_ID)
        {
            var errMsg = "";
            bool isSuccess = true;
            var BA02A = _Service.GetM(BA02A_ID);
            if (BA02A != null)
            {
                BA02A.UPDATE_USER = UserInfo.Id;
                BA02A.UPDATE_DATE = DateTime.Now;

                errMsg = _Service.DeleteM(BA02A);
            }

            if (errMsg.Length > 0 || BA02A == null)
            {
                isSuccess = false;
                ViewData["EditError"] = "Delete Fail";
            }
            ViewData["IsSuccess"] = isSuccess;
            return PartialView("_MasterGrid", GetBA02AList());
        }

        public ActionResult Edit(int key)
        {
            var model = new Edit();
            if (key > 0)
            {
                model.MasterKey = key;//use in detail grid

                var temp = _Service.GetMD(key);
                model.Master = new BA02AViewModel(temp.Master);

                var dataCollection = new List<BA02BViewModel>();
                foreach (var item in temp.DetaiList)
                {
                    dataCollection.Add(new BA02BViewModel(item));
                }
                model.DetailList = dataCollection;
            }
            else
            {
                model.MasterKey = 0;
                model.Master = new BA02AViewModel();
                model.DetailList = new List<BA02BViewModel>();
            }
            return View("Edit", model);
        }

        public ActionResult DetailGrid(int key)
        {
            return PartialView("_DetailGrid", GetBA02BList(key));
        }

        /// <summary>Both master and detail has change</summary>
        /// <param name="batchData">contain updata、insert、delete list</param>
        /// <param name="master"></param>
        /// <returns></returns>
        public ActionResult DetailGridBatchUpdate(MVCxGridViewBatchUpdateValues<BA02BViewModel, int> updateValues, BA02AViewModel master)
        {
            string errMsg = "";
            int errorCount = 0;
            int BA02A_ID = master.BA02A_ID;
            //Validation
            ValidateMaster(master);

            for (int i = 0; i < updateValues.Insert.Count; i++)
            {
                if (updateValues.IsValid(updateValues.Insert[i]) == false)
                {
                    errorCount++;
                }
                if (updateValues.Insert[i].PUR_PR < 0)
                {
                    errorCount++;
                    ModelState.AddModelError($"Insert[{i}].PUR_PR", "金額不可為負");//display error in cell
                }
                //if (updateValues.Insert[i].REM_MM.Length > 10)
                //{
                //    errorCount++;
                //    ModelState.AddModelError($"Update[{i}].REM_MM", "REM_MM太長");//display error in cell
                //}
                //foreach (var err in DDMHelper.ValidateData("BA02B", updateValues.Insert[i], replace: replace))
                //{
                //    errorCount++;
                //    ModelState.AddModelError($"Insert[{i}].{err.Key}", err.Value);//display error in cell
                //}
            }

            for (int i = 0; i < updateValues.Update.Count; i++)
            {
                if (updateValues.IsValid(updateValues.Update[i]) == false)
                {
                    errorCount++;
                }
                if (updateValues.Update[i].PUR_PR < 0)
                {
                    errorCount++;
                    ModelState.AddModelError($"Insert[{i}].PUR_PR", "金額不可為負");//display error in cell
                }
                //if (updateValues.Update[i].REM_MM.Length > 10)
                //{
                //    errorCount++;
                //    ModelState.AddModelError($"Update[{i}].REM_MM", "REM_MM太長");//display error in cell
                //}
                //foreach (var err in DDMHelper.ValidateData("BA02B", updateValues.Update[i], replace: replace))
                //{
                //    errorCount++;
                //    ModelState.AddModelError($"Update[{i}].{err.Key}", err.Value);//display error in cell
                //}
            }

            //Save
            if (errorCount == 0 && ModelState.IsValid)
            {
                var BA02A = MasterToEntity(master);

                var addList = new List<BA02B>();
                foreach (var item in updateValues.Insert)
                {
                    var BA02B = new BA02B();
                    item.REM_MM = master.REM_MM;
                    item.CREATE_USER = UserInfo.Id;
                    item.CREATE_DATE = DateTime.Now;
                    item.ToDomain(BA02B);
                    addList.Add(BA02B);
                }

                var updateList = new List<BA02B>();
                foreach (var item in updateValues.Update)
                {
                    var BA02B = new BA02B();
                    BA02B = _Service.GetD(x => x.BA02B_ID == item.BA02B_ID);
                    item.CREATE_USER = BA02B.CREATE_USER;
                    item.CREATE_DATE = BA02B.CREATE_DATE;
                    item.UPDATE_USER = UserInfo.Id;
                    item.UPDATE_DATE = DateTime.Now;
                    item.ToDomain(BA02B);
                    updateList.Add(BA02B);
                }

                errMsg = _Service.UpdateMD(BA02A, addList, updateList, updateValues.DeleteKeys);
            }

            //Result
            if (errMsg.Length > 0 || !ModelState.IsValid)
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
                ViewData["EditErrorMsg"] = "Failure" + "<br />" + errMsg;
                ViewData["DeleteIDList"] = deleteIDStrList;
                ViewData["MasterForm"] = ReadViewHelper.PartialView(this, "_MasterForm", master);
                return PartialView("_DetailGrid", GetBA02BList(BA02A_ID));
            }
            else
            {
                CacheHelper.Invalidate("BA02A");
                SetTempData(master.BA02A_ID);
                return RedirectToAction("Index");
            }
        }

        /// <summary>Only master data change</summary>
        /// <param name="master"></param>
        /// <returns></returns>
        public ActionResult DetailGridCustomUpdate(BA02AViewModel master)
        {
            string errMsg = "";
            //Validation
            ValidateMaster(master);
            //Save
            if (ModelState.IsValid)
            {
                var BA02A = MasterToEntity(master);

                var result = _Service.UpdateM(BA02A);
                errMsg = result.Message;
                master = new BA02AViewModel(result.Data);
            }
            //Result
            if (errMsg.Length > 0 || !ModelState.IsValid)
            {
                errMsg += "Fail";
                ViewData["EditErrorMsg"] = errMsg;
                ViewData["MasterForm"] = ReadViewHelper.PartialView(this, "_MasterForm", master);
                return PartialView("_DetailGrid", GetBA02BList(master.BA02A_ID));
            }
            else
            {
                CacheHelper.Invalidate("BA02A");
                SetTempData(master.BA02A_ID);
                return RedirectToAction("Index");
            }
        }


        #region Private Funcitons
        private void ValidateMaster(BA02AViewModel master)
        {
            if (master.TYP_ID == null)
            {
                ModelState.AddModelError("TYP_ID", "必填");
            }
        }

        private BA02A MasterToEntity(BA02AViewModel master)
        {
            var BA02A = new BA02A();
            if (master.BA02A_ID == 0)//Adding New
            {
                master.CREATE_USER = UserInfo.Id;
                master.CREATE_DATE = DateTime.Now;
                master.ToDomain(BA02A);
            }
            else//Update
            {
                BA02A = _Service.GetM(master.BA02A_ID);
                master.CREATE_USER = BA02A.CREATE_USER;
                master.CREATE_DATE = BA02A.CREATE_DATE;
                master.UPDATE_USER = UserInfo.Id;
                master.UPDATE_DATE = DateTime.Now;
                master.ToDomain(BA02A);
            }
            return BA02A;
        }

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

        private List<BA02AViewModel> GetBA02AList()
        {
            var dataList = _Service.GetAllM().ToList();

            List<BA02AViewModel> dataCollection = new List<BA02AViewModel>();

            if (dataList.Count > 0)
            {
                foreach (var item in dataList)
                {
                    dataCollection.Add(new BA02AViewModel(item));
                }
            }

            return dataCollection;
        }

        private List<BA02BViewModel> GetBA02BList(int BA02A_ID)
        {
            var dataCollection = new List<BA02BViewModel>();
            if (BA02A_ID > 0)
            {
                var dataList = _Service.GetAllD(BA02A_ID).ToList();
                if (dataList.Count > 0)
                {
                    foreach (var item in dataList)
                    {
                        dataCollection.Add(new BA02BViewModel(item));
                    }
                }
            }
            return dataCollection.OrderByDescending(x => x.CREATE_DATE).ToList();
        }
        #endregion Private Funcitons
    }
}