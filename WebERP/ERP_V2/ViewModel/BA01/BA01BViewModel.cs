using ERP_V2.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ERP_V2.ViewModels.BA01
{
    [MetadataType(typeof(BA01B_MD))]
    public class BA01BViewModel : BA01B
    {
        //[Required]
        //[StringLength(30)]
        //public string ITM_NO { get; set; }

        //public string ITM_NM { get; set; }

        public BA01BViewModel() { }

        public BA01BViewModel(BA01B entity)
        {
            this.FromDomain(entity);
        }

        /// <summary>
        /// entity to viewmodel
        /// </summary>
        /// <param name="entity"></param>
        public void FromDomain(BA01B entity)
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
        public void ToDomain(BA01B entity)
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

    public class BA01B_MD
    {
        [Required]
        [StringLength(20)]
        public string CNT_NM { get; set; }
    }
}