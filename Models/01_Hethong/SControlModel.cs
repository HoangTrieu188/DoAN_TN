using System;
using Gemini.Resources;
using System.ComponentModel.DataAnnotations;

namespace Gemini.Models._01_Hethong
{
    public class SControlModel
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
        public String EventClick { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String SpriteCssClass { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Type { get; set; }

        public int OrderBy { get; set; }

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

        public bool IsMenu { get; set; }

        public bool IsRole { get; set; }

        #region Constructor
        public SControlModel()
        {
        }

        public SControlModel(SControl sControl)
        {
            Guid = sControl.Guid;
            Name = sControl.Name;
            Active = sControl.Active;
            Note = sControl.Note;
            CreatedAt = sControl.CreatedAt;
            CreatedBy = sControl.CreatedBy;
            UpdatedAt = sControl.UpdatedAt;
            UpdatedBy = sControl.UpdatedBy;
            EventClick = sControl.EventClick;
            SpriteCssClass = sControl.SpriteCssClass;
            Type = sControl.Type;
            OrderBy = sControl.Orderby;
        }
        #endregion

        #region Function
        public void Setvalue(SControl sControl)
        {
            if (IsUpdate == 0)
            {
                sControl.Guid = Guid.NewGuid();
                sControl.CreatedBy = CreatedBy;
                sControl.CreatedAt = DateTime.Now;
            }
            sControl.Name = Name;
            sControl.Active = Active;
            sControl.Note = Note;
            sControl.SpriteCssClass = SpriteCssClass;
            sControl.EventClick = EventClick;
            sControl.UpdatedAt = DateTime.Now;
            sControl.UpdatedBy = UpdatedBy;
            sControl.Type = Type;
            sControl.Orderby = OrderBy;
        }
        #endregion
    }
}