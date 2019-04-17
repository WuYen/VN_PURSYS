using ActWeis.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ERP_V2.Models;
using ERP_V2.ViewModel.UserGroup;

namespace ERP_V2.Services.UserGroup
{
    public class UserGroupBusiness
    {
        public static List<string> IgnoreList = new List<string>()
        {
            "ID","CREATE_DATE","CREATE_USER","UPDATE_DATE","UPDATE_USER"
        };

        public static ViewModel.UserGroup.UserGroupModel FromEntity(Models.UserGroup entity)
        {
            var data = new ViewModel.UserGroup.UserGroupModel();
            if (entity != null)
            {
                var objectHelper = new ObjectHelper();
                objectHelper.CopyValue(entity, data);
            }
            return data;
        }

        public static void ToEntity(ref Models.UserGroup entity, UserGroupModel source)
        {

            if (source != null)
            {
                var objectHelper = new ObjectHelper();
                objectHelper.CopyValue(source, entity, IgnoreList);
            }

        }
    }
}