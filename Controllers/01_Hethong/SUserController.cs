using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
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
    public class SUserController : GeminiController
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
            List<SUser> sUsers = DataGemini.SUsers.OrderBy(p => p.Username).ToList();
            DataSourceResult result = ConvertIEnumerate(sUsers).ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        private IEnumerable<SUserModel> ConvertIEnumerate(IEnumerable<SUser> source)
        {
            return source.Select(item => new SUserModel(item)).ToList();
        }

        public ActionResult Create()
        {
            try
            {
                var sUsers = new SUser();
                var viewModel = new SUserModel(sUsers) { IsUpdate = 0, Active = true };
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
                var sUsers = new SUser();
                sUsers = DataGemini.SUsers.FirstOrDefault(c => c.Guid == guid);
                var viewModel = new SUserModel(sUsers) { IsUpdate = 1 };
                return PartialView("Edit", viewModel);
            }
            catch
            {
                return Redirect("/Error/ErrorList");
            }
        }

        [AllowAnonymous]
        public ActionResult EditPopup(Guid guid)
        {
            try
            {
                var sUsers = new SUser();
                sUsers = DataGemini.SUsers.FirstOrDefault(c => c.Guid == guid);
                var viewModel = new SUserModel(sUsers) { IsUpdate = 1 };
                return PartialView("EditPopup", viewModel);
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
                var sUsers = new SUser();
                sUsers = DataGemini.SUsers.FirstOrDefault(c => c.Guid == guid);
                DataGemini.SUsers.Remove(sUsers);
                if (SaveData("SUser") && sUsers != null)
                {
                    DataReturn.ActiveCode = sUsers.Guid.ToString();
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
        public ActionResult Update(SUserModel viewModel)
        {
            var sUsers = new SUser();
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
                    viewModel.Legit = viewModel.Legit.GetValueOrDefault(0);
                    viewModel.LegitCount = viewModel.LegitCount.GetValueOrDefault(0);
                    if (viewModel.IsUpdate == 0)
                    {
                        viewModel.Password = Encrypt(viewModel.Password);
                        viewModel.Setvalue(sUsers);
                        DataGemini.SUsers.Add(sUsers);
                    }
                    else
                    {
                        sUsers = DataGemini.SUsers.FirstOrDefault(c => c.Guid == viewModel.Guid);
                        viewModel.Password = string.IsNullOrEmpty(viewModel.Password) ? sUsers.Password : Encrypt(viewModel.Password);
                        viewModel.Setvalue(sUsers);
                    }
                    if (SaveData("SUser") && sUsers != null)
                    {
                        DataReturn.ActiveCode = sUsers.Guid.ToString();
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
                    DataGemini.SUsers.Remove(sUsers);
                }
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        private List<string> ValidateDuplicate(SUserModel viewModel)
        {
            List<string> lstErrMsg = new List<string>();

            if (string.IsNullOrWhiteSpace(viewModel.Email))
            {
                lstErrMsg.Add("Thiếu thông tin Email");
            }

            var lstUser = DataGemini.SUsers.Where(c => (c.Username.Equals(viewModel.Username, StringComparison.OrdinalIgnoreCase)
                                                        || c.Email.Equals(viewModel.Email, StringComparison.OrdinalIgnoreCase))
                                                       && c.Guid != viewModel.Guid).ToList();

            if (lstUser.Count > 0)
            {
                lstErrMsg.Add("Tài khoản trùng Tên đăng nhập hoặc Email!");
            }

            return lstErrMsg;
        }

        public ActionResult Copy(Guid guid)
        {
            var clone = new SUser();
            var sUsers = new SUser();
            try
            {
                sUsers = DataGemini.SUsers.FirstOrDefault(c => c.Guid == guid);
                #region Copy
                DataGemini.SUsers.Add(clone);
                //Copy values from source to clone
                var sourceValues = DataGemini.Entry(sUsers).CurrentValues;
                DataGemini.Entry(clone).CurrentValues.SetValues(sourceValues);
                //Change values of the copied entity
                clone.Username = clone.Username + " - Copy";
                clone.Guid = Guid.NewGuid();
                clone.UpdatedAt = clone.CreatedAt = DateTime.Now;
                clone.UpdatedBy = clone.CreatedBy = GetUserInSession();
                if (SaveData("SUser"))
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
                DataGemini.SUsers.Remove(clone);
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(HttpPostedFileBase File_path1, string guidUser)
        {
            var msg = "";
            var nameFile = Path.GetFileName(File_path1.FileName);
            guidUser = guidUser ?? String.Empty;
            if (File_path1 != null)
            {
                string tmp = Server.MapPath("~/Content/UserFiles/Images/User/" + guidUser + "/");
                if (System.IO.File.Exists(Path.Combine(tmp, nameFile)))
                {
                    System.IO.File.Delete(Path.Combine(tmp, nameFile));
                }
                var physicalPath = Path.Combine(Server.MapPath("~/Content/UserFiles/Images/User/" + guidUser + "/"), nameFile);

                VerifyDir(physicalPath);
                WriteFileFromStream(File_path1.InputStream, physicalPath);
                msg = nameFile;

                var user = DataGemini.SUsers.FirstOrDefault(x => x.Guid.ToString().ToLower().Trim() == guidUser.ToLower().Trim());
                if (user != null)
                {
                    user.Avartar = "/Content/UserFiles/Images/User/" + guidUser + "/" + nameFile;
                    DataGemini.SaveChanges();
                }
            }
            return Json(new { status = "" + msg + "" }, "text/plain");
        }

        public static void VerifyDir(string path)
        {
            try
            {
                var list = path.Split(new string[] { "\\" }, StringSplitOptions.None);
                var directory = path.Replace("\\" + list[list.Count() - 1], "");
                DirectoryInfo dir = new DirectoryInfo(directory);
                if (!dir.Exists)
                {
                    dir.Create();
                }
            }
            catch { }
        }

        public static void WriteFileFromStream(Stream stream, string toFile)
        {
            using (FileStream fileToSave = new FileStream(toFile, FileMode.Create))
            {
                stream.CopyTo(fileToSave);
            }
        }
    }
}