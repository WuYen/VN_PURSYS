using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ERP_V2.ViewModels.TR01M
{
    public class TR01AModel : TR01V2.TR01AViewModel
    {
        public bool IsAllReceive { get; set; }

        public int DelayDate
        {
            get
            {
                var time = DateTime.UtcNow.AddHours(07);
                return time.Subtract(this.DtEXP_DT.Value).Days;
            }
        }
    }
}