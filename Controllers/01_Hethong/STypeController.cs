using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Gemini.Controllers.Bussiness;
using Gemini.Models._01_Hethong;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Gemini.Models;
using Constants = Gemini.Controllers.Bussiness.Constants;

namespace Gemini.Controllers._01_Hethong
{
    [CustomAuthorizeAttribute]
    public class STypeController : GeminiController
    {
        //private SType sTypes;

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
            //var find = new FINDBASEModel { tukhoa = vString.GetValueTostring(tukhoa) };
            IEnumerable<SType> stypes =
                DataGemini.STypes.OrderBy(p => p.ValueType).ToList();
            DataSourceResult result = ConvertIEnumerate(stypes).ToDataSourceResult(request);
            return Json(result);
        }


        public ActionResult Create()
        {
            try
            {
                var sTypes = new SType();
                var viewModel = new STypeModel(sTypes) { IsUpdate = 0, Active = true};
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
                var sTypes = new SType();
                sTypes = DataGemini.STypes.FirstOrDefault(c => c.Guid == guid);
                var viewModel = new STypeModel(sTypes) { IsUpdate = 1 };
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
                var sTypes = new SType();
                sTypes = DataGemini.STypes.FirstOrDefault(c => c.Guid == guid);
                DataGemini.STypes.Remove(sTypes);
                if (SaveData("SType") && sTypes != null)
                {
                    DataReturn.ActiveCode = sTypes.Guid.ToString();
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
        public ActionResult Update(STypeModel viewModel)
        {
            var sTypes = new SType();
            try
            {
                viewModel.UpdatedBy = viewModel.CreatedBy = GetUserInSession();
                if (viewModel.IsUpdate == 0)
                {
                    viewModel.Setvalue(sTypes);
                    DataGemini.STypes.Add(sTypes);
                }
                else
                {
                    sTypes = DataGemini.STypes.FirstOrDefault(c => c.Guid == viewModel.Guid);
                    viewModel.Setvalue(sTypes);
                }
                if (SaveData("SType") && sTypes != null)
                {
                    DataReturn.ActiveCode = sTypes.Guid.ToString();
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
                    DataGemini.STypes.Remove(sTypes);
                }
                HandleError(ex);
            }
            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }
        
        private IEnumerable<STypeModel> ConvertIEnumerate(IEnumerable<SType> source)
        {
            return source.Select(item => new STypeModel(item)).ToList();
        }

        public ActionResult Copy(Guid guid)
        {
            var sTypes = new SType();
            var clone = new SType();
            try
            {
                sTypes = DataGemini.STypes.FirstOrDefault(c => c.Guid == guid);
                #region Copy
                DataGemini.STypes.Add(clone);
                //Copy values from source to clone
                var sourceValues = DataGemini.Entry(sTypes).CurrentValues;
                DataGemini.Entry(clone).CurrentValues.SetValues(sourceValues);
                //Change values of the copied entity
                clone.ValueType = clone.ValueType + " - Copy";
                clone.Guid = Guid.NewGuid();
                clone.UpdatedAt = clone.CreatedAt = DateTime.Now;
                clone.UpdatedBy = clone.CreatedBy = GetUserInSession();
                if (SaveData("SType"))
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
                DataGemini.STypes.Remove(clone);
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
        //    List<SType> objExcel = DataGemini.STypes.Where(p => p.ValueType.Contains(find.tukhoa)).OrderBy(p => p.ValueType).ToList();
        //    var grid = new GridView { DataSource = objExcel };
        //    ExporteExcel(grid, "SType.xls");

        //    return null;

        //}

        //public ActionResult Exportpdf()
        //{
        //    return null;

        //}
        //#endregion
    }
}
