using ERP_V2.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ERP_V2.ViewModels.BA02
{
    [MetadataType(typeof(BA02A_MD))]
    public class BA02AViewModel : BA02A
    {
        //[Required]
        //public DateTime? DtTRN_DT
        //{
        //    get
        //    {
        //        DateTime dtTmp; //TryParse

        //        DateTime? value = null;
        //        if (!string.IsNullOrWhiteSpace(this.TRN_DT) && this.TRN_DT.Length > 6)
        //        {
        //            DateTime.TryParse(this.TRN_DT.Insert(6, "/").Insert(4, "/"), out dtTmp);
        //            value = dtTmp;
        //        }
        //        return value;
        //    }
        //    set
        //    {
        //        this.TRN_DT = (value != null ? value.Value.ToString("yyyyMMdd") : "");
        //    }
        //}

        //public DateTime? DtEXP_DT
        //{
        //    get
        //    {
        //        DateTime dtTmp; //TryParse

        //        DateTime? value = null;
        //        if (!string.IsNullOrWhiteSpace(this.EXP_DT) && this.EXP_DT.Length > 6)
        //        {
        //            DateTime.TryParse(this.EXP_DT.Insert(6, "/").Insert(4, "/"), out dtTmp);
        //            value = dtTmp;
        //        }
        //        return value;
        //    }
        //    set
        //    {
        //        this.EXP_DT = (value != null ? value.Value.ToString("yyyyMMdd") : "");
        //    }
        //}

        //public string SUP_NM
        //{
        //    get
        //    {
        //        var SUP = CacheCommonDataModule.GetFA07A().Where(x => x.SUP_ID == this.SUP_ID).FirstOrDefault();
        //        return SUP == null ? "" : SUP.SUP_NM;
        //    }
        //}
        //public string STK_NM
        //{
        //    get
        //    {
        //        var STK = CacheCommonDataModule.GetFA16A().Where(x => x.STK_ID == this.STK_ID).FirstOrDefault();
        //        return STK == null ? "" : STK.STK_NM;
        //    }
        //}

        public BA02AViewModel() { }

        public BA02AViewModel(BA02A entity)
        {
            this.FromDomain(entity);
        }

        /// <summary>
        /// entity to viewmodel
        /// </summary>
        /// <param name="entity"></param>
        public void FromDomain(BA02A entity)
        {
            if (entity != null)
            {
                var objectHelper = new ActWeis.Utility.ObjectHelper();
                objectHelper.CopyValue(entity, this);

                #region [ 資料處理 ]
                #endregion [ 資料處理 ]
            }
        }

        /// <summary>
        /// viewmodel to entity
        /// </summary>
        /// <param name="entity"></param>
        public void ToDomain(BA02A entity)
        {
            if (entity != null)
            {
                #region [ 資料處理 ]

                #endregion [ 資料處理 ]

                var objectHelper = new ActWeis.Utility.ObjectHelper();
                objectHelper.CopyValue(this, entity);
            }
        }
    }

    public class BA02A_MD
    {
        //[Required]
        //[StringLength(120)]
        //public string ITM_NO { get; set; }
        [Required]
        [StringLength(120)]
        public string ITM_NM { get; set; }

        [Required]
        public int? TYP_ID { get; set; }
        //[Required]
        //public string STK_ID { get; set; }
    }
}