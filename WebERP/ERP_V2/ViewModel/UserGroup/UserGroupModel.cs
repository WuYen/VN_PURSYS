using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ERP_V2.Models;

namespace ERP_V2.ViewModel.UserGroup
{
    [MetadataType(typeof(UserGroup_MD))]
    public class UserGroupModel : Models.UserGroup
    {
    }


    public class UserGroup_MD
    {
        //[Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Utility.Language))]
        //public string Account { get; set; }
    }

}