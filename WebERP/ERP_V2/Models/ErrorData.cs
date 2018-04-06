using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_V2
{
    public class ErrorData<T>
    {
        public ModelStateDictionary ModelState { get; set; }
        public T Data { get; set; }
    }
}