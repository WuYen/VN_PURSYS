using ActWeis.Utility;
using ERP_V2.Models;
using ERP_V2.Utility;
using ERP_V2.ViewModel.SysUser;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ERP_V2.Services.SysUser
{
    public class SysUserBusiness
    {
        public string BeforeSave(SysUserModel model, ref SystemUser entity, EntityState state)
        {
            var errMsg = "";
            if (state == EntityState.Added) //新增
            {
                ToEntity(model, ref entity);
        
            }
            else if (state == EntityState.Modified)//修改
            {
                errMsg = entity.ID == 0 ? Language.DataHasDeleted + "<br/>" : "";
                if (errMsg == "")
                    ToEntity(model, ref entity);
            }
            else //刪除
            {
                errMsg = entity.ID == 0 ? Language.DataHasDeleted + "<br/>" : "";
            }

            return errMsg;
        }

        private void ToEntity(SysUserModel model, ref SystemUser entity)
        {
            var ignoreList = new List<string>();
            ignoreList.Add("CREATE_USER");
            ignoreList.Add("CREATE_DATE");
            ignoreList.Add("LASTUPDATE_USER");
            ignoreList.Add("LASTUPDATE_DATE");

            new ObjectHelper().CopyValue(model, entity, ignoreList);
            if (entity.ID == 0)
            {
                entity.CREATE_USER = UserInfo.Id;
                entity.CREATE_DATE = DateTime.Now;
            }
            else
            {
                entity.UPDATE_DATE = DateTime.Now;
                entity.UPDATE_USER = UserInfo.Id;
            }
            if (model.Enabled == "True")
            {
                entity.Enabled = "Y";
            }
            else
            {
                entity.Enabled = "N";
            }
        }
    }
}