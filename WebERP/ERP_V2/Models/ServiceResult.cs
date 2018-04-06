using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_V2.Models
{
    public class ServiceResult<T>
    {
        public T Data { get; set; }

        public string Message { get; set; }
    }
}