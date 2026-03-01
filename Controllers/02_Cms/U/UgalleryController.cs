using System;
using System.Collections.Generic;
using System.IO;
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

namespace Gemini.Controllers._02_Cms.U
{
    [CustomAuthorizeAttribute]
    public class UGalleryController : GeminiController
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
            List<UGalleryModel> uGalleryModels = (from ug in DataGemini.UGalleries
                                                  select new UGalleryModel
                                                  {
                                                      Guid = ug.Guid,
                                                      Name = ug.Name,
                                                      Description = ug.Description,
                                                      GuidGroup = ug.GuidGroup,
                                                      Link = ug.Link,
                                                      Image = ug.Image,
                                                      Active = ug.Active,
                                                      Note = ug.Note,
                                                      CreatedAt = ug.CreatedAt,
                                                      CreatedBy = ug.CreatedBy,
                                                      UpdatedAt = ug.UpdatedAt,
                                                      UpdatedBy = ug.UpdatedBy,
                                                  }).OrderBy(s => s.Name).ToList();
            DataSourceResult result = uGalleryModels.ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult Delete(Guid guid)
        {
            var uGallery = new UGallery();
            try
            {
                uGallery = DataGemini.UGalleries.FirstOrDefault(c => c.Guid == guid);

                #region Delete Physical

                var url = uGallery.Image.Replace(ConstantsImage.Slash, "/");
                if (!string.IsNullOrEmpty(url))
                {
                    var folder = Path.GetDirectoryName(url);
                    var fileNameImage = Path.GetFileName(url);
                    fileNameImage = fileNameImage.Replace(ConstantsImage.Space, " ");
                    FileInfo fileImage = new FileInfo(Server.MapPath(folder) + "\\" + fileNameImage);
                    if (fileImage.Exists)
                    {
                        fileImage.Delete();
                    }

                    var fullPath = Path.Combine(Server.MapPath(folder));

                    string filesToDelete = @"*" + uGallery.Guid + "*" + ConstantsImage.FormatJpgImage;
                    try
                    {
                        string[] fileListImage = Directory.GetFiles(fullPath, filesToDelete);
                        foreach (var item in fileListImage)
                        {
                            FileInfo fileImages = new FileInfo(item);
                            if (fileImages.Exists)
                            {
                                fileImages.Delete();
                            }
                        }
                    }
                    catch
                    {

                    }
                }

                #endregion

                #region Delete FProduceGallery

                var fProduceGallery = DataGemini.FProduceGalleries.Where(x => x.GuidGallery == guid).ToList();
                if (fProduceGallery.Any())
                {
                    DataGemini.FProduceGalleries.RemoveRange(fProduceGallery);
                }

                #endregion

                DataGemini.UGalleries.Remove(uGallery);
                if (SaveData("U_GALLERY") && uGallery != null)
                {
                    DataReturn.ActiveCode = uGallery.Guid.ToString();
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
        #endregion

        [AllowAnonymous]
        public ActionResult ReadProduce([DataSourceRequest] DataSourceRequest request, string guid, string lstFilePath)
        {
            List<UGalleryModel> uGalleryModel = new List<UGalleryModel>();

            if (!string.IsNullOrWhiteSpace(guid))
            {
                uGalleryModel = (from ug in DataGemini.UGalleries
                                 join fpg in DataGemini.FProduceGalleries on ug.Guid equals fpg.GuidGallery
                                 join pp in DataGemini.PosProduces on fpg.GuidProduce equals pp.Guid
                                 where pp.Guid.ToString() == guid
                                 select new UGalleryModel
                                 {
                                     Guid = ug.Guid,
                                     Name = ug.Name,
                                     Description = ug.Description,
                                     GuidGroup = ug.GuidGroup,
                                     Link = ug.Link,
                                     Image = ug.Image,
                                     Active = ug.Active,
                                     Note = ug.Note,
                                     IsProduce = true,
                                     CreatedAt = ug.CreatedAt,
                                     CreatedBy = ug.CreatedBy,
                                     UpdatedAt = ug.UpdatedAt,
                                     UpdatedBy = ug.UpdatedBy,
                                 }).OrderByDescending(s => s.CreatedAt).ToList();
            }

            if (!string.IsNullOrWhiteSpace(lstFilePath))
            {
                lstFilePath = lstFilePath.Replace(@"\", @"/");
                uGalleryModel = (from ug in DataGemini.UGalleries
                                 where lstFilePath.Contains(ug.Image)
                                 select new UGalleryModel
                                 {
                                     Guid = ug.Guid,
                                     Name = ug.Name,
                                     Description = ug.Description,
                                     GuidGroup = ug.GuidGroup,
                                     Link = ug.Link,
                                     Image = ug.Image,
                                     Active = ug.Active,
                                     Note = ug.Note,
                                     IsProduce = true,
                                     CreatedAt = ug.CreatedAt,
                                     CreatedBy = ug.CreatedBy,
                                     UpdatedAt = ug.UpdatedAt,
                                     UpdatedBy = ug.UpdatedBy,
                                 }).OrderByDescending(s => s.CreatedAt).ToList();
            }

            DataSourceResult result = uGalleryModel.ToDataSourceResult(request);
            return Json(result);
        }
    }
}