using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Gemini.Controllers.Bussiness;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Gemini.Models;
using Constants = Gemini.Controllers.Bussiness.Constants;
using Gemini.Models._03_Pos;

namespace Gemini.Controllers._03_Pos
{
    [CustomAuthorizeAttribute]
    public class PosBatchController : GeminiController
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
            GetSettingUser();
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var user = GetSettingUser();

            IEnumerable<PosBatch> posBatches = DataGemini.PosBatches.Where(x => x.CreatedBy.Equals(user.Username, StringComparison.OrdinalIgnoreCase) || user.IsAdmin).OrderByDescending(p => p.CreatedAt).ToList();
            DataSourceResult result = ConvertIEnumerate(posBatches).ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult Create()
        {
            try
            {
                var posBatch = new PosBatch();
                var viewModel = new PosBatchModel(posBatch) { IsUpdate = 0, Active = true };
                return PartialView("Edit", viewModel);
            }
            catch
            {
                return Redirect("/Error/ErrorList");
            }
        }

        public ActionResult Edit(Guid guid)
        {
            try
            {
                var posBatch = new PosBatch();
                posBatch = DataGemini.PosBatches.FirstOrDefault(c => c.Guid == guid);
                var viewModel = new PosBatchModel(posBatch) { IsUpdate = 1 };
                return PartialView("Edit", viewModel);
            }
            catch
            {
                return Redirect("/Error/ErrorList");
            }
        }

        public ActionResult Delete(Guid guid)
        {
            try
            {
                var posBatch = new PosBatch();
                posBatch = DataGemini.PosBatches.FirstOrDefault(c => c.Guid == guid);
                DataGemini.PosBatches.Remove(posBatch);
                if (SaveData("PosBatch") && posBatch != null)
                {
                    DataReturn.ActiveCode = posBatch.Guid.ToString();
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                }
                else
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                    DataReturn.MessagError = Constants.CannotDelete + " Date : " + DateTime.Now;
                }

            }
            catch (Exception ex)
            {
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update(PosBatchModel viewModel)
        {
            var posBatch = new PosBatch();
            try
            {
                viewModel.UpdatedBy = viewModel.CreatedBy = GetUserInSession();
                if (viewModel.IsUpdate == 0)
                {
                    viewModel.Setvalue(posBatch);
                    DataGemini.PosBatches.Add(posBatch);
                }
                else
                {
                    posBatch = DataGemini.PosBatches.FirstOrDefault(c => c.Guid == viewModel.Guid);
                    viewModel.Setvalue(posBatch);
                }
                if (SaveData("PosBatch") && posBatch != null)
                {
                    DataReturn.ActiveCode = posBatch.Guid.ToString();
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                }
                else
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                    DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                if (viewModel.IsUpdate == 0)
                {
                    DataGemini.PosBatches.Remove(posBatch);
                }
                HandleError(ex);
            }
            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<PosBatchModel> ConvertIEnumerate(IEnumerable<PosBatch> source)
        {
            return source.Select(item => new PosBatchModel(item)).ToList();
        }

        public ActionResult Copy(Guid guid)
        {
            var posBatch = new PosBatch();
            var clone = new PosBatch();
            try
            {
                posBatch = DataGemini.PosBatches.FirstOrDefault(c => c.Guid == guid);
                #region Copy
                DataGemini.PosBatches.Add(clone);
                //Copy values from source to clone
                var sourceValues = DataGemini.Entry(posBatch).CurrentValues;
                DataGemini.Entry(clone).CurrentValues.SetValues(sourceValues);
                //Change values of the copied entity
                clone.Name = clone.Name + " - Copy";
                clone.Guid = Guid.NewGuid();
                clone.UpdatedAt = clone.CreatedAt = DateTime.Now;
                clone.UpdatedBy = clone.CreatedBy = GetUserInSession();
                if (SaveData("PosBatch"))
                {
                    DataReturn.ActiveCode = clone.Guid.ToString();
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                }
                else
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.Conflict);
                    DataReturn.MessagError = Constants.CannotCopy + " Date : " + DateTime.Now;
                }
                #endregion
            }
            catch (Exception ex)
            {
                DataGemini.PosBatches.Remove(clone);
                HandleError(ex);
            }


            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }
    }
}