using ERP_V2.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_V2.ViewModels.Login
{
    public class LoginViewModel
    {
        public string USR_ID { get; set; }
        public string USR_PW { get; set; }
        public Language.Type Lang_TP { get; set; }        
    }
}