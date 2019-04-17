using ERP_V2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;


namespace ERP_V2.Services.SysUser
{
    public class SysUserService
    {
        private PURSysEntities _Entity;
        public SysUserService()
        {
            _Entity = new PURSysEntities();
        }
        public IQueryable<SystemUser> GetQuerable()
        {
            return _Entity.SystemUser.AsQueryable();
        }

        public SystemUser GetByKey(int key)
        {
            return _Entity.SystemUser.FirstOrDefault(x => x.ID == key) ?? new SystemUser();
        }

        public string SaveChanges(ref SystemUser entity, EntityState state)
        {
            var errMsg = "";
            using (var trans = _Entity.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    if (state == EntityState.Added)
                    {
                        _Entity.SystemUser.Add(entity);
                    }
                    else if (state == EntityState.Deleted)
                    {
                        _Entity.SystemUser.Remove(entity);
                    }
                    _Entity.SaveChanges();
                    trans.Commit();
                    CacheCommonDataModule.ReloadSystemUser();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                {
                    errMsg = ex.Message;
                }
                catch (Exception ex)
                {
                    errMsg = ex.Message;//ValidationHelper.GetSQLMessage(ex);
                }
            }
            return errMsg;
        }
    }
}