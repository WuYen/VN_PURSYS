using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ERP_V2.Models;

namespace ERP_V2.ViewModel.SysUser
{
    [MetadataType(typeof(SystemUser_MD))]
    public class SysUserModel : SystemUser
    {
    }

    public class SystemUser_MD
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Utility.Language))]
        public string Account { get; set; }
    }
}