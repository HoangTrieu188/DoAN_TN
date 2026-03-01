using Gemini.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gemini.Models._02_Cms.U
{
    public class UGroupModel
    {
        public int IsUpdate { get; set; }

        #region Properties
        public Guid Guid { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String Name { get; set; }

        public bool Active { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String Note { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String Type { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String SeoTitle { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String SeoDescription { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String SeoImage { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String SeoFriendUrl { get; set; }

        public Guid? ParentGuid { get; set; }

        public Guid? LanguageGuid { get; set; }

        public int? CountView { get; set; }

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

        public List<UGroupModel> Items { get; set; }

        public int? RootId { get; set; }

        #region Constructor
        public UGroupModel()
        {

        }

        public UGroupModel(UGroup uGroup)
        {
            Guid = uGroup.Guid;
            Name = uGroup.Name;
            Active = uGroup.Active;
            Note = uGroup.Note;
            CreatedAt = uGroup.CreatedAt;
            CreatedBy = uGroup.CreatedBy;
            UpdatedAt = uGroup.UpdatedAt;
            UpdatedBy = uGroup.UpdatedBy;
            Type = uGroup.Type;
            SeoDescription = uGroup.SeoDescription;
            SeoFriendUrl = uGroup.SeoFriendUrl;
            SeoImage = uGroup.SeoImage;
            SeoTitle = uGroup.SeoTitle;
            CountView = uGroup.CountView;
            ParentGuid = uGroup.ParentGuid;
            LanguageGuid = uGroup.LanguageGuid;
        }
        #endregion

        #region Function
        public void Setvalue(UGroup uGroup)
        {
            if (IsUpdate == 0)
            {
                uGroup.CreatedBy = CreatedBy;
                uGroup.Guid = Guid.NewGuid();
                uGroup.CreatedAt = DateTime.Now;
            }
            uGroup.Name = Name;
            uGroup.Active = Active;
            uGroup.Note = Note;
            uGroup.UpdatedAt = DateTime.Now;
            uGroup.UpdatedBy = UpdatedBy;
            uGroup.LanguageGuid = LanguageGuid;
            uGroup.Type = Type;
            uGroup.SeoTitle = SeoTitle;
            uGroup.SeoImage = SeoImage;
            uGroup.SeoFriendUrl = SeoFriendUrl;
            uGroup.SeoDescription = SeoDescription;
            uGroup.CountView = CountView;
            uGroup.ParentGuid = ParentGuid;
        }
        #endregion
    }
}