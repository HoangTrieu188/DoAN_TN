using System;
using System.ComponentModel.DataAnnotations;

namespace Gemini.Models._20_Web
{
    public class UserChangePasswordModel
    {
        [StringLength(255, ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "StringLengthFill", MinimumLength = 6)]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        [Display(Name = "Nsd_Password", ResourceType = typeof(Resources.Resource))]
        [DataType(DataType.Password)]
        public String OldPassword { get; set; }

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
    }
}