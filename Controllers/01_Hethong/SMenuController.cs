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
    public class SMenuController : GeminiController
    {
        //
        //private SMenu sMenus;

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
            List<SMenu> sMenus = DataGemini.SMenus.OrderBy(p => p.OrderMenu).ToList();
            //DataSourceResult result = ConvertIEnumerate(sMenus).ToDataSourceResult(request);
            var data = ConvertIEnumerate(sMenus);
            var roots = BuildTree(data);
            foreach (var item in roots)
            {
                AppendChars(item);
            }

            var result = data.OrderBy(x => x.RootId).ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<SMenuModel> ConvertIEnumerate(IEnumerable<SMenu> source)
        {
            return source.Select(item => new SMenuModel(item)).ToList();
        }
        public ActionResult Create()
        {
            try
            {
                var sMenus = new SMenu();
                var viewModel = new SMenuModel(sMenus) { IsUpdate = 0, Active = true, OrderMenu = 0 };
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
                var sMenus = new SMenu();
                sMenus = DataGemini.SMenus.FirstOrDefault(c => c.Guid == guid);
                var viewModel = new SMenuModel(sMenus) { IsUpdate = 1 };
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
                var sMenus = new SMenu();
                sMenus = DataGemini.SMenus.FirstOrDefault(c => c.Guid == guid);

                var FRoleMenuAndControl = DataGemini.FRoleControlMenus.Where(c => c.GuidMenu == guid).ToList();
                if (FRoleMenuAndControl.Any())
                {
                    DataGemini.FRoleControlMenus.RemoveRange(FRoleMenuAndControl);
                }

                DataGemini.SMenus.Remove(sMenus);
                if (SaveData("SMenu") && sMenus != null)
                {
                    DataReturn.ActiveCode = sMenus.Guid.ToString();
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
        public ActionResult Update(SMenuModel viewModel)
        {
            var sMenus = new SMenu();
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
                        viewModel.Setvalue(sMenus);
                        DataGemini.SMenus.Add(sMenus);
                    }
                    else
                    {
                        sMenus = DataGemini.SMenus.FirstOrDefault(c => c.Guid == viewModel.Guid);
                        viewModel.Setvalue(sMenus);
                    }
                    if (SaveData("SMenu") && sMenus != null)
                    {
                        DataReturn.ActiveCode = sMenus.Guid.ToString();
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
                    DataGemini.SMenus.Remove(sMenus);
                }
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        private List<string> ValidateDuplicate(SMenuModel viewModel)
        {
            List<string> lstErrMsg = new List<string>();

            var lstMenu = DataGemini.SMenus.Where(c => c.LinkUrl.Equals(viewModel.LinkUrl, StringComparison.OrdinalIgnoreCase) && c.Guid != viewModel.Guid);

            if (lstMenu.Count() > 0)
            {
                lstErrMsg.Add("Menu trùng lặp LinkURL!");
            }

            return lstErrMsg;
        }

        public ActionResult Copy(Guid guid)
        {
            var clone = new SMenu();
            var sMenus = new SMenu();
            try
            {
                sMenus = DataGemini.SMenus.FirstOrDefault(c => c.Guid == guid);
                #region Copy
                DataGemini.SMenus.Add(clone);
                //Copy values from source to clone
                var sourceValues = DataGemini.Entry(sMenus).CurrentValues;
                DataGemini.Entry(clone).CurrentValues.SetValues(sourceValues);
                //Change values of the copied entity
                clone.Name = clone.Name + " - Copy";
                clone.Guid = Guid.NewGuid();
                clone.UpdatedAt = clone.CreatedAt = DateTime.Now;
                clone.UpdatedBy = clone.CreatedBy = GetUserInSession();
                if (SaveData("SMenu"))
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
                DataGemini.SMenus.Remove(clone);
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        public static IList<SMenuModel> BuildTree(IEnumerable<SMenuModel> source)
        {
            IList<SMenuModel> roots = new BindingList<SMenuModel>();
            var groups = source.GroupBy(i => i.GuidParent).ToList();
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

        private static void AddChildren(SMenuModel node, IDictionary<Guid, List<SMenuModel>> source, ref int order)
        {
            if (source.ContainsKey(node.Guid))
            {
                node.Items = source[node.Guid];
                foreach (SMenuModel t in node.Items)
                {
                    order++;
                    t.RootId = order;
                    AddChildren(t, source, ref order);
                }
            }
            else
            {
                node.Items = new List<SMenuModel>();
            }
        }

        private static void AppendChars(SMenuModel sMenu, string append = "")
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