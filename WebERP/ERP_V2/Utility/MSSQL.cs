using ERP_V2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ERP_V2.Utility
{
    public class MSSQL
    {
        /// <summary>取得MSSQL錯誤訊息</summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public string GetSQLMessage(System.Runtime.InteropServices._Exception exception)
        {
            if (exception.InnerException != null)
            {
                return GetSQLMessage(exception.InnerException);
            }
            else
            {
                return exception.Message;
            }
        }

        /// <summary>取得Entity欄位資料驗證錯誤訊息</summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public string GetDbEntityValidationExceptionMessage(System.Data.Entity.Validation.DbEntityValidationException ex)
        {
            var errMsg = string.Empty;
            foreach (var eve in ex.EntityValidationErrors)
            {
                errMsg = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                foreach (var ve in eve.ValidationErrors)
                {
                    errMsg += "<br />" + string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                }
            }

            return errMsg;
        }

        public void SQLCommandReader(string command, DataSet ds)
        {
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
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = command;
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
        }
    }
}