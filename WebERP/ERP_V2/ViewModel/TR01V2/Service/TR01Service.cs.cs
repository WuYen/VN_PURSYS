using ERP_V2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;


namespace ERP_V2.ViewModels.TR01V2
{
    public class TR01Service
    {
        private PURSysEntities _Entity;
        public TR01Service()
        {
            _Entity = new PURSysEntities();
        }

        #region GetA   
        /// <summary>用ID找單筆</summary>
        /// <param name="TR01A_ID">key: TR01A_ID</param>
        /// <returns>TR01A</returns>
        public TR01A GetA(int TR01A_ID)
        {
            return _Entity.TR01A.Where(x => x.TR01A_ID == TR01A_ID).FirstOrDefault();
        }

        /// <summary>直接下sql查詢</summary>
        /// <param name="sqlCmd">查詢字串</param>
        /// <returns>List<TR01A></returns>
        public List<TR01A> GetA(string sqlCmd)
        {
            return _Entity.TR01A.SqlQuery(sqlCmd).Take(1000).ToList();
        }

        /// <summary>自組linq取得資料 回去再 ToList() or FirstOrDefault()</summary>
        /// <param name="predicate">自組linq取得資料 expression</param>
        /// <returns>IQueryable<TR01A></returns>
        public IQueryable<TR01A> GetA(Expression<Func<TR01A, bool>> predicate)
        {
            return _Entity.TR01A.Where(predicate);
        }
        #endregion

        #region GetB
        /// <summary>用Master ID找Detail資料</summary>
        /// <param name="TR01A_ID">master key</param>
        /// <returns> List<TR01B> </returns>
        public List<TR01B> GetB(int TR01A_ID)
        {
            return _Entity.TR01B.Where(x => x.TR01A_ID == TR01A_ID).OrderByDescending(x => x.CREATE_DATE).ToList();
        }

        /// <summary>自己下linq找資料 回去再看要 ToList() or FirstOrDefault()</summary>
        /// <param name="predicate">linq expression</param>
        /// <returns>IQueryable</returns>
        public IQueryable<TR01B> GetB(Expression<Func<TR01B, bool>> predicate)
        {
            return _Entity.TR01B.Where(predicate);
        }

        /// <summary>Detail update 查詢的時候先找出舊的資料進行object copy</summary>
        /// <param name="IDlist">要修改的 ID list</param>
        /// <returns>List<TR01B></returns>
        public List<TR01B> GetB(List<int> IDlist)
        {
            return _Entity.TR01B.Where(r => IDlist.Contains(r.TR01B_ID)).ToList();
        }
        #endregion

        public ServiceResult<TR01A> Update(TR01A entity)
        {
            var errMsg = "";
            using (var trans = _Entity.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    if (entity.TR01A_ID == 0)
                    {
                        _Entity.Entry(entity).State = EntityState.Added;
                    }
                    else
                    {
                        _Entity.Entry(entity).State = EntityState.Modified;
                    }
                    var result = _Entity.SaveChanges();
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
            return new ServiceResult<TR01A>() { Data = entity, Message = errMsg };
        }

        /// <summary>delete寫Log的時候要先去update</summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ServiceResult<TR01A> Update(TR01A entity, List<TR01B> CreateD, List<TR01B> UpdateD, List<int> DeleteD)
        {
            string errMsg = "";
            using (var trans = _Entity.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    //Master
                    if (entity.TR01A_ID == 0)
                    {
                        _Entity.Entry(entity).State = EntityState.Added;
                    }
                    else
                    {
                        _Entity.Entry(entity).State = EntityState.Modified;
                    }
                    _Entity.SaveChanges();

                    //detail 處理 TR01A_ID
                    foreach (var item in CreateD)
                    {
                        item.TR01A_ID = entity.TR01A_ID;
                        _Entity.Entry(item).State = EntityState.Added;
                    }
                    foreach (var item in UpdateD)
                    {
                        _Entity.Entry(item).State = EntityState.Modified;
                    }
                    //要先update delete的資料
                    foreach (var key in DeleteD)
                    {
                        var deleteData = new TR01B { TR01B_ID = key };
                        _Entity.Entry(deleteData).State = EntityState.Deleted;
                    }
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
            return new ServiceResult<TR01A>() { Data = entity, Message = errMsg };
        }

        /// <summary>寫Log的時候要先去update</summary>
        /// <param name="key">Master 的 Key</param>
        /// <returns>錯誤訊息</returns>
        public string Delete(int key)
        {
            string errMsg = "";
            using (var trans = _Entity.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    _Entity.TR01B.RemoveRange(_Entity.TR01B.Where(r => r.TR01A_ID == key));
                    _Entity.SaveChanges();

                    var deleteData = new TR01A { TR01A_ID = key };
                    _Entity.Entry(deleteData).State = EntityState.Deleted;
                    var result = _Entity.SaveChanges();
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
            }
            return errMsg;
        }

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing)
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}