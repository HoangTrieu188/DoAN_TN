using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Gemini.Controllers.Bussiness;
using Gemini.Models;
using Gemini.Models._05_Website;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Constants = Gemini.Controllers.Bussiness.Constants;

namespace Gemini.Controllers._05_Website
{
    [CustomAuthorizeAttribute]
    public class WOneParamController : GeminiController
    {
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
            List<WOneParam> wOneParam = DataGemini.WOneParams.OrderByDescending(p => p.CreatedAt).ToList();
            var data = ConvertIEnumerate(wOneParam);
            var result = data.OrderByDescending(x => x.CreatedAt).ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<WOneParamModel> ConvertIEnumerate(IEnumerable<WOneParam> source)
        {
            return source.Select(item => new WOneParamModel(item)).ToList();
        }

        public ActionResult Create()
        {
            try
            {
                var wOneParam = new WOneParam();
                var viewModel = new WOneParamModel(wOneParam) { IsUpdate = 0, Active = false };
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
                var wOneParam = new WOneParam();
                wOneParam = DataGemini.WOneParams.FirstOrDefault(c => c.Guid == guid);
                var viewModel = new WOneParamModel(wOneParam) { IsUpdate = 1 };
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
                var wOneParam = new WOneParam();
                wOneParam = DataGemini.WOneParams.FirstOrDefault(c => c.Guid == guid);
                DataGemini.WOneParams.Remove(wOneParam);
                if (SaveData("WOneParam") && wOneParam != null)
                {
                    DataReturn.ActiveCode = wOneParam.Guid.ToString();
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
        public ActionResult Update(WOneParamModel viewModel)
        {
            var wOneParam = new WOneParam();
            try
            {
                var lstErrMsg = ValidateDuplicate(viewModel);

                if (lstErrMsg.Count > 0)
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.Conflict);
                    DataReturn.MessagError = String.Join("<br/>", lstErrMsg);
                }
                else
                {
                    viewModel.UpdatedBy = viewModel.CreatedBy = GetUserInSession();
                    if (viewModel.IsUpdate == 0)
                    {
                        viewModel.Setvalue(wOneParam);
                        DataGemini.WOneParams.Add(wOneParam);
                    }
                    else
                    {
                        wOneParam = DataGemini.WOneParams.FirstOrDefault(c => c.Guid == viewModel.Guid);
                        viewModel.Setvalue(wOneParam);
                    }
                    if (SaveData("WOneParam") && wOneParam != null)
                    {
                        DataReturn.ActiveCode = wOneParam.Guid.ToString();
                        DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                    }
                    else
                    {
                        DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.Conflict);
                        DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                    }
                }
            }
            catch (Exception ex)
            {
                if (viewModel.IsUpdate == 0)
                {
                    DataGemini.WOneParams.Remove(wOneParam);
                }
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        private List<string> ValidateDuplicate(WOneParamModel viewModel)
        {
            List<string> lstErrMsg = new List<string>();

            var lstProduce = DataGemini.WOneParams.Where(c => c.Active && c.Guid != viewModel.Guid && c.Code.Equals(viewModel.Code, StringComparison.OrdinalIgnoreCase));

            if (lstProduce.Count() > 0)
            {
                lstErrMsg.Add("Tồn tại bản ghi đang kích hoạt!");
            }

            return lstErrMsg;
        }

        public ActionResult Copy(Guid guid)
        {
            var sTypes = new WOneParam();
            var clone = new WOneParam();
            try
            {
                sTypes = DataGemini.WOneParams.FirstOrDefault(c => c.Guid == guid);
                #region Copy
                DataGemini.WOneParams.Add(clone);
                //Copy values from source to clone
                var sourceValues = DataGemini.Entry(sTypes).CurrentValues;
                DataGemini.Entry(clone).CurrentValues.SetValues(sourceValues);
                //Change values of the copied entity
                clone.Code = clone.Code + " - Copy";
                clone.Guid = Guid.NewGuid();
                clone.Active = false;
                clone.UpdatedAt = clone.CreatedAt = DateTime.Now;
                clone.UpdatedBy = clone.CreatedBy = GetUserInSession();
                if (SaveData("WOneParam"))
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
                DataGemini.WOneParams.Remove(clone);
                HandleError(ex);
            }


            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }
    }
}