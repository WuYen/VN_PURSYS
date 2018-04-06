using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ERP_V2.Services.TR01;
using ERP_V2.ViewModels.TR01;
using DevExpress.Web.Mvc;
using ERP_V2.Models;
using System.Data.Entity.Core.Objects;
using System.Data.Common;
using System.Data;

namespace ERP_V2.Controllers
{
    public class TR01Controller : Controller
    {
        private TR01Service _Service;

        public TR01Controller()
        {
            _Service = new TR01Service();
        }

        // GET: TR01
        /// <summary>List all TR01A</summary>
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

            PURSysEntities entities = new PURSysEntities();

            //DbConnection storeConnection = entities.Database.Connection;

            //try
            //{
            //    storeConnection.Open();

            //    using (DbCommand command = storeConnection.CreateCommand())
            //    {
            //        command.CommandText = "select dbo.Get_PUR_NO()";
            //        command.CommandType = CommandType.Text;
            //        command.ExecuteReader();
            //    }
            //}
            //finally
            //{
            //    storeConnection.Close();
            //}

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

            return PartialView("_MasterGrid", GetTR01AList());
        }

        public ActionResult MasterDelete(int TR01A_ID)
        {
            var errMsg = "";
            var TR01A = _Service.GetM(TR01A_ID);
            if (TR01A != null)
            {
                TR01A.UPDATE_USER = "Admin";
                TR01A.UPDATE_DATE = DateTime.Now;

                errMsg = _Service.DeleteM(TR01A);
            }

            if (errMsg.Length > 0 || TR01A == null)
            {
                ViewData["EditError"] = "Delete Fail";
            }
            return PartialView("_MasterGrid", GetTR01AList());
        }

        public ActionResult Edit(int key)
        {
            var model = new Edit();
            if (key > 0)
            {
                model.MasterKey = key;//use in detail grid

                var temp = _Service.GetMD(key);
                model.Master = new TR01AViewModel(temp.Master);

                var dataCollection = new List<TR01BViewModel>();
                foreach (var item in temp.DetaiList)
                {
                    dataCollection.Add(new TR01BViewModel(item));
                }
                model.DetailList = dataCollection;

                var entity = new PURSysEntities();
                var BA01BList = entity.BA01B.Where(x => x.BA01A_ID == model.Master.BA01A_ID).ToList();
                var BA01B = BA01BList.FirstOrDefault(x => x.BA01B_ID == model.Master.BA01B_ID);
                if (BA01B != null)
                {
                    model.Master.TEL_NO = BA01B.TEL_NO;
                    model.Master.TEL_EX = BA01B.TEL_EX;
                }
                ViewBag.Data = BA01BList;
            }
            else
            {
                model.MasterKey = 0;
                model.Master = new TR01AViewModel();
                model.Master.DtPUR_DT = DateTime.UtcNow.AddHours(07);
                model.DetailList = new List<TR01BViewModel>();
            }
            return View("Edit", model);
        }

        public ActionResult DetailGrid(int key)
        {
            return PartialView("_DetailGrid", GetTR01BList(key));
        }

        /// <summary>Both master and detail has change</summary>
        /// <param name="batchData">contain updata、insert、delete list</param>
        /// <param name="master"></param>
        /// <returns></returns>
        public ActionResult DetailGridBatchUpdate(MVCxGridViewBatchUpdateValues<TR01BViewModel, int> updateValues, TR01AViewModel master)
        {
            string errMsg = "";
            int errorCount = 0;
            int TR01A_ID = master.TR01A_ID;
            //Validation
            ValidateMaster(master);

            for (int i = 0; i < updateValues.Insert.Count; i++)
            {
                if (updateValues.IsValid(updateValues.Insert[i]) == false)
                {
                    errorCount++;
                }
                //if (updateValues.Insert[i].REM_MM.Length > 10)
                //{
                //    errorCount++;
                //    ModelState.AddModelError($"Update[{i}].REM_MM", "REM_MM太長");//display error in cell
                //}
            }

            for (int i = 0; i < updateValues.Update.Count; i++)
            {
                if (updateValues.IsValid(updateValues.Update[i]) == false)
                {
                    errorCount++;
                }
                //if (updateValues.Update[i].REM_MM.Length > 10)
                //{
                //    errorCount++;
                //    ModelState.AddModelError($"Update[{i}].REM_MM", "REM_MM太長");//display error in cell
                //}
            }

            //Save
            if (errorCount == 0 && ModelState.IsValid)
            {
                var TR01A = MasterToEntity(master);

                var addList = new List<TR01B>();
                foreach (var item in updateValues.Insert)
                {
                    var TR01B = new TR01B();
                    item.CREATE_USER = "Admin";
                    item.CREATE_DATE = DateTime.Now;
                    item.ToDomain(TR01B);
                    addList.Add(TR01B);
                }

                var updateList = new List<TR01B>();
                foreach (var item in updateValues.Update)
                {
                    var TR01B = new TR01B();
                    TR01B = _Service.GetD(x => x.TR01B_ID == item.TR01B_ID);
                    item.CREATE_USER = TR01B.CREATE_USER;
                    item.CREATE_DATE = TR01B.CREATE_DATE;
                    item.UPDATE_USER = "Admin";
                    item.UPDATE_DATE = DateTime.Now;
                    item.ToDomain(TR01B);
                    updateList.Add(TR01B);
                }

                errMsg = _Service.UpdateMD(TR01A, addList, updateList, updateValues.DeleteKeys);
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
                return PartialView("_DetailGrid", GetTR01BList(TR01A_ID));
            }
            else
            {
                SetTempData(master.TR01A_ID);
                return RedirectToAction("Index");
            }
        }

