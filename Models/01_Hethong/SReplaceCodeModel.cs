using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Gemini.Resources;

namespace Gemini.Models._01_Hethong
{
    public class SReplaceCodeModel
    {
        public int IsUpdate { get; set; }

        #region Properties
        [ScaffoldColumn(false)]
        public Guid Guid { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String ReplaceCode { get; set; }

        [AllowHtml]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFill")]
        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String ReplaceText { get; set; }

        public bool Active { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String Note { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Type { get; set; }

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
        public SReplaceCodeModel()
        {
        }

        public SReplaceCodeModel(SReplaceCode sReplaceCode)
        {
            Guid = sReplaceCode.Guid;
            ReplaceCode = sReplaceCode.ReplaceCode;
            Active = sReplaceCode.Active;
            Note = sReplaceCode.Note;
            CreatedAt = sReplaceCode.CreatedAt;
            CreatedBy = sReplaceCode.CreatedBy;
            UpdatedAt = sReplaceCode.UpdatedAt;
            UpdatedBy = sReplaceCode.UpdatedBy;
            ReplaceText = HttpUtility.HtmlDecode(sReplaceCode.ReplaceText);
            Type = sReplaceCode.Type;
        }
        #endregion

        #region Function
        public void Setvalue(SReplaceCode sReplaceCode)
        {
            if (IsUpdate == 0)
            {
                sReplaceCode.Guid = Guid.NewGuid();
                sReplaceCode.CreatedBy = CreatedBy;
                sReplaceCode.CreatedAt = DateTime.Now;
            }
            sReplaceCode.ReplaceCode = ReplaceCode;
            sReplaceCode.Active = Active;
            sReplaceCode.Note = Note;
            sReplaceCode.UpdatedAt = DateTime.Now;
            sReplaceCode.UpdatedBy = UpdatedBy;
            sReplaceCode.ReplaceText = ReplaceText;
            sReplaceCode.Type = Type;
        }
        #endregion
    }
}