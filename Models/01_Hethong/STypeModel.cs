using System;
using System.ComponentModel.DataAnnotations;
using Gemini.Resources;

namespace Gemini.Models._01_Hethong
{
    public class STypeModel
    {
        public int IsUpdate { get; set; }

        #region Properties
        [ScaffoldColumn(false)]
        public Guid Guid { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String KeyType { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String ValueType { get; set; }

        public bool Active { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String Note { get; set; }

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

        #region Constructor
        public STypeModel()
        {
        }

        public STypeModel(SType sType)
        {
            Guid = sType.Guid;
            KeyType = sType.KeyType;
            ValueType = sType.ValueType;
            Active = sType.Active;
            Note = sType.Note;
            CreatedAt = sType.CreatedAt;
            CreatedBy = sType.CreatedBy;
            UpdatedAt = sType.UpdatedAt;
            UpdatedBy = sType.UpdatedBy;
        }
        #endregion

        #region Function
        public void Setvalue(SType sType)
        {
            if (IsUpdate == 0)
            {
                sType.Guid = Guid.NewGuid();
                sType.CreatedBy = CreatedBy;
                sType.CreatedAt = DateTime.Now;
            }
            sType.KeyType = KeyType;
            sType.ValueType = ValueType;
            sType.Active = Active;
            sType.Note = Note;
            sType.UpdatedAt = DateTime.Now;
            sType.UpdatedBy = UpdatedBy;
        }
        #endregion
    }
}