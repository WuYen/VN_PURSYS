using ActWeis.Utility;
using ERP_V2.Models;
using ERP_V2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_V2.ViewModels.BA04
{
    public class BA04Business
    {
        /// <summary> model 初始化 </summary>
        /// <param name="data"></param>
        public static void SetInitValue(ref BA04AModel data)
        {

        }

        /// <summary> 驗證資料 </summary>
        /// <param name="model"></param>
        /// <param name="modelState"></param>
        /// <param name="isBefore">是否在BeforeSave前面 - 前面驗證 [資料刪除/Metadata/邏輯] / 後面驗證 [DDM]  </param>
        /// <returns></returns>
        public static List<string> Validation(BA04AModel model, ModelStateDictionary modelState)
        {
            List<string> errMsgList = new List<string>();
            Dictionary<string, string> masterErrDic = new Dictionary<string, string>();

            #region [ 驗證資料是否已被刪除 ]
            //不是新增的時候要去驗證是否有被異動過
            if (model.BA04A_ID > 0)
            {
                BA04Service _Service = new BA04Service();
                var count = _Service.GetA(x => x.BA04A_ID == model.BA04A_ID).Count();
                if (count == 0)
                {
                    errMsgList.Add("資料已被其他使用者刪除");
                }
            }
            #endregion

            #region [ master - 邏輯]
            //if (masterErrDic.Where(x => x.Key == "CUS_ID").Count() == 0 && new BA04Service().GetA(x => x.BA04A_ID != model.BA04A_ID && x.CUS_ID == model.CUS_ID).Count() > 0)
            //{
            //    masterErrDic.Add("CUS_ID", string.Format(CommonHelper.GetCodeName("W021"), DDMHelper.GetColName("BA04A", "CUS_ID")));
            //}
            #endregion [ master - 邏輯]

            VallidationResult(modelState, errMsgList, masterErrDic);

            return errMsgList;
        }

        public static List<string> Validation(BA04AModel model, ModelStateDictionary modelState, string tableName)
        {
            List<string> errMsgList = new List<string>();
            Dictionary<string, string> masterErrDic = new Dictionary<string, string>();

            //驗證DDM
            //masterErrDic = DDMHelper.ValidateData(tableName, model);

            VallidationResult(modelState, errMsgList, masterErrDic);

            return errMsgList;
        }

        private static void VallidationResult(ModelStateDictionary modelState, List<string> errMsgList, Dictionary<string, string> masterErrDic)
        {
            if (masterErrDic.Count > 0)
            {
                //errMsgList.Add(CommonHelper.GetCodeName("W013"));
                foreach (var item in masterErrDic)
                {
                    modelState.AddModelError(item.Key, item.Value);
                }
            }

            if (!modelState.IsValid)
            {
                errMsgList.Add("資料有錯請修正");
            }
        }

        #region BA04A Object Convert
        public static BA04AModel FromEntity(BA04A entity)
        {
            var data = new BA04AModel();
            if (entity != null)
            {
                var objectHelper = new ObjectHelper();
                objectHelper.CopyValue(entity, data);

                #region [ 資料處理 ]                                
                #endregion [ 資料處理 ]
            }
            return data;
        }

        public static List<BA04AModel> FromEntity(List<BA04A> entityList)
        {
            List<BA04AModel> list = new List<BA04AModel>();

            if (entityList != null && entityList.Count > 0)
            {
                foreach (var item in entityList)
                {
                    list.Add(FromEntity(item));
                }
            }
            return list;
        }

        /// <summary>
        /// viewmodel to entity
        /// </summary>
        /// <param name="entity"></param>
        private static BA04A ToEntity(BA04AModel data)
        {
            var entity = new BA04A();
            if (data != null)
            {
                #region [ 資料處理 ]

                #endregion [ 資料處理 ]

                var objectHelper = new ObjectHelper();
                objectHelper.CopyValue(data, entity);
            }
            return entity;
        }

        #endregion


        public static BA04A BeforSave(BA04AModel data)
        {
            var BA04A = new BA04A();
            if (data.BA04A_ID == 0)//Adding New
            {
                data.CREATE_USER = UserInfo.Id;
                data.CREATE_DATE = DateTime.Now;
            }
            else//Update
            {
                var _Service = new BA04Service();
                BA04A = _Service.GetA(x => x.BA04A_ID == data.BA04A_ID).First();
                data.CREATE_USER = BA04A.CREATE_USER;
                data.CREATE_DATE = BA04A.CREATE_DATE;
                data.UPDATE_USER = UserInfo.Id;
                data.UPDATE_DATE = DateTime.Now;
            }

            return ToEntity(data);
        }
    }
}