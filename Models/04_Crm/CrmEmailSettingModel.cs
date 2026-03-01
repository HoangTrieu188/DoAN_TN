using System;
using System.ComponentModel.DataAnnotations;
using Gemini.Resources;

namespace Gemini.Models._04_Crm
{
    public class CrmEmailSettingModel
    {
        public int IsUpdate { get; set; }

        #region Properties
        [ScaffoldColumn(false)]
        public Guid Guid { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Email { get; set; }

        public bool Active { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String Note { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String PassEmail { get; set; }

        public bool EnableSsl { get; set; }

        public bool IsHtml { get; set; }

        public int Port { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Smtp { get; set; }

        public int DelayTime { get; set; }

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
        public CrmEmailSettingModel()
        {
        }

        public CrmEmailSettingModel(CrmEmailSetting crmEmailSetting)
        {
            Guid = crmEmailSetting.Guid;
            Email = crmEmailSetting.Email;
            EnableSsl = crmEmailSetting.EnableSsl;
            IsHtml = crmEmailSetting.IsHtml;
            Port = crmEmailSetting.Port;
            Smtp = crmEmailSetting.Smtp;
            Active = crmEmailSetting.Active;
            PassEmail = crmEmailSetting.PassEmail;
            DelayTime = crmEmailSetting.DelayTime / 1000;
            Note = crmEmailSetting.Note;
            CreatedAt = crmEmailSetting.CreatedAt;
            CreatedBy = crmEmailSetting.CreatedBy;
            UpdatedAt = crmEmailSetting.UpdatedAt;
            UpdatedBy = crmEmailSetting.UpdatedBy;
        }
        #endregion

        #region Function
        public void Setvalue(CrmEmailSetting crmEmailSetting)
        {


            if (IsUpdate == 0)
            {
                crmEmailSetting.Guid = Guid.NewGuid();
                crmEmailSetting.CreatedBy = CreatedBy;
                crmEmailSetting.CreatedAt = DateTime.Now;
            }
            crmEmailSetting.Smtp = Smtp;
            crmEmailSetting.EnableSsl = EnableSsl;
            crmEmailSetting.DelayTime = DelayTime * 1000;
            crmEmailSetting.IsHtml = IsHtml;
            crmEmailSetting.Port = Port;
            crmEmailSetting.PassEmail = PassEmail;
            crmEmailSetting.Email = Email;
            crmEmailSetting.Active = Active;
            crmEmailSetting.Note = Note;
            crmEmailSetting.UpdatedAt = DateTime.Now;
            crmEmailSetting.UpdatedBy = UpdatedBy;
        }
        #endregion
    }
}