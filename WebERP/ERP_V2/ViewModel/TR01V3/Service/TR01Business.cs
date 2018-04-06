using DevExpress.Web.Mvc;
using ERP_V2.Models;
using ERP_V2.Utility;
using ERP_V2.ViewModels.TR01V3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_V2.ViewModel.TR01V3
{
    public class TR01Business
    {
        public static void SetDefaultValue(ref TR01AViewModel model) { }

        #region Data Validation       
        public static List<string> Validation(TR01AViewModel model, MVCxGridViewBatchUpdateValues<TR01BViewModel, int> updateValues, ModelStateDictionary modelState)
        {
            List<string> errMsg = new List<string>();

            //不是新增的時候要去驗證是否有被異動過
            if (model.TR01A_ID > 0)
            {
                TR01Service _Service = new TR01Service();
                var count = _Service.GetA(x => x.TR01A_ID == model.TR01A_ID).Count();
                if (count == 0)
                {
                    errMsg.Add("已被其他使用者刪除");
                }

                if (updateValues != null && updateValues.Update.Count > 0)
                {
                    var TR01B_IDList = updateValues.Update.Select(x => x.TR01B_ID).ToList();
                    EntityConnection _Entity = new EntityConnection();
                    var DBCount = _Entity.TR01B.Where(x => TR01B_IDList.Contains(x.TR01B_ID)).Count();
                    if (DBCount != TR01B_IDList.Count())
                    {
                        errMsg.Add("部份明細已被其他使用者刪除" + "<br />" + "請重新整理");
                    }
                }
            }

            //驗證Master
            var masterErrDic = new Dictionary<string, string>();
            ValidateMaster(masterErrDic, model);
            if (masterErrDic.Count > 0)
            {
                errMsg.Add("Master contains error");
                foreach (var err in masterErrDic)
                {
                    modelState.AddModelError($"{err.Key}", err.Value);//display error in cell
                }
            }

            //驗證Detail
            if (updateValues != null)
            {
                var detailErrDic = new Dictionary<string, string>();
                for (int i = 0; i < updateValues.Insert.Count; i++)
                {
                    ValidateDetail(i, detailErrDic, updateValues.Insert[i], EditType.AddNew);
                }

                for (int i = 0; i < updateValues.Update.Count; i++)
                {
                    ValidateDetail(i, detailErrDic, updateValues.Update[i], EditType.Update);
                }

                if (detailErrDic.Count > 0)
                {
                    errMsg.Add("Detail contains error");
                    foreach (var item in detailErrDic)
                    {
                        modelState.AddModelError(item.Key, item.Value);
                    }
                }
            }

            if (!modelState.IsValid)
            {
                errMsg.Add("請檢查紅色驚嘆號");
            }

            return errMsg;
        }

        private static void ValidateMaster(Dictionary<string, string> errorDic, TR01AViewModel master)
        {
            if (master.CUR_RT < 1)
            {
                errorDic.Add("CUR_RT", "不可小於1");
            }
        }

        private static void ValidateDetail(int i, Dictionary<string, string> errorDic, TR01BViewModel detail, EditType type)
        {
            string typeStr = type == EditType.Update ? "Update" : "Insert";

            if (detail.ITM_NO != null || detail.ITM_NO.Length < 1)
            {
                errorDic.Add($"{typeStr}[{i}].ITM_NO", "必填");//display error in cell
            }
            if (detail.PUR_QT < 0)
            {
                errorDic.Add($"{typeStr}[{i}].OUT_QT", "不可為負");//display error in cell
            }
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
                data.CFN_YN = "N";
                data.CREATE_USER = UserInfo.Id;
                data.CREATE_DATE = DateTime.Now;
            }
            else//Update
            {
                TR01Service _Service = new TR01Service();
                var TR01A = _Service.GetA(data.TR01A_ID);
                data.CREATE_USER = TR01A.CREATE_USER;
                data.CREATE_DATE = TR01A.CREATE_DATE;
                data.CFN_DT = TR01A.CFN_DT;
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