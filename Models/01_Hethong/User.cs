using System.ComponentModel.DataAnnotations;
using System.Web.Security;

namespace Gemini.Models._01_Hethong
{
    public class User
    {
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public string SecurityCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public string Password { get; set; }

        public string KendoTheme { get; set; }

        public string KendoLanguage { get; set; }

        public bool RememberMe { get; set; }

        public static FormsIdentity Identity { get; set; }
    }
}