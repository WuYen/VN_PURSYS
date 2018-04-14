using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_V2
{
    [Permissions]
    public class BaseController : Controller
    {
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}