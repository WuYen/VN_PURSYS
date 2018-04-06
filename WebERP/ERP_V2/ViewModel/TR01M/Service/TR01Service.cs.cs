using ERP_V2.Models;
using ERP_V2.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;


namespace ERP_V2.ViewModels.TR01M
{
    public class TR01Service : ERP_V2.ViewModels.TR01V2.TR01Service
    {
        private PURSysEntities _Entity;
        public TR01Service()
        {
            _Entity = new PURSysEntities();
        }

        #region GetC
        /// <summary>用Master ID找Detail資料</summary>
        /// <param name="TR01A_ID">master key</param>
        /// <returns> List<TR01C> </returns>
        public List<TR01C> GetC(int TR01B_ID)
        {
            return _Entity.TR01C.Where(x => x.TR01B_ID == TR01B_ID).OrderByDescending(x => x.CREATE_DATE).ToList();
        }

        /// <summary>自己下linq找資料 回去再看要 ToList() or FirstOrDefault()</summary>
        /// <param name="predicate">linq expression</param>
        /// <returns>IQueryable</returns>
        public IQueryable<TR01C> GetC(Expression<Func<TR01C, bool>> predicate)
        {
            return _Entity.TR01C.Where(predicate);
        }

        /// <summary>Detail update 查詢的時候先找出舊的資料進行object copy</summary>
        /// <param name="IDlist">要修改的 ID list</param>
        /// <returns>List<TR01C></returns>
        public List<TR01C> GetC(List<int> IDlist)
        {
            return _Entity.TR01C.Where(r => IDlist.Contains(r.TR01C_ID)).ToList();
        }
        #endregion

        public string Update(List<TR01C> CreateD, List<TR01C> UpdateD, List<int> DeleteD)
        {
            string errMsg = "";
            using (var trans = _Entity.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    foreach (var item in CreateD)
                    {
                        _Entity.Entry(item).State = EntityState.Added;
                    }
                    foreach (var item in UpdateD)
                    {
                        _Entity.Entry(item).State = EntityState.Modified;
                    }
                    _Entity.TR01B.RemoveRange(_Entity.TR01B.Where(r => DeleteD.Contains(r.TR01B_ID)));

                    _Entity.SaveChanges();
                    trans.Commit();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    foreach (var eve in ex.EntityValidationErrors)
                    {
                        errMsg = string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:", eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            errMsg += "<br />" + string.Format("- Property: \"{0}\", Error: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;
                }
            }
            return errMsg;
        }

        public ServiceResult<TR01C> Update(TR01C entity)
        {
            var errMsg = "";
            using (var trans = _Entity.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    if (entity.TR01C_ID == 0)
                    {
                        _Entity.Entry(entity).State = EntityState.Added;
                    }
                    else
                    {
                        _Entity.Entry(entity).State = EntityState.Modified;
                    }
                    var result = _Entity.SaveChanges();
                    trans.Commit();
                    CacheHelper.Invalidate("TR01C");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    errMsg = ErrorHandler.GetDbEntityValidationExceptionMessage(ex);
                }
                catch (Exception ex)
                {
                    errMsg = ErrorHandler.GetSQLMessage(ex);
                }
            }
            return new ServiceResult<TR01C>() { Data = entity, Message = errMsg };
        }

        /// <summary>刪除</summary>
        /// <param name="key">Master 的 Key</param>
        /// <returns>被trigger擋下=> 取回資料, 被其他人刪單=> new TR01C</returns>
        public ServiceResult<TR01C> DeleteC(int key)
        {
            string errMsg = "";
            var master = _Entity.TR01C.FirstOrDefault(x => x.TR01C_ID == key);
            using (var trans = _Entity.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    if (master != null && master.TR01C_ID > 0)
                    {
                        _Entity.TR01C.Remove(master);
                        _Entity.SaveChanges();
                        CacheHelper.Invalidate("TR01C");
                    }
                    else
                    {
                        errMsg = "資料已被刪除";//被其他人刪單 ErrorHandler.GetCodeName("W011");
                        master = new TR01C();
                    }
                    trans.Commit();

                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    errMsg = ErrorHandler.GetDbEntityValidationExceptionMessage(ex);
                }
                catch (Exception ex)
                {
                    errMsg = ErrorHandler.GetSQLMessage(ex);
                }
            }
            return new ServiceResult<TR01C>() { Data = master, Message = errMsg };
        }
        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        new protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this._Entity.Dispose();
                }

                disposedValue = true;
            }
        }

        new public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}