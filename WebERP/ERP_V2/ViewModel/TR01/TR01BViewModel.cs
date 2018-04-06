using ERP_V2.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ERP_V2.ViewModels.TR01
{
    [MetadataType(typeof(TR01B_MD))]
    public class TR01BViewModel : TR01B
    {
        public string ITM_NO { get; set; }

        public string ITM_NM { get; set; }

        public string INC_NO { get; set; }
        public string INC_NM { get; set; }


        public TR01BViewModel() { }

        public TR01BViewModel(TR01B entity)
        {
            this.FromDomain(entity);
        }

        /// <summary>
        /// entity to viewmodel
        /// </summary>
        /// <param name="entity"></param>
        public void FromDomain(TR01B entity)
        {
            if (entity != null)
            {
                var objectHelper = new ActWeis.Utility.ObjectHelper();
                objectHelper.CopyValue(entity, this);

                #region [ 資料處理 ]
                var item = CacheCommonDataModule.GetBA02A().Where(x => x.BA02A_ID == entity.BA02A_ID).FirstOrDefault();
                if (item != null)
                {
                    this.ITM_NO = item.ITM_NO;
                    this.ITM_NM = item.ITM_NM;
                }
                //var incorporation = CacheCommonDataModule.GetBA01A().Where(x => x.BA01A_ID == entity.BA01A_ID).FirstOrDefault();
                //if (incorporation != null)
                //{
                //    this.ITM_NO = item.ITM_NO;
                //    this.ITM_NM = item.ITM_NM;
                //}

                #endregion [ 資料處理 ]
            }
        }

        /// <summary>
        /// viewmodel to entity
        /// </summary>
        /// <param name="entity"></param>
        public void ToDomain(TR01B entity)
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