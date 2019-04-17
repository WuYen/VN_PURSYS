using DevExpress.Web.Mvc;
using ERP_V2.Models;
using ERP_V2.Utility;
using ERP_V2.ViewModels.TR01V2;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;

namespace ERP_V2.ViewModels.TR01M
{
    public class TR01Business : ERP_V2.ViewModels.TR01V2.TR01Business
    {
        /// <summary> 驗證資料 </summary>
        /// <param name="model"></param>
        /// <param name="modelState"></param>
        /// <param name="isBefore">是否在BeforeSave前面 - 前面驗證 [資料刪除/Metadata/邏輯] / 後面驗證 [DDM]  </param>
        /// <returns></returns>
        public static List<string> Validation(TR01CModel model, ModelStateDictionary modelState)
        {
            List<string> errMsgList = new List<string>();
            Dictionary<string, string> masterErrDic = new Dictionary<string, string>();

            #region [ 驗證資料是否已被刪除 ]
            //不是新增的時候要去驗證是否有被異動過
            //if (model.TR01A_ID > 0)
            //{
            //    TR01Service _Service = new TR01Service();
            //    var count = _Service.GetA(x => x.TR01A_ID == model.TR01A_ID).Count();
            //    if (count == 0)
            //    {
            //        errMsgList.Add("資料已被其他使用者刪除");
            //    }
            //}
            #endregion

            #region [ master - 邏輯]
            if (model.CUR_RT < 0)
            {
                masterErrDic.Add("CUR_RT", Resources.Resource.GreaterThanZero);
            }  
            var entity = new PURSysEntities();
            var PUR_QT = entity.TR01B.First(x=>x.TR01B_ID == model.TR01B_ID).PUR_QT;

            var tr01cList = entity.TR01C.Where(x => x.TR01B_ID == model.TR01B_ID && x.TR01C_ID != model.TR01C_ID).ToList();//.Sum(x => x.ARR_QT);
            var totalARR_QT = new decimal();
            if (tr01cList.Count > 0)
            {
                totalARR_QT = tr01cList.Sum(x => x.ARR_QT);
            }
            totalARR_QT = totalARR_QT + model.ARR_QT;
            if (totalARR_QT > PUR_QT)
            {
                masterErrDic.Add("ARR_QT", "数量超过");
            }

            if (model.DtARR_DT > DateTime.Now.AddDays(7))
            {
                masterErrDic.Add("DtARR_DT", "日期不可超过一周");
            }

            //if (masterErrDic.Where(x => x.Key == "CUS_ID").Count() == 0 && new TR01Service().GetA(x => x.TR01A_ID != model.TR01A_ID && x.CUS_ID == model.CUS_ID).Count() > 0)
            //{
            //    masterErrDic.Add("CUS_ID", string.Format(CommonHelper.GetCodeName("W021"), DDMHelper.GetColName("TR01A", "CUS_ID")));
            //}
            #endregion [ master - 邏輯]

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
        public static bool Validation(MVCxGridViewBatchUpdateValues<TR01CModel, int> updateValues, ModelStateDictionary modelState)
        {
            for (int i = 0; i < updateValues.Insert.Count; i++)
            {
                //if (updateValues.Insert[i].ARR_QT < 1)
                //{
                //    modelState.AddModelError($"Insert[{i}].ARR_QT", "> 0");
                //}
                if (updateValues.Insert[i].ARR_QT < 0)
                {
                    modelState.AddModelError($"Insert[{i}].ARR_QT", "不可以小於零");
                }
                if (updateValues.IsValid(updateValues.Insert[i]) == false)
                {
                    modelState.AddModelError($"Insert[{i}]", "error");
                }
            }

            for (int i = 0; i < updateValues.Update.Count; i++)
            {
                //if (updateValues.Update[i].ITM_NO == null || updateValues.Update[i].ITM_NO.Length < 1)
                //{
                //    modelState.AddModelError($"Update[{i}].ITM_NO", "必填");
                //}
                if (updateValues.Update[i].ARR_QT < 0)
                {
                    modelState.AddModelError($"Update[{i}].ARR_QT", "不可以小於零");
                }

                if (updateValues.IsValid(updateValues.Update[i]) == false)
                {
                    modelState.AddModelError($"Update[{i}]", "error");
                }
            }

            return modelState.IsValid;
        }


        new public static List<TR01AModel> FromEntity(List<TR01A> entityList)
        {
            List<TR01AModel> list = new List<TR01AModel>();

            if (entityList != null && entityList.Count > 0)
            {
                PURSysEntities entity = new PURSysEntities();
                foreach (var item in entityList)
                {

                    list.Add(FromEntity(item, entity));
                }
            }
            return list;
        }
        public static TR01AModel FromEntity(TR01A entity, PURSysEntities entityConnection)
        {
            var data = new TR01AModel();
            if (entity != null)
            {
                var objectHelper = new ActWeis.Utility.ObjectHelper();
                objectHelper.CopyValue(entity, data);
            }

            var result = entityConnection.Database.SqlQuery<int>($"select dbo.Get_Not_Receive_Count({entity.TR01A_ID})").FirstOrDefault();
            data.IsAllReceive = result == 0;
            return data;
        }

        public static TR01CModel FromEntity(TR01C entity)
        {
            var data = new TR01CModel();
            if (entity != null)
            {
                var objectHelper = new ActWeis.Utility.ObjectHelper();
                objectHelper.CopyValue(entity, data);

                if (data.INV_MY.HasValue)
                {
                    data.INV_MY = decimal.Parse(data.INV_MY.Value.ToString("G29"));
                }

                data.ARR_QT = decimal.Parse(data.ARR_QT.ToString("G29"));
                data.CUR_RT = decimal.Parse(data.CUR_RT.ToString("G29"));
                //資料處理
                //var item = CacheCommonDataModule.GetBA02A().Where(x => x.BA02A_ID == entity.BA02A_ID).FirstOrDefault();
                //if (item != null)
                //{
                //    data.ITM_NO = item.ITM_NO;
                //    data.ITM_NM = item.ITM_NM;
                //}
            }
            return data;
        }

        public static List<TR01CModel> FromEntity(List<TR01C> entityList)
        {
            List<TR01CModel> list = new List<TR01CModel>();

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
        private static TR01C ToEntity(TR01CModel data)
        {
            //entity = data;
            var entity = new TR01C();
            if (data != null)
            {
                //資料處理

                var objectHelper = new ActWeis.Utility.ObjectHelper();
                objectHelper.CopyValue(data, entity);
            }
            return entity;
        }


        new public static List<TR01BModel> FromEntity(List<TR01B> entityList)
        {
            List<TR01BModel> list = new List<TR01BModel>();

            if (entityList != null && entityList.Count > 0)
            {
                var result = GetData(entityList.First().TR01A_ID);
                foreach (var item in entityList)
                {
                    var row = result.Select("TR01B_ID = " + item.TR01B_ID).First();
                    list.Add(FromEntity(item, row));
                }
            }
            return list;
        }

        private static DataTable GetData(int TR01A_ID)
        {
            string sqlCmd = $"SELECT A.TR01B_ID, Sum(B.ARR_QT) TotalQT , Sum(B.INV_MY) TotalMY FROM[DB_A2BD6B_PandY].[dbo].[TR01B] as A left join[DB_A2BD6B_PandY].[dbo].[TR01C] as B on A.TR01B_ID = B.TR01B_ID where A.TR01A_ID = {TR01A_ID} group by A.TR01B_ID";

            var ds = new DataSet();

            using (var _Entity = new PURSysEntities())
            {
                var conn = _Entity.Database.Connection;
                var connectionState = conn.State;
                if (connectionState != ConnectionState.Open)
                {
                    conn.Open();
                }

                using (var cmd = conn.CreateCommand())
                {
                    //cmd.Parameters.AddRange(sqlParameterList.ToArray());
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sqlCmd;
                    cmd.CommandTimeout = 60;
                    using (var reader = cmd.ExecuteReader())
                    {
                        do
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            ds.Tables.Add(dt);
                        }
                        while (!reader.IsClosed);
                    }
                }
            }
            return ds.Tables[0];
        }

        public static TR01BModel FromEntity(TR01B entity, DataRow dataRow)
        {
            var data = new TR01BModel();
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
                var totalQTSTR = dataRow["TotalQT"].ToString();
                decimal.TryParse(totalQTSTR, out decimal total);
                data.ARR_QT_Sum = total;
                if (data.ARR_QT_Sum == data.PUR_QT)
                {
                    data.ARR_ST = "1";
                }
                else
                {
                    data.ARR_ST = "0";
                }

                var totalMYStr = dataRow["TotalMY"].ToString();
                decimal.TryParse(totalMYStr, out decimal totalMY);
                data.ReceiveMY = totalMY;
            }
            return data;
        }
        //==========================BeforSave==========================

        public static List<TR01C> BeforSave(List<TR01CModel> data, EditType editType)
        {
            List<TR01C> list = new List<TR01C>();

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
                var DBTR01CList = _Service.GetC(data.Select(x => x.TR01C_ID).ToList());
                foreach (var item in DBTR01CList)
                {
                    var tempData = data.First(x => x.TR01C_ID == item.TR01C_ID);
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

        public static TR01C BeforSave(TR01CModel data)
        {
            var TR01C = new TR01C();
            if (data.TR01C_ID == 0)//Adding New
            {
                data.CREATE_USER = UserInfo.Id;
                data.CREATE_DATE = DateTime.Now;
            }
            else//Update
            {
                var _Service = new TR01Service();
                TR01C = _Service.GetC(x => x.TR01C_ID == data.TR01C_ID).First();
                data.CREATE_USER = TR01C.CREATE_USER;
                data.CREATE_DATE = TR01C.CREATE_DATE;
            }
            data.UPDATE_USER = UserInfo.Id;
            data.UPDATE_DATE = DateTime.Now;

            return ToEntity(data);
        }
    }
}