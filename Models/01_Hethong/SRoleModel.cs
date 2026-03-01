using System;
using System.ComponentModel.DataAnnotations;
using Gemini.Resources;
using SINNOVA.Core;

namespace Gemini.Models._01_Hethong
{
    public class SRoleModel
    {
        public int IsUpdate { get; set; }

        #region Properties
        [ScaffoldColumn(false)]
        public Guid Guid { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String Name { get; set; }

        public bool Active { get; set; }

        public bool IsAdmin { get; set; }

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
        public SRoleModel()
        {
        }

        public SRoleModel(SRole sRole)
        {
            Guid = sRole.Guid;
            Name = sRole.Name;
            Active = sRole.Active;
            IsAdmin = sRole.IsAdmin.GetValueOrDefault(false);
            Note = sRole.Note;
            CreatedAt = sRole.CreatedAt;
            CreatedBy = vString.GetValueTostring(sRole.CreatedBy);
            UpdatedAt = sRole.UpdatedAt;
            UpdatedBy = vString.GetValueTostring(sRole.UpdatedBy);
        }
        #endregion

        #region Function
        public void Setvalue(SRole sRole)
        {
            if (IsUpdate == 0)
            {
                sRole.Guid = Guid.NewGuid();
                sRole.CreatedBy = CreatedBy;
                sRole.CreatedAt = DateTime.Now;
            }
            sRole.Name = Name;
            sRole.Active = Active;
            sRole.IsAdmin = IsAdmin;
            sRole.Note = Note;
            sRole.UpdatedAt = DateTime.Now;
            sRole.UpdatedBy = UpdatedBy;
        }
        #endregion
    }
}