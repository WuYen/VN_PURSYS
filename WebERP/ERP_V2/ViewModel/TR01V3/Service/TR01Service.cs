using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERP_V2.Models;
using System.Linq.Expressions;
using System.Data;
using System.Data.Entity;
using ERP_V2.Utility;

namespace ERP_V2.ViewModel.TR01V3
{
    public class TR01Service
    {
        private EntityConnection _Entity;

        public TR01Service()
        {
            _Entity = new EntityConnection();
        }
        public TR01Service(EntityConnection entity)
        {
            _Entity = entity;
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
            return _Entity.TR01B.Where(x => x.TR01A_ID == TR01A_ID).ToList();
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
                    errMsg = new MSSQL().GetSQLMessage(ex);
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
                        entity.PUR_NO = _Entity.Database.SqlQuery<string>("select dbo.Get_PUR_NO()").FirstOrDefault();
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
                        item.TR01A_ID = entity.TR01A_ID;
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
                    errMsg = new MSSQL().GetSQLMessage(ex);
                }
            }
            return new ServiceResult<TR01A>() { Data = entity, Message = errMsg };
        }

        /// <summary>刪除上下檔</summary>
        /// <param name="key">Master 的 Key</param>
        /// <returns>被trigger擋下=> 取回資料, 被其他人刪單=> new TR01A</returns>
        public ServiceResult<TR01A> Delete(int key)
        {
            string errMsg = "";
            var master = _Entity.TR01A.FirstOrDefault(x => x.TR01A_ID == key);
            using (var trans = _Entity.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    if (master != null && master.TR01A_ID > 0)
                    {
                        _Entity.TR01B.RemoveRange(_Entity.TR01B.Where(r => r.TR01A_ID == key));
                        _Entity.SaveChanges();
                        _Entity.TR01A.Remove(master);
                        _Entity.SaveChanges();
                    }
                    else
                    {
                        errMsg = "已被其他使用者刪除";
                        master = new TR01A();
                    }
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
                    errMsg = new MSSQL().GetSQLMessage(ex);
                }
            }
            return new ServiceResult<TR01A>() { Data = master, Message = errMsg };
        }
    }
}