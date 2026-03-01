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
    public class SLanguageController : GeminiController
    {
        //
        // GET: /Dmlanguage/

        //private SLanguage sLanguages;

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
            //var find = new FINDBASEModel { tukhoa = vString.GetValueTostring(tukhoa) };
            List<SLanguage> slanguages = DataGemini.SLanguages.OrderBy(p => p.Name).ToList();
            DataSourceResult result = ConvertIEnumerate(slanguages).ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<SLanguageModel> ConvertIEnumerate(IEnumerable<SLanguage> source)
        {
            return source.Select(item => new SLanguageModel(item)).ToList();
        }
        public ActionResult Create()
        {
            try
            {
                var sLanguages = new SLanguage();
                var viewModel = new SLanguageModel(sLanguages) { IsUpdate = 0, Active = true};
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
                var sLanguages = new SLanguage();
                sLanguages = DataGemini.SLanguages.FirstOrDefault(c => c.Guid == guid);
                var viewModel = new SLanguageModel(sLanguages) { IsUpdate = 1 };
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
                var sLanguages = new SLanguage();
                sLanguages = DataGemini.SLanguages.FirstOrDefault(c => c.Guid == guid);
                DataGemini.SLanguages.Remove(sLanguages);
                if (SaveData("SLanguage") && sLanguages != null)
                {
                    DataReturn.ActiveCode = sLanguages.Guid.ToString();
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
        public ActionResult Update(SLanguageModel viewModel)
        {
            var sLanguages = new SLanguage();
            try
            {
                viewModel.UpdatedBy = viewModel.CreatedBy = GetUserInSession();
                if (viewModel.IsUpdate == 0)
                {
                    viewModel.Setvalue(sLanguages);
                    DataGemini.SLanguages.Add(sLanguages);
                }
                else
                {
                    sLanguages = DataGemini.SLanguages.FirstOrDefault(c => c.Guid == viewModel.Guid);
                    viewModel.Setvalue(sLanguages);
                }
                if (SaveData("SLanguage") && sLanguages != null)
                {
                    DataReturn.ActiveCode = sLanguages.Guid.ToString();
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
                    DataGemini.SLanguages.Remove(sLanguages);
                }
                HandleError(ex);
            }
            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Copy(Guid guid)
        {
            var clone = new SLanguage();
            var sLanguages = new SLanguage();
            try
            {
                sLanguages = DataGemini.SLanguages.FirstOrDefault(c => c.Guid == guid);
                #region Copy
                DataGemini.SLanguages.Add(clone);
                //Copy values from source to clone
                var sourceValues = DataGemini.Entry(sLanguages).CurrentValues;
                DataGemini.Entry(clone).CurrentValues.SetValues(sourceValues);
                //Change values of the copied entity
                clone.Name = clone.Name + " - Copy";
                clone.Guid = Guid.NewGuid();
                clone.UpdatedAt = clone.CreatedAt = DateTime.Now;
                clone.UpdatedBy = clone.CreatedBy = GetUserInSession();
                if (SaveData("SLanguage"))
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
                DataGemini.SLanguages.Remove(clone);
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        //#region Export
        //public ActionResult Exportexcel(string tukhoa)
        //{
        //    var find = new FINDBASEModel
        //    {
        //        tukhoa = vString.GetValueTostring(tukhoa),
        //    };
        //    List<SLanguage> objExcel = DataGemini.SLanguages.Where(p => p.Name.Contains(find.tukhoa)).OrderBy(p => p.Name).ToList();
        //    var grid = new GridView { DataSource = objExcel };
        //    ExporteExcel(grid, "SLanguage.xls");

        //    return null;

        //}

        //public ActionResult Exportpdf()
        //{
        //    return null;

        //}
        //#endregion
    }
}
