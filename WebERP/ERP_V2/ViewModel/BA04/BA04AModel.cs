using ERP_V2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_V2.ViewModels.BA04
{
    [MetadataType(typeof(BA04A_MD))]
    public class BA04AModel : BA04A
    {
        public ModelStateDictionary ModelState { get; set; }
    }

    public class BA04A_MD
    {
        //[Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "enterPassword")]
        //public string Password { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequireErrMsg")]
        public string TYP_CN { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequireErrMsg")]
        public string TYP_VN { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequireErrMsg")]
        public string TYP_US { get; set; }
    }
}