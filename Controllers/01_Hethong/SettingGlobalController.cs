using System.Web.Mvc;
using System.Web.Security;
using Gemini.Controllers.Bussiness;
using SINNOVA.Core;

namespace Gemini.Controllers._01_Hethong
{
    public class SettingGlobalController : Controller
    {
        #region Define
        private const string DefaultTheme = "kendo.Default.min.css";
        private const string DefaultLanguage = "kendo.Vi-vn.js";
        private const string DefaultCulture = "kendo.culture.vi-VN.min.js";
        #endregion

        #region GetKendoTheme
        public string GetKendoTheme()
        {
            var id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket authTicket = id.Ticket;
            var currentUserData = authTicket.UserData;
            string[] kendoSetting = currentUserData.Split(';');
            if (!string.IsNullOrEmpty(kendoSetting[0]))
            {
                return kendoSetting[0];
            }
            return DefaultTheme;
        }
        #endregion

        #region GetKendoLanguage
        public string GetKendoLanguage()
        {
            var id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket authTicket = id.Ticket;
            var currentUserData = authTicket.UserData;
            string[] kendoSetting = currentUserData.Split(';');
            if (!string.IsNullOrEmpty(kendoSetting[1]))
            {
                return kendoSetting[1];
            }
            return DefaultLanguage;
        }
        #endregion

        #region GetKendoCulture
        public string GetKendoCulture()
        {
            var id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket authTicket = id.Ticket;
            var currentUserData = authTicket.UserData;
            string[] kendoSetting = currentUserData.Split(';');
            if (!string.IsNullOrEmpty(kendoSetting[1]))
            {
                switch (vString.GetTostring(kendoSetting[1]))
                {
                    case "kendo.en-GB.js":
                    {
                        new SitesLanguage().SetLanguage("en-GB");
                        return "kendo.culture.en-GB.min.js";
                    }
                    case "kendo.en-US.js":
                    {
                        new SitesLanguage().SetLanguage("en-US");
                        return "kendo.culture.en-US.min.js";
                    }
                    case "kendo.Vi-vn.js":
                    {
                        new SitesLanguage().SetLanguage("vi-VN");
                       return "kendo.culture.vi-VN.min.js";
                    }
                }
            }
            return DefaultCulture;
        }
        public string GetKendoCultureName()
        {
            var id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket authTicket = id.Ticket;
            var currentUserData = authTicket.UserData;
            string[] kendoSetting = currentUserData.Split(';');
            if (!string.IsNullOrEmpty(kendoSetting[1]))
            {
                switch (vString.GetTostring(kendoSetting[1]))
                {
                    case "kendo.en-GB.js":
                    {
                        return "en-GB";
                    }
                    case "kendo.en-US.js":
                    {
                        return "en-US";
                    }
                    case "kendo.Vi-vn.js":
                    {
                     return "vi-VN";
                    }
                }
            }
            return "vi-VN";
        }
        
        #endregion
    }
}
