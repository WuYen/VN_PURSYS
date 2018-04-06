using ERP_V2.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ERP_V2.ViewModels.BA01
{
    [MetadataType(typeof(BA01A_MD))]
    public class BA01AViewModel : BA01A
    {
        public BA01AViewModel() { }

        public BA01AViewModel(BA01A entity)
        {
            this.FromDomain(entity);
        }

        /// <summary>
        /// entity to viewmodel
        /// </summary>
        /// <param name="entity"></param>
        public void FromDomain(BA01A entity)
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
        public void ToDomain(BA01A entity)
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

    public class BA01A_MD
    {
        [Required]
        [StringLength(20)]
        public string INC_NO { get; set; }
        [Required]
        [StringLength(100)]
        public string INC_NM { get; set; }
    }
}