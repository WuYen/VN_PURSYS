using ERP_V2.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_V2.ViewModels.TR01M
{
    [MetadataType(typeof(TR01C_MD))]
    public class TR01CModel : TR01C
    {
        public ModelStateDictionary ModelState { get; set; }

        [Required]
        public DateTime? DtARR_DT
        {
            get
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                DateTime dtTmp; //TryParse

                DateTime? value = null;
                if (!string.IsNullOrWhiteSpace(this.ARR_DT) && this.ARR_DT.Length > 6)
                {
                    DateTime.TryParse(this.ARR_DT.Insert(6, "/").Insert(4, "/"), out dtTmp);
                    value = dtTmp;
                }
                return value;
            }
            set
            {
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
                this.ARR_DT = (value != null ? value.Value.ToString("yyyyMMdd") : "");
            }
        }
    }

    public class TR01C_MD
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Resource),
              ErrorMessageResourceName = "RequireErrMsg")]
        public string CUR_RT { get; set; }
    }
}