using DevExpress.Web.Mvc;
using ERP_V2.Models;
using ERP_V2.Utility;
using ERP_V2.ViewModels.TR01V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_V2.ViewModels.TR01V2
{
    public class TR01Business
    {
        #region Validation
        public static bool Validation(TR01AViewModel model, ModelStateDictionary modelState)
        {
            //if (model.REM_MM != null && model.REM_MM.Length > 10)
            //{
            //    modelState.AddModelError("REM_MM", "REM_MM Too long From Business layer");
            //}
            if (model.TAX_RT < 0)
            {
                modelState.AddModelError("TAX_RT", "Error");
            }
            return modelState.IsValid;
        }

        public static bool Validation(TR01AViewModel model, MVCxGridViewBatchUpdateValues<TR01BViewModel, int> updateValues, ModelStateDictionary modelState)
        {
            if (model.TAX_RT < 0)
            {
                modelState.AddModelError("TAX_RT", "Error");
            }

            for (int i = 0; i < updateValues.Insert.Count; i++)
            {
                //if (updateValues.Insert[i].ITM_NO == null || updateValues.Insert[i].ITM_NO.Length < 1)
                //{
                //    modelState.AddModelError($"Insert[{i}].ITM_NO", "必填");
                //}
                if (updateValues.Insert[i].ITM_NM == null || updateValues.Insert[i].ITM_NM.Length < 1)
                {
                    modelState.AddModelError($"Insert[{i}].ITM_NM", "必填");
                }

                if (updateValues.Insert[i].PUR_QT < 0)
                {
                    modelState.AddModelError($"Insert[{i}].PUR_QT", "不可以小於零");
                }
                if (updateValues.IsValid(updateValues.Insert[i]) == false)
                {
                    modelState.AddModelError($"Insert[{i}]", "error");
                }
            }

            for (int i = 0; i < updateValues.Update.Count; i++)
            {
                //測試用
                //if (updateValues.Update[i].REM_MM != null && updateValues.Update[i].REM_MM.Length > 10)
                //{
                //    modelState.AddModelError($"Update[{i}].REM_MM", "REM_MM太長");
                //}
                if (updateValues.Update[i].ITM_NM == null || updateValues.Update[i].ITM_NM.Length < 1)
                {
                    modelState.AddModelError($"Update[{i}].ITM_NM", "必填");
                }
                if (updateValues.Update[i].PUR_QT < 0)
                {
                    modelState.AddModelError($"Update[{i}].PUR_QT", "不可以小於零");
                }

                if (updateValues.IsValid(updateValues.Update[i]) == false)
                {
                    modelState.AddModelError($"Update[{i}]", "error");
                }
            }

            return modelState.IsValid;
        }

        public static bool ValidateRemove(TR01AViewModel model, List<TR01BViewModel> updateDetail)
        {
            if (model.TR01A_ID > 0)
            {
                TR01Service _Service = new TR01Service();
                var hasMaster = _Service.GetA(model.TR01A_ID);
                if (hasMaster == null)
                {
                    return false;
                }

                if (updateDetail != null && updateDetail.Count > 0)
                {
                    var TR01B_IDList = updateDetail.Select(x => x.TR01B_ID).ToList();
                    PURSysEntities _Entity = new PURSysEntities();
                    var DBCount = _Entity.TR01B.Where(x => TR01B_IDList.Contains(x.TR01B_ID)).Count();
                    if (DBCount != TR01B_IDList.Count())
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        #endregion

        #region TR01A Object Convert
        /// <summary>Entity to viewmodel Single</summary>
        /// <param name="entity">TR01A</param>
        public static TR01AViewModel FromEntity(TR01A entity)
        {
            var data = new TR01AViewModel();
            if (entity != null)
            {
                var objectHelper = new ActWeis.Utility.ObjectHelper();
                objectHelper.CopyValue(entity, data);

                #region [ 資料處理 ]
                var BA01A = CacheCommonDataModule.GetBA01A().FirstOrDefault(x => x.BA01A_ID == entity.BA01A_ID);
                if (BA01A != null)
                {
                    data.FAX_NO = BA01A.FAX_NO;
                    data.ADD_DR = BA01A.ADD_DR;
                }

                var BA01B = new Services.BA01.BA01Service().GetD(x => x.BA01B_ID == entity.BA01B_ID);
                if (BA01B != null)
                {
                    data.TEL_NO = BA01B.TEL_NO;
                    data.TEL_EX = BA01B.TEL_EX;
                }
                #endregion [ 資料處理 ]
            }
            return data;
        }

        /// <summary>Entity to viewmodel Batch</summary>
        /// <param name="entityList">List<TR01A></param>
        /// <returns>List<TR01AViewModel></returns>
        public static List<TR01AViewModel> FromEntity(List<TR01A> entityList)
        {
            List<TR01AViewModel> list = new List<TR01AViewModel>();

            if (entityList != null && entityList.Count > 0)
            {
                foreach (var item in entityList)
                {
                    list.Add(FromEntity(item));
                }
            }
            return list;
        }

        /// <summary>viewmodel to entity </summary>
        /// <param name="entity"></param>
        private static TR01A ToEntity(TR01AViewModel data)
        {
            var entity = new TR01A();
            if (data != null)
            {
                #region [ 資料處理 ]

                #endregion [ 資料處理 ]

                var objectHelper = new ActWeis.Utility.ObjectHelper();
                objectHelper.CopyValue(data, entity);
            }
            return entity;
        }
        #endregion

        #region TR01B Object Convert
        /// <summary>Entity to viewmodel Single</summary>
        /// <param name="entity">TR01B</param>
        public static TR01BViewModel FromEntity(TR01B entity)
        {
            var data = new TR01BViewModel();
            if (entity != null)
            {
                var objectHelper = new ActWeis.Utility.ObjectHelper();
                objectHelper.CopyValue(entity, data);

                //資料處理
                var item = CacheCommonDataModule.GetBA02A().Where(x => x.BA02A_ID == entity.BA02A_ID).FirstOrDefault();
                if (item != null)
                {
                    data.ITM_NO = item.ITM_NO;
                    data.ITM_NM = item.ITM_NM;
                    data.ITM_SP = item.ITM_SP;
                }
            }
            return data;
        }

        public static List<TR01BViewModel> FromEntity(List<TR01B> entityList)
        {
            List<TR01BViewModel> list = new List<TR01BViewModel>();

            if (entityList != null && entityList.Count > 0)
            {
                foreach (var item in entityList)
                {
                    list.Add(FromEntity(item));
                }
            }
            return list;
        }

        /// <summary>viewmodel to entity </summary>
        /// <param name="entity"></param>
        private static TR01B ToEntity(TR01BViewModel data)
        {
            var entity = new TR01B();
            if (data != null)
            {
                //資料處理

                var objectHelper = new ActWeis.Utility.ObjectHelper();
                objectHelper.CopyValue(data, entity);
            }
            return entity;
        }
        #endregion

        //==========================BeforSave==========================
        public static TR01A BeforSave(TR01AViewModel data)
        {
            if (data.TR01A_ID == 0)//Adding New
            {
                data.PUR_NO = new PURSysEntities().Database.SqlQuery<string>("select dbo.Get_PUR_NO()").FirstOrDefault();
                data.CREATE_USER = UserInfo.Id;
                data.CREATE_DATE = DateTime.Now;
                data.CFN_YN = "N";
            }
            else//Update
            {
                TR01Service _Service = new TR01Service();
                var TR01A = _Service.GetA(data.TR01A_ID);
                data.CREATE_USER = TR01A.CREATE_USER;
                data.CREATE_DATE = TR01A.CREATE_DATE;
                data.UPDATE_USER = UserInfo.Id;
                data.UPDATE_DATE = DateTime.Now;
            }
            return ToEntity(data);
        }

        public static List<TR01B> BeforSave(List<TR01BViewModel> data, EditType editType)
        {
            List<TR01B> list = new List<TR01B>();

            if (editType == EditType.AddNew)
            {
                foreach (var item in data)
                {
                    item.CREATE_USER = UserInfo.Id;
                    item.CREATE_DATE = DateTime.Now;
                    list.Add(ToEntity(item));
                }
            }
            else
            {
                TR01Service _Service = new TR01Service();
                var DBTR01BList = _Service.GetB(data.Select(x => x.TR01B_ID).ToList());
                foreach (var item in DBTR01BList)
                {
                    var tempData = data.First(x => x.TR01B_ID == item.TR01B_ID);
                    tempData.TR01A_ID = item.TR01A_ID;
                    tempData.CREATE_USER = item.CREATE_USER;
                    tempData.CREATE_DATE = item.CREATE_DATE;
                    tempData.UPDATE_USER = UserInfo.Id;
                    tempData.UPDATE_DATE = DateTime.Now;
                    list.Add(ToEntity(tempData));
                }
            }
            return list;
        }
    }
}