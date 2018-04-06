using ERP_V2.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ERP_V2.ViewModels.BA02
{
    [MetadataType(typeof(BA02B_MD))]
    public class BA02BViewModel : BA02B
    {
        //[Required]
        //[StringLength(30)]
        //public string ITM_NO { get; set; }

        //public string ITM_NM { get; set; }

        public BA02BViewModel() { }

        public BA02BViewModel(BA02B entity)
        {
            this.FromDomain(entity);
        }

        /// <summary>
        /// entity to viewmodel
        /// </summary>
        /// <param name="entity"></param>
        public void FromDomain(BA02B entity)
        {
            if (entity != null)
            {
                var objectHelper = new ActWeis.Utility.ObjectHelper();
                objectHelper.CopyValue(entity, this);

                #region [ 資料處理 ]
                //var item = CacheCommonDataModule.GetVW_FA11A().Where(x => x.FA11A_ID == entity.FA11A_ID).FirstOrDefault();
                //if (item != null)
                //{
                //    this.ITM_NO = item.ITM_NO;
                //    this.ITM_NM = item.ITM_NM;
                //    if (item.FA24A_ID_PUR.HasValue && !this.FA24A_ID.HasValue)
                //    {
                //        this.FA24A_ID = item.FA24A_ID_PUR.Value;
                //    }
                //}

                #endregion [ 資料處理 ]
            }
        }

        /// <summary>
        /// viewmodel to entity
        /// </summary>
        /// <param name="entity"></param>
        public void ToDomain(BA02B entity)
        {
            if (entity != null)
            {
                var objectHelper = new ActWeis.Utility.ObjectHelper();
                objectHelper.CopyValue(this, entity);

                #region [ 資料處理 ]                
                #endregion [ 資料處理 ]
            }
        }
    }

    public class BA02B_MD
    {
        //[Required]
        //[Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "必須要>0")]
        //public Nullable<decimal> TRN_QT { get; set; }

        //[Required]
        //[Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "必須要>0")]
        //public Nullable<decimal> UNT_PR { get; set; }
    }
}