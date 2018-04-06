using ERP_V2.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ERP_V2.ViewModels.TR01V3
{
    [MetadataType(typeof(TR01B_MD))]
    public class TR01BViewModel : TR01B
    {
        [Required]
        public string ITM_NO { get; set; }

        public string ITM_NM { get; set; }

        public string INC_NO { get; set; }
        public string INC_NM { get; set; }
    }

    public class TR01B_MD
    {
        [Required]
        public decimal PUR_PR { get; set; }
        [Required]
        public int PUR_QT { get; set; }

    }
}