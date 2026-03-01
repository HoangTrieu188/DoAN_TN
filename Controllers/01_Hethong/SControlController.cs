using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Gemini.Controllers.Bussiness;
using Gemini.Models;
using Gemini.Models._01_Hethong;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Constants = Gemini.Controllers.Bussiness.Constants;

namespace Gemini.Controllers._01_Hethong
{
    [CustomAuthorizeAttribute]
    public class SControlController : GeminiController
    {
        //private SControl sControls;

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

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            List<SControl> sControls = DataGemini.SControls.OrderBy(p => p.Name).ToList();
            DataSourceResult result = ConvertIEnumerate(sControls).ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<SControlModel> ConvertIEnumerate(IEnumerable<SControl> source)
        {
            return source.Select(item => new SControlModel(item)).ToList();
        }
        public ActionResult Create()
        {
            try
            {
                var sControls = new SControl();
                var viewModel = new SControlModel(sControls) { IsUpdate = 0, Active = true};
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
                var sControls = new SControl();
                sControls = DataGemini.SControls.FirstOrDefault(c => c.Guid == guid);
                var viewModel = new SControlModel(sControls) { IsUpdate = 1 };
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
                var sControls = new SControl();
                sControls = DataGemini.SControls.FirstOrDefault(c => c.Guid == guid);

                var FRoleMenuAndControl = DataGemini.FRoleControlMenus.Where(c => c.GuidMenu == guid).ToList();
                if (FRoleMenuAndControl.Any())
                {
                    DataGemini.FRoleControlMenus.RemoveRange(FRoleMenuAndControl);
                }

                DataGemini.SControls.Remove(sControls);
                if (SaveData("SControl") && sControls != null)
                {
                    DataReturn.ActiveCode = sControls.Guid.ToString();
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
        public ActionResult Update(SControlModel viewModel)
        {
            var sControls = new SControl();
            try
            {
                viewModel.UpdatedBy = viewModel.CreatedBy = GetUserInSession();
                if (viewModel.IsUpdate == 0)
                {
                    viewModel.Setvalue(sControls);
                    DataGemini.SControls.Add(sControls);
                }
                else
                {
                    sControls = DataGemini.SControls.FirstOrDefault(c => c.Guid == viewModel.Guid);
                    viewModel.Setvalue(sControls);
                }
                if (SaveData("SControl") && sControls != null)
                {
                    DataReturn.ActiveCode = sControls.Guid.ToString();
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
                    DataGemini.SControls.Remove(sControls);
                }
                HandleError(ex);
            }
            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Copy(Guid guid)
        {
            var clone = new SControl();
            var sControls = new SControl();
            try
            {
                sControls = DataGemini.SControls.FirstOrDefault(c => c.Guid == guid);
                #region Copy
                DataGemini.SControls.Add(clone);
                //Copy values from source to clone
                var sourceValues = DataGemini.Entry(sControls).CurrentValues;
                DataGemini.Entry(clone).CurrentValues.SetValues(sourceValues);
                //Change values of the copied entity
                clone.Name = clone.Name + " - Copy";
                clone.Guid = Guid.NewGuid();
                clone.UpdatedAt = clone.CreatedAt = DateTime.Now;
                clone.UpdatedBy = clone.CreatedBy = GetUserInSession();
                if (SaveData("SControl"))
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
                DataGemini.SControls.Remove(clone);
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }
        
    }
}
