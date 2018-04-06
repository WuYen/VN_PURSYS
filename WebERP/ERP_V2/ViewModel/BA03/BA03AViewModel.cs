using ERP_V2.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ERP_V2.ViewModels.BA03
{
    [MetadataType(typeof(BA03A_MD))]
    public class BA03AViewModel : BA03A
    {
        public BA03AViewModel() { }

        public BA03AViewModel(BA03A entity)
        {
            this.FromDomain(entity);
        }

        /// <summary>
        /// entity to viewmodel
        /// </summary>
        /// <param name="entity"></param>
        public void FromDomain(BA03A entity)
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
        public void ToDomain(BA03A entity)
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

    public class BA03A_MD
    {
        [Required]
        [StringLength(20)]
        public string CUR_ID { get; set; }

        [Required]
        [StringLength(20)]
        public string CUR_NM { get; set; }

        [Required]
        public decimal CUR_RT { get; set; }
    }
}