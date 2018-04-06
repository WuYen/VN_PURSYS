using ERP_V2.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ERP_V2.ViewModels.TR01V2
{
    [MetadataType(typeof(TR01B_MD))]
    public class TR01BViewModel : TR01B
    {
        //[Required]
        public string ITM_NO { get; set; }

        public string ITM_NM { get; set; }

        public string INC_NO { get; set; }
        public string INC_NM { get; set; }
        public string ITM_SP { get; set; }
    }

    public class TR01B_MD
    {
        //[Required]
        //[Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "必須要>0")]
        //public Nullable<decimal> TRN_QT { get; set; }

        //[Required]
        //[Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "必須要>0")]
        //public Nullable<decimal> UNT_PR { get; set; }
        [Required]
        public decimal PUR_PR { get; set; }
        [Required]
        public int PUR_QT { get; set; }

    }
}