using System;
using System.ComponentModel.DataAnnotations;

namespace Gemini.Models._20_Web
{
    public class UserRegisterModel
    {
        [StringLength(255, ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "StringLengthFill", MinimumLength = 6)]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        [Display(Name = "Nsd_TenNsd", ResourceType = typeof(Resources.Resource))]
        public String Username { get; set; }

        [StringLength(255, ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "StringLengthFill", MinimumLength = 6)]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        [Display(Name = "Nsd_Username", ResourceType = typeof(Resources.Resource))]
        public String Account { get; set; }

        [StringLength(255, ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "StringLengthFill", MinimumLength = 6)]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        [Display(Name = "Nsd_Password", ResourceType = typeof(Resources.Resource))]
        [DataType(DataType.Password)]
        public String Password { get; set; }

        [StringLength(255, ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "StringLengthFill", MinimumLength = 6)]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        [DataType(DataType.Password)]
        [Display(Name = "Nsd_Password1", ResourceType = typeof(Resources.Resource))]
        public String PasswordAgain { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$", ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RegularEmailFill")]
        [Display(Name = "Nsd_Email", ResourceType = typeof(Resources.Resource))]
        public String Email { get; set; }

        public String CodeExtend { get; set; }
    }
}