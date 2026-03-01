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
    public class FControlMenuController : GeminiController
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
            GeminiEntities DataGemini = new GeminiEntities();

            List<SMenuModel> fRoleAndMenu = (from es in DataGemini.SRoles
                                             join frc in DataGemini.FRoleControlMenus on es.Guid equals frc.GuidRole
                                             join co in DataGemini.SMenus on frc.GuidMenu equals co.Guid
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

        public ActionResult ReadTabc1([DataSourceRequest] DataSourceRequest request, string guid)
        {
            GeminiEntities DataGemini = new GeminiEntities();

            List<SControlModel> fControlMenu = (from es in DataGemini.SMenus
                                                join frc in DataGemini.FControlMenus on es.Guid equals frc.GuidMenu
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
                                                   IsMenu = false
                                               }).ToList();

            foreach (var item in erpControls)
            {
                item.IsMenu = fControlMenu.FirstOrDefault(s => s.Guid == item.Guid) != null
                    ? true
                    : false;
            }

            DataSourceResult result = erpControls.ToDataSourceResult(request);
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveControlMenus(string controlCodes, string guidMenu)
        {
            try
            {
                if (string.IsNullOrEmpty(guidMenu))
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                    return Json(DataReturn, JsonRequestBehavior.AllowGet);
                }

                var controlList = controlCodes.Split(';').ToArray();
                if (controlList.Any())
                {
                    var fControlMenus = DataGemini.FControlMenus.Where(s => s.GuidMenu.ToString() == guidMenu).ToList();
                    if (fControlMenus.Any())
                    {
                        DataGemini.FControlMenus.RemoveRange(fControlMenus);
                    }
                    foreach (var item in controlList)
                    {
                        if (string.IsNullOrEmpty(item) == false)
                        {
                            FControlMenu fControlMenu = new FControlMenu();

                            fControlMenu.CreatedAt = DateTime.Now;
                            fControlMenu.CreatedBy = GetUserInSession();
                            fControlMenu.Guid = Guid.NewGuid();
                            fControlMenu.GuidControl = Guid.Parse(item);
                            fControlMenu.GuidMenu = Guid.Parse(guidMenu);

                            DataGemini.FControlMenus.Add(fControlMenu);
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