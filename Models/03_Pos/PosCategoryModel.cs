using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Gemini.Resources;

namespace Gemini.Models._03_Pos
{
    public class PosCategoryModel
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

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String SeoFriendUrl { get; set; }

        public Guid? ParentGuid { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String SeoTitle { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String SeoDescription { get; set; }

        public int? OrderBy { get; set; }

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

        public int RootId { get; set; }

        public List<PosCategoryModel> Items { get; set; }

        public List<PosProduceModel> Produces { get; set; }

        #region Constructor
        public PosCategoryModel()
        {
        }

        public PosCategoryModel(PosCategory posCategory)
        {
            Guid = posCategory.Guid;
            Name = posCategory.Name;
            Note = posCategory.Note;
            CreatedAt = posCategory.CreatedAt;
            CreatedBy = posCategory.CreatedBy;
            UpdatedAt = posCategory.UpdatedAt;
            UpdatedBy = posCategory.UpdatedBy;
            Active = posCategory.Active;
            ParentGuid = posCategory.ParentGuid;
            OrderBy = posCategory.OrderBy;
            SeoFriendUrl = posCategory.SeoFriendUrl;
            SeoTitle = posCategory.SeoTitle;
            SeoDescription = posCategory.SeoDescription;
        }
        #endregion

        #region Function
        public void Setvalue(PosCategory posCategory)
        {
            if (IsUpdate == 0)
            {
                posCategory.Guid = Guid.NewGuid();
                posCategory.CreatedBy = CreatedBy;
                posCategory.CreatedAt = DateTime.Now;
            }
            posCategory.Name = Name;
            posCategory.OrderBy = OrderBy;
            posCategory.Active = Active;
            posCategory.Note = Note;
            posCategory.ParentGuid = ParentGuid;
            posCategory.UpdatedAt = DateTime.Now;
            posCategory.UpdatedBy = UpdatedBy;
            posCategory.SeoFriendUrl = SeoFriendUrl;
            posCategory.SeoTitle = SeoTitle;
            posCategory.SeoDescription = SeoDescription;
        }
        #endregion
    }
}