﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_V2.Utility
{
    public class ErrorHandler
    {
        /// <summary>取得MSSQL錯誤訊息</summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string GetSQLMessage(System.Runtime.InteropServices._Exception exception)
        {
            if (exception.InnerException != null)
            {
                return GetSQLMessage(exception.InnerException);
            }
            return "Error";
        }

        /// <summary>取得Entity欄位資料驗證錯誤訊息 - W009</summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetDbEntityValidationExceptionMessage(System.Data.Entity.Validation.DbEntityValidationException ex)
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
    }
}