using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Gemini.Controllers.Bussiness;
using Gemini.Models;
using Gemini.Models._04_Crm;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Constants = Gemini.Controllers.Bussiness.Constants;

namespace Gemini.Controllers._04_Crm
{
    [CustomAuthorizeAttribute]
    public class CrmEmailTemplateController : GeminiController
    {
        #region Main

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var userInformation = GetSettingUser();
            ViewBag.Pagesize = (userInformation.RecordsInPage == null || userInformation.RecordsInPage == 0) ? 10 : userInformation.RecordsInPage;
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var userInformation = GetSettingUser();
            //var find = new FINDBASEModel { tukhoa = vString.GetValueTostring(tukhoa) };
            IEnumerable<CrmEmailTemplate> crmCustomersGroup = DataGemini.CrmEmailTemplates.OrderBy(p => p.SubjectEmail).ToList();
            DataSourceResult result = ConvertIEnumerate(crmCustomersGroup).ToDataSourceResult(request);
            return Json(result);
        }

        private IEnumerable<CrmEmailTemplateModel> ConvertIEnumerate(IEnumerable<CrmEmailTemplate> source)
        {
            return source.Select(item => new CrmEmailTemplateModel(item)).ToList();
        }

        public ActionResult Preview(string contentTemplate)
        {
            var crmEmailTemplate = new CrmEmailTemplate();
            var viewModel = new CrmEmailTemplateModel(crmEmailTemplate) { ContentTemplate = contentTemplate };
            return PartialView("Preview", viewModel);
        }

        public ActionResult Create()
        {
            var crmEmailTemplate = new CrmEmailTemplate();
            try
            {
                var viewModel = new CrmEmailTemplateModel(crmEmailTemplate) { IsUpdate = 0, Active = true };
                return PartialView("Edit", viewModel);
            }
            catch (Exception)
            {
                return Redirect("/Error/ErrorList");
            }

        }

        public ActionResult Edit(Guid guid)
        {
            var crmEmailTemplate = new CrmEmailTemplate();
            try
            {
                var userInformation = GetSettingUser();
                crmEmailTemplate = DataGemini.CrmEmailTemplates.FirstOrDefault(c => c.Guid == guid);
                var viewModel = new CrmEmailTemplateModel(crmEmailTemplate) { IsUpdate = 1 };

                return PartialView("Edit", viewModel);
            }
            catch (Exception)
            {
                return Redirect("/Error/ErrorList");
            }
        }

        public ActionResult Delete(Guid guid)
        {
            var crmEmailTemplate = new CrmEmailTemplate();
            try
            {
                var userInformation = GetSettingUser();
                crmEmailTemplate = DataGemini.CrmEmailTemplates.FirstOrDefault(c => c.Guid == guid);

                DataGemini.CrmEmailTemplates.Remove(crmEmailTemplate);
                if (SaveData("CrmEmailTemplate") && crmEmailTemplate != null)
                {
                    DataReturn.ActiveCode = crmEmailTemplate.Guid.ToString();
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
        public ActionResult Update(CrmEmailTemplateModel viewModel)
        {
            var crmEmailTemplate = new CrmEmailTemplate();
            try
            {
                viewModel.UpdatedBy = viewModel.CreatedBy = GetUserInSession();
                if (viewModel.IsUpdate == 0)
                {
                    viewModel.Setvalue(crmEmailTemplate);
                    DataGemini.CrmEmailTemplates.Add(crmEmailTemplate);
                }
                else
                {
                    crmEmailTemplate = DataGemini.CrmEmailTemplates.FirstOrDefault(c => c.Guid == viewModel.Guid);
                    viewModel.Setvalue(crmEmailTemplate);
                }
                if (SaveData("CrmEmailTemplate") && crmEmailTemplate != null)
                {
                    DataReturn.ActiveCode = crmEmailTemplate.Guid.ToString();
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
                    DataGemini.CrmEmailTemplates.Remove(crmEmailTemplate);
                }
                HandleError(ex);
            }


            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Copy(Guid guid)
        {
            var crmEmailTemplate = new CrmEmailTemplate();
            var clone = new CrmEmailTemplate();
            try
            {
                crmEmailTemplate = DataGemini.CrmEmailTemplates.FirstOrDefault(c => c.Guid == guid);
                #region Copy
                DataGemini.CrmEmailTemplates.Add(clone);
                //Copy values from source to clone
                var sourceValues = DataGemini.Entry(crmEmailTemplate).CurrentValues;
                DataGemini.Entry(clone).CurrentValues.SetValues(sourceValues);
                //Change values of the copied entity
                clone.SubjectEmail = clone.SubjectEmail + " - Copy";
                clone.Guid = Guid.NewGuid();
                clone.UpdatedAt = clone.CreatedAt = DateTime.Now;
                clone.UpdatedBy = clone.CreatedBy = GetUserInSession();
                if (SaveData("CrmEmailTemplate"))
                {
                    DataReturn.ActiveCode = clone.Guid.ToString();
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                }
                else
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.Conflict);
                    DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                }
                #endregion
            }
            catch (Exception ex)
            {
                DataGemini.CrmEmailTemplates.Remove(crmEmailTemplate);
                HandleError(ex);
            }


            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}