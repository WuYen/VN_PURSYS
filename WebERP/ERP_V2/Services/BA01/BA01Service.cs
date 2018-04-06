using ERP_V2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace ERP_V2.Services.BA01
{
    public class BA01Service : IServiceMD<MD, BA01A, BA01B, int, int>, IDisposable
    {
        private PURSysEntities _Entity;

        public BA01Service()
        {
            _Entity = new PURSysEntities();
        }

        public BA01A GetM(Expression<Func<BA01A, bool>> predicate)
        {
            return _Entity.BA01A.Where(predicate).FirstOrDefault();
        }
        public BA01A GetM(int key)
        {
            return _Entity.BA01A.Where(x => x.BA01A_ID == key).FirstOrDefault();
        }

        public BA01B GetD(Expression<Func<BA01B, bool>> predicate)
        {
            return _Entity.BA01B.Where(predicate).FirstOrDefault();
        }
        public BA01B GetD(int key)
        {
            return _Entity.BA01B.Where(x => x.BA01B_ID == key).FirstOrDefault();
        }

        public IQueryable<BA01A> GetAllM()
        {
            return _Entity.BA01A.AsQueryable();
        }

        public IQueryable<BA01B> GetAllD(int key)
        {
            return _Entity.BA01B.Where(x => x.BA01A_ID == key);
        }

        public MD GetMD(int key)
        {
            var data = new MD()
            {
                Master = _Entity.BA01A.Where(x => x.BA01A_ID == key).FirstOrDefault(),
                DetaiList = _Entity.BA01B.Where(x => x.BA01A_ID == key).ToList()
            };
            return data;
        }

        public ServiceResult<BA01A> UpdateM(BA01A entity)
        {
            var errMsg = "";
            using (var trans = _Entity.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    if (entity.BA01A_ID == 0)
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
            return new ServiceResult<BA01A>() { Data = entity, Message = errMsg };
        }

        /// <summary>寫Log的時候要先去update</summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public string DeleteM(BA01A data)
        {
            string errMsg = "";
            using (var trans = _Entity.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    //Delete detail first
                    var detailList = _Entity.BA01B.Where(x => x.BA01A_ID == data.BA01A_ID).ToList();
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

        public string UpdateMD(BA01A entity, List<BA01B> CreateD, List<BA01B> UpdateD, List<int> DeleteD)
        {
            string errMsg = "";
            using (var trans = _Entity.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    if (entity.BA01A_ID == 0)
                    {
                        _Entity.Entry(entity).State = EntityState.Added;
                    }
                    else
                    {
                        _Entity.Entry(entity).State = EntityState.Modified;
                    }
                    _Entity.SaveChanges();

                    //detail 處理 BA01A_ID
                    foreach (var item in CreateD)
                    {
                        item.BA01A_ID = entity.BA01A_ID;
                        _Entity.Entry(item).State = EntityState.Added;
                    }
                    foreach (var item in UpdateD)
                    {
                        _Entity.Entry(item).State = EntityState.Modified;
                    }
                    foreach (var key in DeleteD)
                    {
                        var deleteData = new BA01B { BA01B_ID = key };
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
        public BA01A Master { get; set; }

        public List<BA01B> DetaiList { get; set; }
    }
}
