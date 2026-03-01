using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;
using Gemini.Resources;
using SINNOVA.Core;

namespace Gemini.Models._04_Crm
{
    public class CrmEmailTemplateModel
    {
        public int IsUpdate { get; set; }

        #region Properties
        [ScaffoldColumn(false)]
        public Guid Guid { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String Code { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String SubjectEmail { get; set; }

        public bool Active { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String Note { get; set; }

        [AllowHtml]
        public String ContentTemplate { get; set; }

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
        public CrmEmailTemplateModel()
        {
        }

        public CrmEmailTemplateModel(CrmEmailTemplate crmEmailTemplate)
        {
            Guid = crmEmailTemplate.Guid;
            Code = crmEmailTemplate.Code;
            SubjectEmail = crmEmailTemplate.SubjectEmail;
            Active = crmEmailTemplate.Active;
            ContentTemplate = HttpUtility.HtmlDecode(crmEmailTemplate.ContentTemplate);
            Note = crmEmailTemplate.Note;
            CreatedAt = crmEmailTemplate.CreatedAt;
            CreatedBy = crmEmailTemplate.CreatedBy;
            UpdatedAt = crmEmailTemplate.UpdatedAt;
            UpdatedBy = crmEmailTemplate.UpdatedBy;
        }
        #endregion

        #region Function
        public void Setvalue(CrmEmailTemplate crmEmailTemplate)
        {
            if (IsUpdate == 0)
            {
                crmEmailTemplate.Guid = Guid.NewGuid();
                crmEmailTemplate.CreatedBy = CreatedBy;
                crmEmailTemplate.CreatedAt = DateTime.Now;
            }
            crmEmailTemplate.Code = vString.GetValueTostring(Code);
            crmEmailTemplate.SubjectEmail = vString.GetValueTostring(SubjectEmail);
            crmEmailTemplate.ContentTemplate = ContentTemplate;
            crmEmailTemplate.Active = Active;
            crmEmailTemplate.Note = Note;
            crmEmailTemplate.UpdatedAt = DateTime.Now;
            crmEmailTemplate.UpdatedBy = UpdatedBy;
        }
        #endregion
    }
}