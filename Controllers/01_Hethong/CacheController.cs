using System.Web;
using System.Web.Mvc;

namespace Gemini.Controllers._01_Hethong
{
    public class CacheController : Controller
    {
        //
        // GET: /Cache/

        public ActionResult Index()
        {
            HttpRuntime.Cache.Remove("Menu");

            return View();
        }
    }
}