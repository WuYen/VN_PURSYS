using ERP_V2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace ERP_V2.Services.BA02
{
    public class BA02Service : IServiceMD<MD, BA02A, BA02B, int, int>, IDisposable
    {
        private PURSysEntities _Entity;

        public BA02Service()
        {
            _Entity = new PURSysEntities();
        }

        public BA02A GetM(Expression<Func<BA02A, bool>> predicate)
        {
            return _Entity.BA02A.Where(predicate).FirstOrDefault();
        }
        public BA02A GetM(int key)
        {
            return _Entity.BA02A.Where(x => x.BA02A_ID == key).FirstOrDefault();
        }

        public BA02B GetD(Expression<Func<BA02B, bool>> predicate)
        {
            return _Entity.BA02B.Where(predicate).FirstOrDefault();
        }
        public BA02B GetD(int key)
        {
            return _Entity.BA02B.Where(x => x.BA02B_ID == key).FirstOrDefault();
        }

        public IQueryable<BA02A> GetAllM()
        {
            return _Entity.BA02A.AsQueryable();
        }

        public IQueryable<BA02B> GetAllD(int key)
        {
            return _Entity.BA02B.Where(x => x.BA02A_ID == key);
        }

        public MD GetMD(int key)
        {
            var data = new MD()
            {
                Master = _Entity.BA02A.Where(x => x.BA02A_ID == key).FirstOrDefault(),
                DetaiList = _Entity.BA02B.Where(x => x.BA02A_ID == key).ToList()
            };
            return data;
        }

        public ServiceResult<BA02A> UpdateM(BA02A entity)
        {
            var errMsg = "";
            using (var trans = _Entity.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    if (entity.BA02A_ID == 0)
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
            return new ServiceResult<BA02A>() { Data = entity, Message = errMsg };
        }

        /// <summary>寫Log的時候要先去update</summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string DeleteM(BA02A data)
        {
            string errMsg = "";
            using (var trans = _Entity.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    //Delete detail first
                    var detailList = _Entity.BA02B.Where(x => x.BA02A_ID == data.BA02A_ID).ToList();
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

        public string UpdateMD(BA02A entity, List<BA02B> CreateD, List<BA02B> UpdateD, List<int> DeleteD)
        {
            string errMsg = "";
            using (var trans = _Entity.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    if (entity.BA02A_ID == 0)
                    {
                        _Entity.Entry(entity).State = EntityState.Added;
                    }
                    else
                    {
                        _Entity.Entry(entity).State = EntityState.Modified;
                    }
                    _Entity.SaveChanges();

                    //detail 處理 BA02A_ID
                    foreach (var item in CreateD)
                    {
                        item.BA02A_ID = entity.BA02A_ID;
                        _Entity.Entry(item).State = EntityState.Added;
                    }
                    foreach (var item in UpdateD)
                    {            
                        _Entity.Entry(item).State = EntityState.Modified;
                    }
                    foreach (var key in DeleteD)
                    {
                        var deleteData = new BA02B { BA02B_ID = key };
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
        public BA02A Master { get; set; }

        public List<BA02B> DetaiList { get; set; }
    }
}