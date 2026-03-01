using System;
using System.ComponentModel.DataAnnotations;

namespace Gemini.Models._01_Hethong
{
    public class UserForgotPassword
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}$", ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RegularEmailFill")]
        [Display(Name = "Nsd_Email", ResourceType = typeof(Resources.Resource))]
        public String Email { get; set; }
    }
}