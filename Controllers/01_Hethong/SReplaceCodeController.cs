using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Gemini.Controllers.Bussiness;
using Gemini.Models;
using Gemini.Models._01_Hethong;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Gemini.Controllers._01_Hethong
{
    [CustomAuthorizeAttribute]
    public class SReplaceCodeController : GeminiController
    {
        #region Main

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            GetSettingUser();
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            List<SReplaceCode> sReplaceCode = DataGemini.SReplaceCodes.OrderBy(p => p.ReplaceCode).ToList();
            DataSourceResult result = ConvertIEnumerate(sReplaceCode).ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult Create()
        {
            var sReplaceCode = new SReplaceCode();
            try
            {
                var viewModel = new SReplaceCodeModel(sReplaceCode) { IsUpdate = 0, Active = true };
                return PartialView("Edit", viewModel);
            }
            catch (Exception)
            {
                return Redirect("/Error/ErrorList");
            }

        }

        public ActionResult Edit(Guid guid)
        {
            var sReplaceCode = new SReplaceCode();
            try
            {
                sReplaceCode = DataGemini.SReplaceCodes.FirstOrDefault(c => c.Guid == guid);
                var viewModel = new SReplaceCodeModel(sReplaceCode) { IsUpdate = 1 };
                return PartialView("Edit", viewModel);
            }
            catch (Exception)
            {
                return Redirect("/Error/ErrorList");
            }


        }

        public ActionResult Delete(Guid guid)
        {
            var sReplaceCode = new SReplaceCode();
            try
            {
                sReplaceCode = DataGemini.SReplaceCodes.FirstOrDefault(c => c.Guid == guid);
                DataGemini.SReplaceCodes.Remove(sReplaceCode);
                if (SaveData("SReplaceCode") && sReplaceCode != null)
                {
                    DataReturn.ActiveCode = sReplaceCode.Guid.ToString();
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
        public ActionResult Update(SReplaceCodeModel viewModel)
        {
            var sReplaceCode = new SReplaceCode();
            try
            {
                viewModel.UpdatedBy = viewModel.CreatedBy = GetUserInSession();
                if (viewModel.IsUpdate == 0)
                {
                    viewModel.Setvalue(sReplaceCode);
                    DataGemini.SReplaceCodes.Add(sReplaceCode);
                }
                else
                {
                    sReplaceCode = DataGemini.SReplaceCodes.FirstOrDefault(c => c.Guid == viewModel.Guid);
                    viewModel.Setvalue(sReplaceCode);
                }
                if (SaveData("SReplaceCode") && sReplaceCode != null)
                {
                    DataReturn.ActiveCode = sReplaceCode.Guid.ToString();
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                }
                else
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.Conflict);
                    DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                if (viewModel.IsUpdate == 0)
                {
                    DataGemini.SReplaceCodes.Remove(sReplaceCode);
                }
                HandleError(ex);
            }
            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<SReplaceCodeModel> ConvertIEnumerate(IEnumerable<SReplaceCode> source)
        {
            return source.Select(item => new SReplaceCodeModel(item)).ToList();
        }

        public ActionResult Copy(Guid guid)
        {
            var sReplaceCode = new SReplaceCode();
            var clone = new SReplaceCode();
            try
            {
                sReplaceCode = DataGemini.SReplaceCodes.FirstOrDefault(c => c.Guid == guid);
                #region Copy
                DataGemini.SReplaceCodes.Add(clone);
                //Copy values from source to clone
                var sourceValues = DataGemini.Entry(sReplaceCode).CurrentValues;
                DataGemini.Entry(clone).CurrentValues.SetValues(sourceValues);
                //Change values of the copied entity
                clone.ReplaceCode = clone.ReplaceCode + " - Copy";
                clone.Guid = Guid.NewGuid();
                clone.UpdatedAt = clone.CreatedAt = DateTime.Now;
                clone.UpdatedBy = clone.CreatedBy = GetUserInSession();
                if (SaveData("SReplaceCode"))
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
                DataGemini.SReplaceCodes.Remove(clone);
                HandleError(ex);
            }


            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}