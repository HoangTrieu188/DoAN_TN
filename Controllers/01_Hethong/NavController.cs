using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Gemini.Controllers.Bussiness;
using Gemini.Models._01_Hethong;

namespace Gemini.Controllers._01_Hethong
{
    public class NavController : GeminiController
    {
        /// <summary>
        /// Polycy Memu
        /// </summary>
        /// <returns></returns>
        public ActionResult Amenu()
        {
            try
            {
                var route = RouteTable.Routes.GetRouteData(HttpContext);
                var currentPortal = "admin";
                if (route != null)
                {
                    currentPortal = route.GetRequiredString("Portal");
                    ViewData["Portal"] = route.GetRequiredString("Portal");
                    var user = GetSettingUser();
                    if (user != null)
                    {
                        ViewData["Username"] = user.Username;
                        ViewData["GuidUser"] = user.Guid.ToString().ToLower().Trim();
                    }
                }

                var models = new List<SMenuModel>();
                var id = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket authTicket = id.Ticket;
                var role = DataGemini.SUsers.Where(s => s.Username.ToUpper().Equals(authTicket.Name.ToUpper())).Select(x => new { x.GuidRole }).FirstOrDefault();
                if (role != null && role.GuidRole != null)
                {
                    models = GetAllMenuInRole(role.GuidRole);
                }
                return View(models);
            }
            catch
            {
                return Redirect("/Error/ErrorList");
            }

        }

        /// <summary>
        /// Policy Toolbar
        /// </summary>
        /// <returns></returns>
        public ActionResult AToolbar()
        {
            try
            {
                var models = GetAllMainControlInMenu();

                return View(models);
            }
            catch
            {
                return Redirect("/Error/ErrorList");
            }
        }

        /// <summary>
        /// Policy Item 
        /// </summary>
        /// <returns></returns>
        public ActionResult AToolbarItem()
        {
            try
            {
                var route = RouteTable.Routes.GetRouteData(HttpContext);
                var controller = route.GetRequiredString("Controller");
                var action = route.GetRequiredString("Action");

                var models = new List<SControlModel>();

                if (controller.Equals("SUser", System.StringComparison.OrdinalIgnoreCase) && action.Equals("EditPopup", System.StringComparison.OrdinalIgnoreCase))
                {
                    var lstControlEditPopupUser = DataGemini.SControls.Where(x => x.Type == "EDIT_CONTROL" && (x.EventClick == "btnSaveclose"
                                                                                                               || x.EventClick == "btnRefresh"
                                                                                                               || x.EventClick == "btnClose")).OrderBy(x => x.Orderby).ToList();

                    lstControlEditPopupUser.ForEach(x => models.Add(new SControlModel(x)));
                }
                else
                    models = GetAllEditControlInMenu();

                return View(models);
            }
            catch
            {
                return Redirect("/Error/ErrorList");
            }
        }

        /// <summary>
        /// Policy Tool Bar Under
        /// </summary>
        /// <returns></returns>
        public ActionResult AToolbarc()
        {
            try
            {
                var models = GetAllTabControlInMenu();

                return View(models);
            }
            catch
            {
                return Redirect("/Error/ErrorList");
            }
        }
    }
}