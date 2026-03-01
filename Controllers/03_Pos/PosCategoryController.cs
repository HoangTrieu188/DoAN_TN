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
using Gemini.Models._03_Pos;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Constants = Gemini.Controllers.Bussiness.Constants;

namespace Gemini.Controllers._03_Pos
{
    [CustomAuthorizeAttribute]
    public class PosCategoryController : GeminiController
    {
        #region Main

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
            List<PosCategory> posCategory = DataGemini.PosCategories.OrderBy(p => p.OrderBy).ToList();
            var data = ConvertIEnumerate(posCategory);
            var roots = BuildTreeCategory(data);
            foreach (var item in roots)
            {
                AppendCharsCategory(item);
            }

            var result = data.OrderBy(x => x.RootId).ThenBy(x => x.OrderBy).ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            var posCategory = new PosCategory();
            try
            {
                var viewModel = new PosCategoryModel(posCategory) { IsUpdate = 0, Active = true };
                return PartialView("Edit", viewModel);
            }
            catch (Exception)
            {
                return Redirect("/Error/ErrorList");
            }

        }

        public ActionResult Edit(Guid guid)
        {
            var posCategory = new PosCategory();
            try
            {
                PosCategoryModel viewModel;
                posCategory = DataGemini.PosCategories.FirstOrDefault(c => c.Guid == guid);
                if (posCategory.ParentGuid == null)
                {
                    viewModel = new PosCategoryModel(posCategory) { IsUpdate = 1 };
                }
                else
                {
                    var parent = DataGemini.PosCategories.FirstOrDefault(x => x.Guid == posCategory.ParentGuid);
                    viewModel = new PosCategoryModel(posCategory) { IsUpdate = 1 };
                }
                return PartialView("Edit", viewModel);
            }
            catch (Exception)
            {
                return Redirect("/Error/ErrorList");
            }


        }

        public ActionResult Delete(Guid guid)
        {
            var posCategory = new PosCategory();
            try
            {
                posCategory = DataGemini.PosCategories.FirstOrDefault(c => c.Guid == guid);

                #region Remove Produce and FProduceGallery

                var posProduce = DataGemini.PosProduces.Where(x => x.GuidCategory == guid).ToList();
                foreach (var item in posProduce)
                {
                    var fProduceGallery = DataGemini.FProduceGalleries.Where(x => x.GuidProduce == item.Guid).ToList();
                    DataGemini.FProduceGalleries.RemoveRange(fProduceGallery);
                }
                DataGemini.PosProduces.RemoveRange(posProduce);

                #endregion

                #region Remove Categories Child

                var childCategory = DataGemini.PosCategories.Where(c => c.ParentGuid == guid).ToList();
                if (childCategory.Any())
                {
                    DataGemini.PosCategories.RemoveRange(childCategory);
                }

                #endregion

                DataGemini.PosCategories.Remove(posCategory);
                if (SaveData("PosCategory") && posCategory != null)
                {
                    DataReturn.ActiveCode = posCategory.Guid.ToString();
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
        public ActionResult Update(PosCategoryModel viewModel)
        {
            var posCategory = new PosCategory();
            try
            {
                var lstErrMsg = ValidateCategory(viewModel);

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
                        viewModel.Setvalue(posCategory);
                        DataGemini.PosCategories.Add(posCategory);
                    }
                    else
                    {
                        posCategory = DataGemini.PosCategories.FirstOrDefault(c => c.Guid == viewModel.Guid);
                        viewModel.Setvalue(posCategory);
                    }
                    posCategory.SeoFriendUrl = posCategory.Guid.ToString();

                    if (SaveData("PosCategory") && posCategory != null)
                    {
                        DataReturn.ActiveCode = posCategory.Guid.ToString();
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
                    DataGemini.PosCategories.Remove(posCategory);
                }
                HandleError(ex);
            }
            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        private List<string> ValidateCategory(PosCategoryModel viewModel)
        {
            List<string> lstErrMsg = new List<string>();

            var lstUser = DataGemini.PosCategories.Where(c => c.Name.Equals(viewModel.Name, StringComparison.OrdinalIgnoreCase) && c.Guid != viewModel.Guid).ToList();

            if (lstUser.Count > 0)
            {
                lstErrMsg.Add("Tài khoản trùng Tên danh mục!");
            }

            return lstErrMsg;
        }

        private IEnumerable<PosCategoryModel> ConvertIEnumerate(IEnumerable<PosCategory> source)
        {
            return source.Select(item => new PosCategoryModel(item)).ToList();
        }

        public ActionResult Copy(Guid guid)
        {
            var posCategory = new PosCategory();
            var clone = new PosCategory();
            try
            {
                posCategory = DataGemini.PosCategories.FirstOrDefault(c => c.Guid == guid);
                #region Copy
                DataGemini.PosCategories.Add(clone);
                //Copy values from source to clone
                var sourceValues = DataGemini.Entry(posCategory).CurrentValues;
                DataGemini.Entry(clone).CurrentValues.SetValues(sourceValues);
                //Change values of the copied entity
                clone.Name = clone.Name + " - Copy";
                clone.Guid = Guid.NewGuid();
                clone.SeoFriendUrl = clone.Guid.ToString().ToLower();
                clone.UpdatedAt = clone.CreatedAt = DateTime.Now;
                clone.UpdatedBy = clone.CreatedBy = GetUserInSession();
                if (SaveData("PosCategory"))
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
                DataGemini.PosCategories.Remove(clone);
                HandleError(ex);
            }


            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}