        /// <summary>Only master data change</summary>
        /// <param name="master"></param>
        /// <returns></returns>
        public ActionResult DetailGridCustomUpdate(TR01AViewModel master)
        {
            string errMsg = "";
            //Validation
            ValidateMaster(master);
            //Save
            if (ModelState.IsValid)
            {
                var TR01A = MasterToEntity(master);

                var result = _Service.UpdateM(TR01A);
                errMsg = result.Message;
                master = new TR01AViewModel(result.Data);
            }
            //Result
            if (errMsg.Length > 0 || !ModelState.IsValid)
            {
                errMsg += "Fail";
                ViewData["EditErrorMsg"] = errMsg;
                ViewData["MasterForm"] = ReadViewHelper.PartialView(this, "_MasterForm", master);
                return PartialView("_DetailGrid", GetTR01BList(master.TR01A_ID));
            }
            else
            {
                SetTempData(master.TR01A_ID);
                return RedirectToAction("Index");
            }
        }

        public ActionResult ComboBoxContact(int? key)
        {
            if (key.HasValue)
            {
                var entity = new PURSysEntities();
                var item = entity.BA01A.FirstOrDefault(x => x.BA01A_ID == key.Value);
                if (item != null)
                {
                    ViewData["ADD_DR"] = item.ADD_DR;
                    ViewData["FAX_NO"] = item.FAX_NO;
                    ViewData["BA03A_ID"] = item.BA03A_ID;
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


        #region Private Funcitons
        private void ValidateMaster(TR01AViewModel master)
        {
            //if (master.REM_MM != null && master.REM_MM.Length > 10)
            //{
            //    ModelState.AddModelError("REM_MM", "Error REM_MM Too Long");
            //}
        }

        private TR01A MasterToEntity(TR01AViewModel master)
        {
            var TR01A = new TR01A();
            if (master.TR01A_ID == 0)//Adding New
            {
                master.CREATE_USER = "Admin";
                master.CREATE_DATE = DateTime.Now;
                master.PUR_NO = new PURSysEntities().Database.SqlQuery<string>("select dbo.Get_PUR_NO()").FirstOrDefault();
                master.ToDomain(TR01A);
            }
            else//Update
            {
                TR01A = _Service.GetM(master.TR01A_ID);
                master.CREATE_USER = TR01A.CREATE_USER;
                master.CREATE_DATE = TR01A.CREATE_DATE;
                master.UPDATE_USER = "Admin";
                master.UPDATE_DATE = DateTime.Now;
                master.ToDomain(TR01A);
            }
            return TR01A;
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

        private List<TR01AViewModel> GetTR01AList()
        {
            var dataList = _Service.GetAllM().ToList();

            List<TR01AViewModel> dataCollection = new List<TR01AViewModel>();

            if (dataList.Count > 0)
            {
                foreach (var item in dataList)
                {
                    dataCollection.Add(new TR01AViewModel(item));
                }
            }

            return dataCollection;
        }

        private List<TR01BViewModel> GetTR01BList(int TR01A_ID)
        {
            var dataCollection = new List<TR01BViewModel>();
            var dataList = _Service.GetAllD(TR01A_ID).ToList();
            if (dataList.Count > 0)
            {
                foreach (var item in dataList)
                {
                    dataCollection.Add(new TR01BViewModel(item));
                }
            }

            return dataCollection.OrderByDescending(x => x.CREATE_DATE).ToList();
        }
        #endregion Private Funcitons
    }
}