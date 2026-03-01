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
using SINNOVA.Core;
using Constants = Gemini.Controllers.Bussiness.Constants;

namespace Gemini.Controllers._01_Hethong
{
    [CustomAuthorizeAttribute]
    public class SRoleController : GeminiController
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

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            List<SRole> sRoles = DataGemini.SRoles.OrderBy(p => p.Name).ToList();
            DataSourceResult result = ConvertIEnumerate(sRoles).ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<SRoleModel> ConvertIEnumerate(IEnumerable<SRole> source)
        {
            return source.Select(item => new SRoleModel(item)).ToList();
        }

        public ActionResult Create()
        {
            var user = GetSettingUser();
            ViewBag.IsAdmin = user.IsAdmin;

            try
            {
                var sRoles = new SRole();
                var viewModel = new SRoleModel(sRoles) { IsUpdate = 0, Active = true };
                return PartialView("Edit", viewModel);
            }
            catch
            {
                return Redirect("/Error/ErrorList");
            }
        }

        public ActionResult Edit(Guid guid)
        {
            var user = GetSettingUser();
            ViewBag.IsAdmin = user.IsAdmin;

            try
            {
                var sRoles = new SRole();
                sRoles = DataGemini.SRoles.FirstOrDefault(c => c.Guid == guid);
                var viewModel = new SRoleModel(sRoles) { IsUpdate = 1 };
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
                var sRoles = new SRole();

                var FRoleMenuAndControl = DataGemini.FRoleControlMenus.Where(c => c.GuidRole == guid).ToList();
                if (FRoleMenuAndControl.Any())
                {
                    DataGemini.FRoleControlMenus.RemoveRange(FRoleMenuAndControl);
                }

                sRoles = DataGemini.SRoles.FirstOrDefault(c => c.Guid == guid);
                if (sRoles != null)
                {
                    DataGemini.SRoles.Remove(sRoles);
                }


                if (SaveData("SRole") && sRoles != null)
                {
                    DataReturn.ActiveCode = sRoles.Guid.ToString();
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
        public ActionResult Update(SRoleModel viewModel)
        {
            var sRoles = new SRole();
            try
            {
                viewModel.UpdatedBy = viewModel.CreatedBy = GetUserInSession();
                if (viewModel.IsUpdate == 0)
                {
                    viewModel.Setvalue(sRoles);
                    DataGemini.SRoles.Add(sRoles);
                }
                else
                {
                    sRoles = DataGemini.SRoles.FirstOrDefault(c => c.Guid == viewModel.Guid);
                    viewModel.Setvalue(sRoles);
                }
                if (SaveData("SRole") && sRoles != null)
                {
                    DataReturn.ActiveCode = sRoles.Guid.ToString();
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
                    DataGemini.SRoles.Remove(sRoles);
                }
                HandleError(ex);
            }
            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Copy(Guid guid)
        {
            var clone = new SRole();
            var sRoles = new SRole();
            try
            {
                sRoles = DataGemini.SRoles.FirstOrDefault(c => c.Guid == guid);
                #region Copy
                DataGemini.SRoles.Add(clone);
                //Copy values from source to clone
                var sourceValues = DataGemini.Entry(sRoles).CurrentValues;
                DataGemini.Entry(clone).CurrentValues.SetValues(sourceValues);
                //Change values of the copied entity
                clone.Name = clone.Name + " - Copy";
                clone.Guid = Guid.NewGuid();
                clone.UpdatedAt = clone.CreatedAt = DateTime.Now;
                clone.UpdatedBy = clone.CreatedBy = GetUserInSession();
                if (SaveData("SRole"))
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
                DataGemini.SRoles.Remove(clone);
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadTabc1([DataSourceRequest] DataSourceRequest request, string guid)
        {
            GeminiEntities DataGemini = new GeminiEntities();

            List<SControlModel> fRoleAndControl = (from es in DataGemini.SRoles
                                                   join frc in DataGemini.FRoleControlMenus on es.Guid equals frc.GuidRole
                                                   join co in DataGemini.SControls on frc.GuidControl equals co.Guid
                                                   where es.Guid.ToString() == guid
                                                   select new SControlModel
                                                   {
                                                       Guid = co.Guid
                                                   }).ToList();
            List<SControlModel> erpControls = (from co in DataGemini.SControls
                                               select new SControlModel
                                               {
                                                   Guid = co.Guid,
                                                   Name = co.Name,
                                                   Active = co.Active,
                                                   Note = co.Note,
                                                   UpdatedAt = co.UpdatedAt,
                                                   UpdatedBy = co.UpdatedBy,
                                                   CreatedAt = co.CreatedAt,
                                                   CreatedBy = co.CreatedBy,
                                                   IsRole = false
                                               }).ToList();

            foreach (var item in erpControls)
            {
                item.IsRole = fRoleAndControl.FirstOrDefault(s => s.Guid == item.Guid) != null
                    ? true
                    : false;
            }

            DataSourceResult result = erpControls.ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult ReadTabc2([DataSourceRequest] DataSourceRequest request, string guid)
        {
            GeminiEntities DataGemini = new GeminiEntities();

            List<SMenuModel> fRoleAndMenu = (from es in DataGemini.SRoles
                                             join frc in DataGemini.FRoleControlMenus on es.Guid equals frc.GuidRole
                                             join co in DataGemini.SMenus on frc.GuidMenu equals co.Guid
                                             where es.Guid.ToString() == guid
                                             select new SMenuModel
                                             {
                                                 Guid = co.Guid
                                             }).ToList();
            List<SMenuModel> erpMenus = (from co in DataGemini.SMenus
                                         select new SMenuModel
                                         {
                                             Guid = co.Guid,
                                             Name = co.Name,
                                             Active = co.Active,
                                             Note = co.Note,
                                             UpdatedAt = co.UpdatedAt,
                                             UpdatedBy = co.UpdatedBy,
                                             CreatedAt = co.CreatedAt,
                                             CreatedBy = co.CreatedBy,
                                             GuidParent = co.GuidParent,
                                             IsRoleMenu = false
                                         }).ToList();

            foreach (var item in erpMenus)
            {
                item.IsRoleMenu = fRoleAndMenu.FirstOrDefault(s => s.Guid == item.Guid) != null
                    ? true
                    : false;
            }
            var roots = BuildTree(erpMenus);
            foreach (var item in roots)
            {
                AppendChars(item);
            }
            DataSourceResult result = erpMenus.OrderBy(x => x.RootId).ToDataSourceResult(request);
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveControls(string controlCodes, string guidRole)
        {
            try
            {
                if (string.IsNullOrEmpty(guidRole))
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                    return Json(DataReturn, JsonRequestBehavior.AllowGet);
                }

                var controlList = controlCodes.Split(';').ToArray();
                if (controlList.Any())
                {
                    var fRoleAndControlList = DataGemini.FRoleControlMenus.Where(s => s.GuidRole.ToString() == guidRole && s.GuidMenu == Guid.Empty).ToList();
                    if (fRoleAndControlList.Any())
                    {
                        DataGemini.FRoleControlMenus.RemoveRange(fRoleAndControlList);
                    }
                    foreach (var item in controlList)
                    {
                        if (string.IsNullOrEmpty(item) == false)
                        {
                            FRoleControlMenu froleAndControl = new FRoleControlMenu();

                            froleAndControl.CreatedAt = DateTime.Now;
                            froleAndControl.CreatedBy = GetUserInSession();
                            froleAndControl.Guid = Guid.NewGuid();
                            froleAndControl.GuidRole = Guid.Parse(guidRole);
                            froleAndControl.GuidControl = Guid.Parse(item);
                            froleAndControl.GuidMenu = Guid.Empty;
                            DataGemini.FRoleControlMenus.Add(froleAndControl);
                        }

                    }
                }
                DataGemini.SaveChanges();
                DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                return Json(DataReturn, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                HandleError(ex);
                DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                return Json(DataReturn, JsonRequestBehavior.AllowGet);
            }

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveMenus(string menuCodes, string guidRole)
        {
            try
            {
                if (string.IsNullOrEmpty(guidRole))
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                    return Json(DataReturn, JsonRequestBehavior.AllowGet);
                }
                var menuList = menuCodes.Split(';').ToArray();
                if (menuList.Any())
                {
                    var fRoleAndMenuList = DataGemini.FRoleControlMenus.Where(s => s.GuidRole.ToString() == guidRole && s.GuidControl == Guid.Empty).ToList();
                    if (fRoleAndMenuList.Any())
                    {
                        DataGemini.FRoleControlMenus.RemoveRange(fRoleAndMenuList);
                    }
                    foreach (var item in menuList)
                    {
                        if (string.IsNullOrEmpty(item) == false)
                        {
                            FRoleControlMenu froleAndMenu = new FRoleControlMenu();

                            froleAndMenu.CreatedAt = DateTime.Now;
                            froleAndMenu.CreatedBy = GetUserInSession();
                            froleAndMenu.Guid = Guid.NewGuid();
                            froleAndMenu.GuidRole = Guid.Parse(guidRole);
                            froleAndMenu.GuidMenu = Guid.Parse(item);
                            froleAndMenu.GuidControl = Guid.Empty;
                            DataGemini.FRoleControlMenus.Add(froleAndMenu);
                        }

                    }
                }
                DataGemini.SaveChanges();
                DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                return Json(DataReturn, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                HandleError(ex);
                DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                return Json(DataReturn, JsonRequestBehavior.AllowGet);
            }

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