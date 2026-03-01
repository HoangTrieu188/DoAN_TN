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
    public class CrmEmailSettingController : GeminiController
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
            IEnumerable<CrmEmailSetting> crmCustomersGroup = DataGemini.CrmEmailSettings.OrderBy(p => p.Email).ToList();
            DataSourceResult result = ConvertIEnumerate(crmCustomersGroup).ToDataSourceResult(request);
            return Json(result);
        }

        private IEnumerable<CrmEmailSettingModel> ConvertIEnumerate(IEnumerable<CrmEmailSetting> source)
        {
            return source.Select(item => new CrmEmailSettingModel(item)).ToList();
        }

        public ActionResult Create()
        {
            try
            {
                var crmEmailSetting = new CrmEmailSetting();
                var viewModel = new CrmEmailSettingModel(crmEmailSetting)
                {
                    IsUpdate = 0,
                    Active = true,
                    Smtp = "smtp.gmail.com",
                    Port = 587
                };
                return PartialView("Edit", viewModel);
            }
            catch (Exception)
            {
                return Redirect("/Error/ErrorList");
            }

        }

        public ActionResult Edit(Guid guid)
        {
            var crmEmailSetting = new CrmEmailSetting();
            try
            {
                var userInformation = GetSettingUser();
                crmEmailSetting = DataGemini.CrmEmailSettings.FirstOrDefault(c => c.Guid == guid);
                var viewModel = new CrmEmailSettingModel(crmEmailSetting) { IsUpdate = 1 };

                return PartialView("Edit", viewModel);
            }
            catch (Exception)
            {
                return Redirect("/Error/ErrorList");
            }


        }

        public ActionResult Delete(Guid guid)
        {
            var crmEmailSetting = new CrmEmailSetting();
            try
            {
                var userInformation = GetSettingUser();
                crmEmailSetting = DataGemini.CrmEmailSettings.FirstOrDefault(c => c.Guid == guid);
                DataGemini.CrmEmailSettings.Remove(crmEmailSetting);
                if (SaveData("CrmEmailSetting") && crmEmailSetting != null)
                {
                    DataReturn.ActiveCode = crmEmailSetting.Guid.ToString();
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
        public ActionResult Update(CrmEmailSettingModel viewModel)
        {
            var crmEmailSetting = new CrmEmailSetting();
            try
            {
                viewModel.CreatedBy = viewModel.UpdatedBy = GetUserInSession();
                if (viewModel.IsUpdate == 0)
                {
                    viewModel.Setvalue(crmEmailSetting);
                    DataGemini.CrmEmailSettings.Add(crmEmailSetting);
                }
                else
                {
                    crmEmailSetting = DataGemini.CrmEmailSettings.FirstOrDefault(c => c.Guid == viewModel.Guid);
                    viewModel.Setvalue(crmEmailSetting);
                }
                if (SaveData("CrmEmailSetting") && crmEmailSetting != null)
                {
                    DataReturn.ActiveCode = crmEmailSetting.Guid.ToString();
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
                    DataGemini.CrmEmailSettings.Remove(crmEmailSetting);
                }
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Copy(Guid guid)
        {
            var crmEmailSetting = new CrmEmailSetting();
            var clone = new CrmEmailSetting();
            try
            {
                crmEmailSetting = DataGemini.CrmEmailSettings.FirstOrDefault(c => c.Guid == guid);
                #region Copy
                DataGemini.CrmEmailSettings.Add(clone);
                //Copy values from source to clone
                var sourceValues = DataGemini.Entry(crmEmailSetting).CurrentValues;
                DataGemini.Entry(clone).CurrentValues.SetValues(sourceValues);
                //Change values of the copied entity
                clone.Email = clone.Email + " - Copy";
                clone.Guid = Guid.NewGuid();
                clone.Email = clone.Email + 1;
                clone.UpdatedAt = clone.CreatedAt = DateTime.Now;
                clone.UpdatedBy = clone.CreatedBy = GetUserInSession();
                if (SaveData("CrmEmailSetting") && crmEmailSetting != null)
                {
                    DataReturn.ActiveCode = clone.Guid.ToString();
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                }
                else
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                    DataReturn.MessagError = Constants.CannotCopy + " Date : " + DateTime.Now;
                }

                #endregion
            }
            catch (Exception ex)
            {
                DataGemini.CrmEmailSettings.Remove(crmEmailSetting);
                HandleError(ex);
            }
            return Json(DataReturn, JsonRequestBehavior.AllowGet);

        }
        #endregion
    }
}