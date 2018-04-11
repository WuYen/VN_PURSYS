using ERP_V2.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ERP_V2.ViewModels.TR01V2
{
    [MetadataType(typeof(TR01A_MD))]
    public class TR01AViewModel : TR01A
    {
        [Required]
        public DateTime? DtPUR_DT
        {
            get
            {
                DateTime dtTmp; //TryParse

                DateTime? value = null;
                if (!string.IsNullOrWhiteSpace(this.PUR_DT) && this.PUR_DT.Length > 6)
                {
                    DateTime.TryParse(this.PUR_DT.Insert(6, "/").Insert(4, "/"), out dtTmp);
                    value = dtTmp;
                }
                return value;
            }
            set
            {
                this.PUR_DT = (value != null ? value.Value.ToString("yyyyMMdd") : "");
            }
        }

        [Required]
        public DateTime? DtEXP_DT
        {
            get
            {
                DateTime dtTmp; //TryParse

                DateTime? value = null;
                if (!string.IsNullOrWhiteSpace(this.EXP_DT) && this.EXP_DT.Length > 6)
                {
                    DateTime.TryParse(this.EXP_DT.Insert(6, "/").Insert(4, "/"), out dtTmp);
                    value = dtTmp;
                }
                return value;
            }
            set
            {
                this.EXP_DT = (value != null ? value.Value.ToString("yyyyMMdd") : "");
            }
        }

        [Required]
        public int? CbBA01A_ID
        {
            get
            {
                if (this.BA01A_ID > 0)
                {
                    return this.BA01A_ID;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.BA01A_ID = (value != null ? value.Value : -1);
            }
        }

        [Required]
        public int? CbBA01B_ID
        {
            get
            {
                if (this.BA01B_ID > 0)
                {
                    return this.BA01B_ID;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.BA01B_ID = (value != null ? value.Value : -1);
            }
        }

        [Required]
        public int? CbBA03A_ID
        {
            get
            {
                if (this.BA03A_ID > 0)
                {
                    return this.BA03A_ID;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.BA03A_ID = (value != null ? value.Value : -1);
            }
        }

        public string ADD_DR { get; set; }
        public string FAX_NO { get; set; }
        public string TEL_NO { get; set; }
        public string TEL_EX { get; set; }

        public DateTime? DtRCV_DT
        {
            get
            {
                DateTime dtTmp; //TryParse

                DateTime? value = null;
                if (!string.IsNullOrWhiteSpace(this.EXP_DT) && this.EXP_DT.Length > 6)
                {
                    DateTime.TryParse(this.EXP_DT.Insert(6, "/").Insert(4, "/"), out dtTmp);
                    value = dtTmp;
                }
                return value;
            }
            set
            {
                this.RCV_DT = (value != null ? value.Value.ToString("yyyyMMdd") : "");
            }
        }
    }

    public class TR01A_MD
    {
        [Required]
        public string CPN_NM { get; set; }

        //[Required]
        public string CUR_RT { get; set; }
    }
}