using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.Routing;
using System.Web;
using System.Security.Principal;
using System.Web.Security;

namespace Gemini.Controllers.Bussiness
{
    [AttributeUsageAttribute(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        //Called when access is denied
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            //User isn't logged in
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Admin", action = "Login" })
                );
            }
            //User is logged in but has no access
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Admin", action = "NotAuthorized" })
                );
            }
        }

        //Core authentication, called before each action
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var cookie = httpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null)
            {
                return false;
            }

            var ticket = FormsAuthentication.Decrypt(cookie.Value);
            var userData = !string.IsNullOrWhiteSpace(ticket.UserData) ? ticket.UserData.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>();

            string lstMenus = userData.Any() ? userData[userData.Count - 1] : string.Empty;

            var route = RouteTable.Routes.GetRouteData(httpContext);
            var linkUrl = route.GetRequiredString("Controller");

            var menuInRole = string.IsNullOrWhiteSpace(lstMenus) ? new List<string>() : lstMenus.ToLower().Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var sMenu = menuInRole.FirstOrDefault(x => menuInRole.Exists(c => c.Contains(("/" + linkUrl).ToLower())));

            return sMenu != null;
        }
    }
}