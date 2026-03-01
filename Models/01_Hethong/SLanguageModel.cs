using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Gemini.Resources;
using SINNOVA.Core;

namespace Gemini.Models._01_Hethong
{
    public class SLanguageModel
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
        public SLanguageModel()
        {
        }

        public SLanguageModel(SLanguage sLanguage)
        {
            Guid = sLanguage.Guid;
            Name = sLanguage.Name;
            Active = sLanguage.Active;
            Note = sLanguage.Note;
            CreatedAt = sLanguage.CreatedAt;
            CreatedBy = vString.GetValueTostring(sLanguage.CreatedBy);
            UpdatedAt = sLanguage.UpdatedAt;
            UpdatedBy = vString.GetValueTostring(sLanguage.UpdatedBy);
        }
        #endregion

        #region Function
        public void Setvalue(SLanguage sLanguage)
        {
            if (IsUpdate == 0)
            {
                sLanguage.Guid = Guid.NewGuid();
                sLanguage.CreatedBy = CreatedBy;
                sLanguage.CreatedAt = DateTime.Now;
            }
            sLanguage.Name = Name;
            sLanguage.Active = Active;
            sLanguage.Note = Note;
            sLanguage.UpdatedAt = DateTime.Now;
            sLanguage.UpdatedBy = UpdatedBy;
        }
        #endregion
    }
}