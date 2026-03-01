using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class SRegionController : GeminiController
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

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<SRegion> sRegion = DataGemini.SRegions.OrderBy(p => p.Name).ToList();
            //DataSourceResult result = ConvertIEnumerate(sRegion).ToDataSourceResult(request);
            List<SRegion> sRegion = DataGemini.SRegions.OrderBy(p => p.Name).ToList();
            var data = ConvertIEnumerate(sRegion);
            var roots = BuildTree(data);
            foreach (var item in roots)
            {
                AppendChars(item);
            }
            var result = data.OrderBy(x => x.RootId).ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<SRegionModel> ConvertIEnumerate(IEnumerable<SRegion> source)
        {
            return source.Select(item => new SRegionModel(item)).ToList();
        }
        public ActionResult Create()
        {
            try
            {
                var sLanguages = new SRegion();
                var viewModel = new SRegionModel(sLanguages) { IsUpdate = 0, Active = true};
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
                var sLanguages = new SRegion();
                sLanguages = DataGemini.SRegions.FirstOrDefault(c => c.Guid == guid);
                var viewModel = new SRegionModel(sLanguages) { IsUpdate = 1 };
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
                var sLanguages = new SRegion();
                sLanguages = DataGemini.SRegions.FirstOrDefault(c => c.Guid == guid);
                DataGemini.SRegions.Remove(sLanguages);
                if (SaveData("SRegion") && sLanguages != null)
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
        public ActionResult Update(SRegionModel viewModel)
        {
            var sLanguages = new SRegion();
            try
            {
                viewModel.UpdatedBy = viewModel.CreatedBy = GetUserInSession();
                if (viewModel.IsUpdate == 0)
                {
                    viewModel.Setvalue(sLanguages);
                    DataGemini.SRegions.Add(sLanguages);
                }
                else
                {
                    sLanguages = DataGemini.SRegions.FirstOrDefault(c => c.Guid == viewModel.Guid);
                    viewModel.Setvalue(sLanguages);
                }
                if (SaveData("SRegion") && sLanguages != null)
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
                    DataGemini.SRegions.Remove(sLanguages);
                }
                HandleError(ex);
            }
            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Copy(Guid guid)
        {
            var clone = new SRegion();
            var sLanguages = new SRegion();
            try
            {
                sLanguages = DataGemini.SRegions.FirstOrDefault(c => c.Guid == guid);
                #region Copy
                DataGemini.SRegions.Add(clone);
                //Copy values from source to clone
                var sourceValues = DataGemini.Entry(sLanguages).CurrentValues;
                DataGemini.Entry(clone).CurrentValues.SetValues(sourceValues);
                //Change values of the copied entity
                clone.Name = clone.Name + " - Copy";
                clone.Guid = Guid.NewGuid();
                clone.UpdatedAt = clone.CreatedAt = DateTime.Now;
                clone.UpdatedBy = clone.CreatedBy = GetUserInSession();
                if (SaveData("SRegion"))
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
                DataGemini.SRegions.Remove(clone);
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
        //    List<SRegion> objExcel = DataGemini.SRegions.Where(p => p.Name.Contains(find.tukhoa)).OrderBy(p => p.Name).ToList();
        //    var grid = new GridView { DataSource = objExcel };
        //    ExporteExcel(grid, "SRegion.xls");

        //    return null;

        //}

        //public ActionResult Exportpdf()
        //{
        //    return null;

        //}
        //#endregion

        public static IList<SRegionModel> BuildTree(IEnumerable<SRegionModel> source)
        {
            IList<SRegionModel> roots = new BindingList<SRegionModel>();
            var groups = source.GroupBy(i => i.ParentGuid).ToList();
            var rootgroups = groups.FirstOrDefault(g => g.Key.HasValue == false);
            if (rootgroups != null)
            {
                roots = rootgroups.ToList();
                if (roots.Count > 0)
                {
                    var dict = groups.Where(g => g.Key.HasValue).ToDictionary(g => g.Key.Value, g => g.ToList());
                    int order = 0;
                    foreach (var t in roots)
                    {
                        order++;
                        t.RootId = order;
                        AddChildren(t, dict, ref order);
                    }
                }
            }

            return roots;
        }

        private static void AddChildren(SRegionModel node, IDictionary<Guid, List<SRegionModel>> source, ref int order)
        {
            if (source.ContainsKey(node.Guid))
            {
                node.Items = source[node.Guid];
                foreach (SRegionModel t in node.Items)
                {
                    order++;
                    t.RootId = order;
                    AddChildren(t, source, ref order);
                }
            }
            else
            {
                node.Items = new List<SRegionModel>();
            }
        }

        private static void AppendChars(SRegionModel sMenu, string append = "")
        {
            if (sMenu.Items != null && sMenu.Items.Any())
            {
                append += ">> ";
                foreach (var item in sMenu.Items)
                {
                    item.Name = string.Format("{0} {1}", append, item.Name);
                    AppendChars(item, append);
                }
            }
        }

    }
}
