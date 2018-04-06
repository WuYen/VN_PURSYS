using ERP_V2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace ERP_V2.Services.TR01
{
    public class TR01Service : IServiceMD<MD, TR01A, TR01B, int, int>, IDisposable
    {
        private PURSysEntities _Entity;

        public TR01Service()
        {
            _Entity = new PURSysEntities();
        }

        public TR01A GetM(Expression<Func<TR01A, bool>> predicate)
        {
            return _Entity.TR01A.Where(predicate).FirstOrDefault();
        }
        public TR01A GetM(int key)
        {
            return _Entity.TR01A.Where(x => x.TR01A_ID == key).FirstOrDefault();
        }

        public TR01B GetD(Expression<Func<TR01B, bool>> predicate)
        {
            return _Entity.TR01B.Where(predicate).FirstOrDefault();
        }
        public TR01B GetD(int key)
        {
            return _Entity.TR01B.Where(x => x.TR01B_ID == key).FirstOrDefault();
        }

        public IQueryable<TR01A> GetAllM()
        {
            return _Entity.TR01A.AsQueryable();
        }

        public IQueryable<TR01B> GetAllD(int key)
        {
            return _Entity.TR01B.Where(x => x.TR01A_ID == key);
        }

        public MD GetMD(int key)
        {
            var data = new MD()
            {
                Master = _Entity.TR01A.Where(x => x.TR01A_ID == key).FirstOrDefault(),
                DetaiList = _Entity.TR01B.Where(x => x.TR01A_ID == key).ToList()
            };
            return data;
        }

        public ServiceResult<TR01A> UpdateM(TR01A entity)
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
            }
            return new ServiceResult<TR01A>() { Data = entity, Message = errMsg };
        }

        /// <summary>寫Log的時候要先去update</summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string DeleteM(TR01A data)
        {
            string errMsg = "";
            using (var trans = _Entity.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    //Delete detail first
                    var detailList = _Entity.TR01B.Where(x => x.TR01A_ID == data.TR01A_ID).ToList();
                    foreach (var item in detailList)
                    {
                        item.UPDATE_DATE = data.UPDATE_DATE;
                        item.UPDATE_USER = data.UPDATE_USER;
                        _Entity.Entry(item).State = EntityState.Modified;                        
                    }
                    _Entity.SaveChanges();
                    foreach (var item in detailList)
                    {
                        _Entity.Entry(item).State = EntityState.Deleted;
                    }
                    _Entity.SaveChanges();

                    //Delete Master
                    _Entity.Entry(data).State = EntityState.Deleted;
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
            }
            return errMsg;
        }

        public string UpdateMD(TR01A entity, List<TR01B> CreateD, List<TR01B> UpdateD, List<int> DeleteD)
        {
            string errMsg = "";
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

    public class MD
    {
        public TR01A Master { get; set; }

        public List<TR01B> DetaiList { get; set; }
    }
}