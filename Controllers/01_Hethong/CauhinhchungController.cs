using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Gemini.Controllers.Bussiness;

namespace Gemini.Controllers._01_Hethong
{
    public class CauhinhchungController : GeminiController
    {
        public ActionResult Index()
        {

            return RedirectToAction("List");
        }

        /// <summary>
        /// List view grid
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            return View();
        }

        public ActionResult Save(HttpPostedFileBase filePath1, string filename)
        {
            try
            {
                var msg = "";
                if (filePath1 != null)
                {
                    var physicalPath = Path.Combine(Server.MapPath("~/Content/UserFiles/Cauhinh"), filename);
                    filePath1.SaveAs(physicalPath);
                    msg = filename;
                }
                return Json(new { status = "" + msg + "" }, "text/plain");
            }
            catch (Exception ex)
            {
                HandleError(ex);
                return Json(new { status = "" + ex + "" }, "text/plain");
            }



        }
    }
}