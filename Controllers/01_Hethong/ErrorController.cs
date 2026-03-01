using System.Web.Mvc;

namespace Gemini.Controllers._01_Hethong
{
    public class ErrorController : Controller
    {
        public ActionResult ErrorList()
        {
            return PartialView("ErrorList");
        }
    }
}