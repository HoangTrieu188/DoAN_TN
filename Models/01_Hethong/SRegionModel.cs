using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Gemini.Resources;
using SINNOVA.Core;

namespace Gemini.Models._01_Hethong
{
    public class SRegionModel
    {
        public int IsUpdate { get; set; }

        #region Properties
        [ScaffoldColumn(false)]
        public Guid Guid { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String Name { get; set; }

        public bool Active { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String Note { get; set; }

        public Guid? ParentGuid { get; set; }

        public int RootId { get; set; }

        [Editable(false)]
        public DateTime? CreatedAt { get; set; }

        [Editable(false)]
        [StringLength(25, ErrorMessageResourceName = "ErrorMaxLength25", ErrorMessageResourceType = typeof(Resource))]
        public String CreatedBy { get; set; }

        [Editable(false)]
        public DateTime? UpdatedAt { get; set; }

        [Editable(false)]
        [StringLength(25, ErrorMessageResourceName = "ErrorMaxLength25", ErrorMessageResourceType = typeof(Resource))]
        public String UpdatedBy { get; set; }
        #endregion

        public List<SRegionModel> Items { get; set; }

        #region Constructor
        public SRegionModel()
        {

        }

        public SRegionModel(SRegion sRegion)
        {
            Guid = sRegion.Guid;
            Name = sRegion.Name;
            Active = sRegion.Active;
            Note = sRegion.Note;
            ParentGuid = sRegion.ParentGuid;
            CreatedAt = sRegion.CreatedAt;
            CreatedBy = vString.GetValueTostring(sRegion.CreatedBy);
            UpdatedAt = sRegion.UpdatedAt;
            UpdatedBy = vString.GetValueTostring(sRegion.UpdatedBy);
        }
        #endregion

        #region Function
        public void Setvalue(SRegion sRegion)
        {
            if (IsUpdate == 0)
            {
                sRegion.CreatedBy = CreatedBy;
                sRegion.Guid = Guid.NewGuid();
                sRegion.CreatedAt = DateTime.Now;
            }
            sRegion.Name = vString.GetValueTostring(Name);
            sRegion.Active = Active;
            sRegion.Note = Note;
            sRegion.ParentGuid = ParentGuid;
            sRegion.UpdatedAt = DateTime.Now;
            sRegion.UpdatedBy = UpdatedBy;
        }
        #endregion
    }
}