using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Gemini.Controllers.Bussiness;
using Gemini.Models;
using Gemini.Models._02_Cms.U;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Constants = Gemini.Controllers.Bussiness.Constants;

namespace Gemini.Controllers._01_Hethong
{
    [CustomAuthorizeAttribute]
    public class UGroupController : GeminiController
    {
        public ActionResult Index(string id)
        {
            return RedirectToAction("List", new { type = id });
        }

        /// <summary>
        /// List view grid
        /// </summary>
        /// <returns></returns>
        public ActionResult List(string type)
        {
            ViewData["type"] = type;
            GetSettingUser();
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request, string type)
        {
            List<UGroup> uGroups = DataGemini.UGroups.Where(s => s.Type == type.Trim()).OrderBy(p => p.Name).ToList();
            var data = ConvertIEnumerate(uGroups);
            var roots = BuildTree(data);
            foreach (var item in roots)
            {
                AppendChars(item);
            }
            DataSourceResult result = data.OrderBy(x => x.RootId).ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<UGroupModel> ConvertIEnumerate(IEnumerable<UGroup> source)
        {
            return source.Select(item => new UGroupModel(item)).ToList();
        }

        public ActionResult Create(string type)
        {
            try
            {
                var uGroup = new UGroup();
                var viewModel = new UGroupModel(uGroup) { IsUpdate = 0, Active = true, Type = type };
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
                var uGroup = new UGroup();
                uGroup = DataGemini.UGroups.FirstOrDefault(c => c.Guid == guid);
                var viewModel = new UGroupModel(uGroup) { IsUpdate = 1 };
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
                var uGroup = new UGroup();
                uGroup = DataGemini.UGroups.FirstOrDefault(c => c.Guid == guid);
                DataGemini.UGroups.Remove(uGroup);
                if (SaveData("UGroup") && uGroup != null)
                {
                    DataReturn.ActiveCode = uGroup.Guid.ToString();
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
        public ActionResult Update(UGroupModel viewModel)
        {
            var uGroup = new UGroup();
            try
            {
                viewModel.UpdatedBy = viewModel.CreatedBy = GetUserInSession();
                if (viewModel.IsUpdate == 0)
                {
                    viewModel.Setvalue(uGroup);
                    DataGemini.UGroups.Add(uGroup);
                }
                else
                {
                    uGroup = DataGemini.UGroups.FirstOrDefault(c => c.Guid == viewModel.Guid);
                    viewModel.Setvalue(uGroup);
                }
                uGroup.SeoFriendUrl = uGroup.Guid.ToString();

                if (SaveData("UGroup") && uGroup != null)
                {
                    DataReturn.ActiveCode = uGroup.Guid.ToString();
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                }
                else
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.Conflict);
                    DataReturn.MessagError = Constants.CannotUpdate + " Date: " + DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                if (viewModel.IsUpdate == 0)
                {
                    DataGemini.UGroups.Remove(uGroup);
                }
                HandleError(ex);
            }
            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Copy(Guid guid)
        {
            var clone = new UGroup();
            try
            {
                var uGroup = new UGroup();
                uGroup = DataGemini.UGroups.FirstOrDefault(c => c.Guid == guid);
                #region Copy
                DataGemini.UGroups.Add(clone);
                //Copy values from source to clone
                var sourceValues = DataGemini.Entry(uGroup).CurrentValues;
                DataGemini.Entry(clone).CurrentValues.SetValues(sourceValues);
                //Change values of the copied entity
                clone.Name = clone.Name + " - Copy";
                clone.Guid = Guid.NewGuid();
                clone.SeoFriendUrl = uGroup.Guid.ToString();
                clone.UpdatedAt = clone.CreatedAt = DateTime.Now;
                clone.UpdatedBy = clone.CreatedBy = GetUserInSession();
                if (SaveData("UGroup"))
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
                DataGemini.UGroups.Remove(clone);
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        public static IList<UGroupModel> BuildTree(IEnumerable<UGroupModel> source)
        {
            IList<UGroupModel> roots = new BindingList<UGroupModel>();
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

        private static void AddChildren(UGroupModel node, IDictionary<Guid, List<UGroupModel>> source, ref int order)
        {
            if (source.ContainsKey(node.Guid))
            {
                node.Items = source[node.Guid];
                foreach (UGroupModel t in node.Items)
                {
                    order++;
                    t.RootId = order;
                    AddChildren(t, source, ref order);
                }
            }
            else
            {
                node.Items = new List<UGroupModel>();
            }
        }

        private static void AppendChars(UGroupModel uGroup, string append = "")
        {
            if (uGroup.Items != null && uGroup.Items.Any())
            {
                append += ">> ";
                foreach (var item in uGroup.Items)
                {
                    item.Name = string.Format("{0} {1}", append, item.Name);
                    AppendChars(item, append);
                }
            }
        }
    }
}