using ERP_V2.Models;
using ERP_V2.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace ERP_V2.ViewModels.BA04
{
    public class BA04Service
    {
        public PURSysEntities _Entity;
        public BA04Service()
        {
            _Entity = new PURSysEntities();
        }
        public BA04Service(PURSysEntities entity)
        {
            _Entity = entity;
        }

        /// <summary>用ID找單筆</summary>
        /// <param name="BA04A_ID">key: BA04A_ID</param>
        /// <returns>FA11A</returns>
        public BA04A GetA(int BA04A_ID)
        {
            return _Entity.BA04A.Where(x => x.BA04A_ID == BA04A_ID).FirstOrDefault();
        }

        /// <summary>直接下sql查詢</summary>
        /// <param name="sqlCmd">查詢字串</param>
        /// <returns>List<BA04A></returns>
        public List<BA04A> GetA(string sqlCmd)
        {
            return _Entity.BA04A.SqlQuery(sqlCmd).Take(1000).ToList();
        }

        /// <summary>自組linq取得資料 回去再 ToList() or FirstOrDefault()</summary>
        /// <param name="predicate">自組linq取得資料 expression</param>
        /// <returns>IQueryable<BA04A></returns>
        public IQueryable<BA04A> GetA(Expression<Func<BA04A, bool>> predicate)
        {
            return _Entity.BA04A.Where(predicate);
        }


        public ServiceResult<BA04A> Update(BA04A entity)
        {
            var errMsg = "";
            using (var trans = _Entity.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    if (entity.BA04A_ID == 0)
                    {
                        _Entity.Entry(entity).State = EntityState.Added;
                    }
                    else
                    {
                        _Entity.Entry(entity).State = EntityState.Modified;
                    }
                    var result = _Entity.SaveChanges();
                    trans.Commit();
                    CacheHelper.Invalidate("BA04A");
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
            return new ServiceResult<BA04A>() { Data = entity, Message = errMsg };
        }

        /// <summary>刪除</summary>
        /// <param name="key">Master 的 Key</param>
        /// <returns>被trigger擋下=> 取回資料, 被其他人刪單=> new BA04A</returns>
        public ServiceResult<BA04A> Delete(int key)
        {
            string errMsg = "";
            var master = _Entity.BA04A.FirstOrDefault(x => x.BA04A_ID == key);
            using (var trans = _Entity.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    if (master != null && master.BA04A_ID > 0)
                    {
                        _Entity.BA04A.Remove(master);
                        _Entity.SaveChanges();
                        CacheHelper.Invalidate("BA04A");
                    }
                    else
                    {
                        errMsg = "資料已被刪除";//被其他人刪單 ErrorHandler.GetCodeName("W011");
                        master = new BA04A();
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
            return new ServiceResult<BA04A>() { Data = master, Message = errMsg };
        }
    }
}