using ERP_V2.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace ERP_V2.Services.UserGroup
{
    public class UserGroupService
    {
        private PURSysEntities _Entity;

        public UserGroupService()
        {
            _Entity = new PURSysEntities();
        }
        public IQueryable<Models.UserGroup> GetQuerable()
        {
            return _Entity.UserGroup.AsQueryable();
        }

        public Models.UserGroup GetByKey(int key)
        {
            return _Entity.UserGroup.FirstOrDefault(x => x.ID == key) ?? new Models.UserGroup();
        }

        //public List<UserGroupUser> GetGroupUser(int key)
        //{
        //    return _Entity.UserGroupUser.Where(x => x.GroupID == key).ToList();
        //}
        public List<SystemUser> GetGroupUser(int key)
        {
            var userIDs = _Entity.UserGroupUser.Where(x => x.GroupID == key).Select(x => x.UserID);
            return _Entity.SystemUser.Where(x => userIDs.Contains(x.ID)).ToList();
        }
        public List<UserGroupPermission> GetGroupPermissions(int key)
        {
            return _Entity.UserGroupPermission.Where(x => x.UserGroupID == key).ToList();
        }

        /// <summary> 要存master、permission、User </summary>
        public string SaveChanges(ref Models.UserGroup entity, List<int> permissions, List<int> insertUser, List<int> deleteUser, EntityState state)
        {
            var errMsg = "";
            using (var trans = _Entity.Database.BeginTransaction(IsolationLevel.Snapshot))
            {
                try
                {
                    if (state == EntityState.Deleted)
                    {
                        var userGroupID = entity.ID;
                        _Entity.UserGroupPermission.RemoveRange(_Entity.UserGroupPermission.Where(x => x.UserGroupID == userGroupID));//移除權限
                        _Entity.UserGroupUser.RemoveRange(_Entity.UserGroupUser.Where(x => x.GroupID == userGroupID));//移除使用者
                        _Entity.UserGroup.Remove(entity);
                        _Entity.SaveChanges();
                    }
                    else
                    {
                        var userGroupID = entity.ID;
                        if (state == EntityState.Added)
                        {
                            _Entity.UserGroup.Add(entity);
                            _Entity.SaveChanges();
                            userGroupID = entity.ID;
                        }
                        else
                        {
                            //先移除在全部新增回去
                            _Entity.UserGroupPermission.RemoveRange(_Entity.UserGroupPermission.Where(x => x.UserGroupID == userGroupID));
                            _Entity.SaveChanges();
                        }
                        _Entity.UserGroupPermission.AddRange(permissions.Select(x =>
                        {
                            return new UserGroupPermission()
                            {
                                PermissionID = x,
                                UserGroupID = userGroupID
                            };
                        }));
                        _Entity.UserGroupUser.AddRange(insertUser.Select(x =>
                        {
                            return new UserGroupUser()
                            {
                                UserID = x,
                                GroupID = userGroupID
                            };
                        }));
                        _Entity.UserGroupUser.RemoveRange(deleteUser.Select(x =>
                        {
                            return new UserGroupUser()
                            {
                                UserID = x,
                                GroupID = userGroupID
                            };
                        }));
                        _Entity.SaveChanges();
                    }
                    trans.Commit();
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