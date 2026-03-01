using System;
using System.ComponentModel.DataAnnotations;
using Gemini.Resources;

namespace Gemini.Models._01_Hethong
{
    public class SUserModel
    {
        public int IsUpdate { get; set; }

        #region Properties
        [ScaffoldColumn(false)]
        public Guid Guid { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String Username { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String FullName { get; set; }

        public bool Active { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String Note { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String Email { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Avartar { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [RegularExpression(@"^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$", ErrorMessage = "**")]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String Mobile { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String FullAddress { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Password { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Position { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Skype { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? StartDate { get; set; }

        public Guid GuidRole { get; set; }

        public Boolean Notification { get; set; }

        public int? RecordsInPage { get; set; }

        public int? Legit { get; set; }

        public int? LegitCount { get; set; }

        public string GuidFollow { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Zalo { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Facebook { get; set; }

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

        public bool IsAdmin { get; set; }

        public bool IsChangePassword { get; set; }

        public String Password1 { get; set; }

        public int CountFollow { get; set; }

        #region Constructor
        public SUserModel()
        {
        }

        public SUserModel(SUser sUser)
        {
            Guid = sUser.Guid;
            FullName = sUser.FullName;
            Username = sUser.Username;
            Active = sUser.Active;
            Note = sUser.Note;
            CreatedAt = sUser.CreatedAt;
            CreatedBy = sUser.CreatedBy;
            UpdatedAt = sUser.UpdatedAt;
            UpdatedBy = sUser.UpdatedBy;
            Avartar = sUser.Avartar;
            Email = sUser.Email;
            RecordsInPage = sUser.RecordsInPage;
            EndDate = sUser.EndDate;
            GuidRole = sUser.GuidRole;
            Mobile = sUser.Mobile;
            FullAddress = sUser.FullAddress;
            Notification = sUser.Notification;
            Password = sUser.Password;
            Position = sUser.Position;
            Skype = sUser.Skype;
            StartDate = sUser.StartDate;
            Zalo = sUser.Zalo;
            Facebook = sUser.Facebook;
            Legit = sUser.Legit;
            LegitCount = sUser.LegitCount;
            GuidFollow = sUser.GuidFollow;
        }
        #endregion

        #region Function
        public void Setvalue(SUser sUser)
        {
            if (IsUpdate == 0)
            {
                sUser.Guid = Guid.NewGuid();
                sUser.CreatedBy = CreatedBy;
                sUser.CreatedAt = DateTime.Now;
            }
            sUser.FullName = FullName;
            sUser.Username = Username;
            sUser.Active = Active;
            sUser.Note = Note;
            sUser.UpdatedAt = DateTime.Now;
            sUser.UpdatedBy = UpdatedBy;
            sUser.Avartar = Avartar;
            sUser.Email = Email;
            sUser.RecordsInPage = RecordsInPage;
            sUser.EndDate = EndDate;
            sUser.GuidRole = GuidRole;
            sUser.Mobile = Mobile;
            sUser.FullAddress = FullAddress;
            sUser.Notification = Notification;
            sUser.Password = Password;
            sUser.Position = Position;
            sUser.Skype = Skype;
            sUser.StartDate = StartDate;
            sUser.Zalo = Zalo;
            sUser.Facebook = Facebook;
            sUser.Legit = Legit;
            sUser.LegitCount = LegitCount;
            sUser.GuidFollow = GuidFollow;
        }
        #endregion
    }
}