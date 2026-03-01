using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Validation;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gemini.Models;
using Gemini.Models._01_Hethong;
using Gemini.Models._02_Cms.U;
using Gemini.Models._03_Pos;
using log4net;
using SINNOVA.Core;
using Image = System.Drawing.Image;

namespace Gemini.Controllers.Bussiness
{
    public class GeminiController : Controller
    {
        public ILog Log = LogManager.GetLogger(typeof(GeminiController));

        public class ResponseObj
        {
            public int StatusCode { get; set; }
            public String MessagError { get; set; }
            public int ActiveId { get; set; }
            public String ActiveCode { get; set; }
            public int ActiveRootId { get; set; }
            public String MessagSuccess { get; set; }
        }

        public ResponseObj DataReturn;

        public GeminiController()
        {
            DataReturn = new ResponseObj();
        }

        protected static GeminiEntities DataContext = new GeminiEntities();

        public static GeminiEntities DataGemini
        {
            get { return DataContext ?? (DataContext = new GeminiEntities()); }
        }

        public bool SaveData(string nameTable)
        {
            try
            {
                return DataGemini.SaveChanges() > 0;
            }
            catch (DbEntityValidationException dbEx)
            {

                var messageError = "";
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Log.Debug(validationError);

                        messageError = messageError + validationError.PropertyName + " " + validationError.ErrorMessage;
                    }
                }
                var argEx = new ArgumentException(messageError, dbEx);
                throw argEx;
            }
        }

        public void HandleError(Exception ex)
        {
            if (ex.InnerException != null && ex.InnerException.Message.Contains("EntityValidationErrors"))
            {
                Log.Debug(ex);
                DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.NotFound);
                DataReturn.MessagError = Constants.EntityValidationErrors;
            }
            else if (ex.InnerException != null && ex.InnerException.InnerException != null && ex.InnerException.InnerException.Message.Contains("column does not allow nulls"))
            {
                Log.Debug(ex);
                DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.NotFound);
                DataReturn.MessagError = Constants.ColumnDoesNotAllow + " " + ex.InnerException.InnerException.Message;
            }
            else if (ex.InnerException != null && ex.InnerException.InnerException != null && ex.InnerException.InnerException.Message.Contains("Cannot insert duplicate key"))
            {
                Log.Debug(ex);
                DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.NotFound);
                DataReturn.MessagError = Constants.DuplicateKey + " " + ex.InnerException.InnerException.Message;
            }
            else if (ex.InnerException != null && ex.InnerException.InnerException != null && ex.InnerException.InnerException.Message.Contains("The DELETE statement conflicted with the REFERENCE constraint"))
            {
                Log.Debug(ex);
                DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.NotFound);
                DataReturn.MessagError = Constants.ConflictedWithReference + " " + ex.InnerException.InnerException.Message;
            }
            else
            {
                Log.Debug(ex);
                DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.NotFound);
                DataReturn.MessagError = ex.Message;
            }

        }

        public void ExporteExcel(GridView gridview, string fileName)
        {
            GridView grid = gridview;
            grid.DataBind();
            System.Web.HttpContext.Current.Response.ClearContent();
            System.Web.HttpContext.Current.Response.Buffer = true;
            System.Web.HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + fileName + "");
            System.Web.HttpContext.Current.Response.ContentType = "application/ms-excel";
            System.Web.HttpContext.Current.Response.Write("<font style='font-size:10pt; font-family:Times New Roman;'>");
            System.Web.HttpContext.Current.Response.Charset = "";
            var sw = new StringWriter();
            var htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);
            System.IO.File.WriteAllText("E:\\CrmCustomer.xls", sw.ToString());
            System.Web.HttpContext.Current.Response.Output.Write(sw.ToString());
            System.Web.HttpContext.Current.Response.Flush();
            System.Web.HttpContext.Current.Response.End();

        }

        // Here I have created this for execute each time any controller (inherit this) load 
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            HttpCookie langCookie = Request.Cookies["culture"];
            string lang = langCookie != null ? langCookie.Value : SitesLanguage.GetDefaultLanguage();
            new SitesLanguage().SetLanguage(lang);

            return base.BeginExecuteCore(callback, state);
        }

        /// <summary>
        /// Split String To Arr
        /// </summary>
        /// <param name="str"></param>
        /// <param name="chartocut"></param>
        /// <returns></returns>
        public string[] SplitStringToArr(string str, char chartocut)
        {
            string[] arrs = str.Split(chartocut);
            return arrs;
        }

        public string GetUserInSession()
        {
            try
            {
                var id = (FormsIdentity)User.Identity;
                FormsAuthenticationTicket authTicket = id.Ticket;
                return authTicket.Name;
            }
            catch (Exception ex)
            {
                return "";
            }

        }

        /// <summary>
        /// GetFullNameUsers return NSD
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        protected string GetFullNameUsers(string userCode)
        {
            var userName = "";
            var erpNsd = DataGemini.SUsers.FirstOrDefault(s => s.Username == userCode);
            if (erpNsd != null)
            {
                userName = (!erpNsd.Position.IsNullOrEmpty()) ? erpNsd.Username + " (" + erpNsd.Position + ")" : erpNsd.Username;
            }

            return userName;
        }

        public List<SMenuModel> GetAllMenuInRole(Guid guidRole)
        {
            return (from frc in DataGemini.FRoleControlMenus
                    join er in DataGemini.SRoles on frc.GuidRole equals er.Guid
                    join em in DataGemini.SMenus on frc.GuidMenu equals em.Guid
                    where frc.GuidRole == guidRole && em.Type == "ADMIN"
                    select new SMenuModel()
                    {
                        Guid = em.Guid,
                        Name = em.Name,
                        Active = em.Active,
                        Note = em.Note,
                        FriendUrl = em.FriendUrl,
                        GuidLanguage = em.GuidLanguage,
                        GuidParent = em.GuidParent,
                        Icon = string.IsNullOrEmpty(em.Icon) ? "001_Help" : em.Icon,
                        LinkUrl = em.LinkUrl,
                        OrderMenu = em.OrderMenu,
                        RouterUrl = em.RouterUrl,
                        Type = em.Type,
                    }).OrderBy(s => s.OrderMenu).ToList();
        }

        public List<SControlModel> GetAllMainControlInMenu()
        {
            var route = RouteTable.Routes.GetRouteData(HttpContext);
            var linkUrl = route.GetRequiredString("Controller");
            var sMenu = DataGemini.SMenus.FirstOrDefault(x => x.LinkUrl != null && x.LinkUrl.Trim() != string.Empty && x.LinkUrl.Trim().ToLower().Contains(("/" + linkUrl).ToLower()));
            Guid? guidMenu = Guid.Empty;
            if (sMenu != null)
            {
                guidMenu = sMenu.Guid;
            }

            var models = new List<SControlModel>();
            var id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket authTicket = id.Ticket;

            var roleControls = DataGemini.SUsers.Where(s => s.Username.ToUpper().Equals(authTicket.Name.ToUpper())).Select(x => new { x.GuidRole }).FirstOrDefault();

            var lstControls = (from frc in DataGemini.FRoleControlMenus
                               join fcm in DataGemini.FControlMenus on frc.GuidMenu equals fcm.GuidMenu
                               where frc.GuidRole == roleControls.GuidRole && fcm.GuidMenu == guidMenu && fcm.GuidControl != null && fcm.GuidControl != Guid.Empty
                               select fcm.GuidControl).ToList();

            if (roleControls != null && roleControls.GuidRole != null)
            {
                models = (from frc in DataGemini.FRoleControlMenus
                          join er in DataGemini.SRoles on frc.GuidRole equals er.Guid
                          join ec in DataGemini.SControls on frc.GuidControl equals ec.Guid
                          where frc.GuidRole == roleControls.GuidRole && ec.Type == "MAIN_CONTROL" && frc.GuidControl != null && frc.GuidControl != Guid.Empty && lstControls.Contains(frc.GuidControl)
                          select new SControlModel()
                          {
                              Guid = ec.Guid,
                              Name = ec.Name,
                              SpriteCssClass = ec.SpriteCssClass,
                              EventClick = ec.EventClick,
                              OrderBy = ec.Orderby,
                              Active = ec.Active,
                          }).OrderBy(s => s.OrderBy).ToList();
            }

            return models;
        }

        public List<SControlModel> GetAllEditControlInMenu()
        {
            var route = RouteTable.Routes.GetRouteData(HttpContext);
            var linkUrl = route.GetRequiredString("Controller");
            var sMenu = DataGemini.SMenus.FirstOrDefault(x => x.LinkUrl != null && x.LinkUrl.Trim() != string.Empty && x.LinkUrl.Trim().ToLower().Contains(("/" + linkUrl).ToLower()));
            Guid? guidMenu = Guid.Empty;
            if (sMenu != null)
            {
                guidMenu = sMenu.Guid;
            }

            var models = new List<SControlModel>();
            var id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket authTicket = id.Ticket;

            var roleControls = DataGemini.SUsers.Where(s => s.Username.ToUpper().Equals(authTicket.Name.ToUpper())).Select(x => new { x.GuidRole }).FirstOrDefault();

            var lstControls = (from frc in DataGemini.FRoleControlMenus
                               join fcm in DataGemini.FControlMenus on frc.GuidMenu equals fcm.GuidMenu
                               where frc.GuidRole == roleControls.GuidRole && fcm.GuidMenu == guidMenu && fcm.GuidControl != null && fcm.GuidControl != Guid.Empty
                               select fcm.GuidControl).ToList();

            if (roleControls != null && roleControls.GuidRole != null)
            {
                models = (from frc in DataGemini.FRoleControlMenus
                          join er in DataGemini.SRoles on frc.GuidRole equals er.Guid
                          join ec in DataGemini.SControls on frc.GuidControl equals ec.Guid
                          where frc.GuidRole == roleControls.GuidRole && ec.Type == "EDIT_CONTROL" && frc.GuidControl != null && frc.GuidControl != Guid.Empty && lstControls.Contains(frc.GuidControl)
                          select new SControlModel()
                          {
                              Guid = ec.Guid,
                              Name = ec.Name,
                              SpriteCssClass = ec.SpriteCssClass,
                              EventClick = ec.EventClick,
                              OrderBy = ec.Orderby,
                              Active = ec.Active,
                          }).OrderBy(s => s.OrderBy).ToList();
            }

            return models;
        }

        public List<SControlModel> GetAllTabControlInMenu()
        {
            var route = RouteTable.Routes.GetRouteData(HttpContext);
            var linkUrl = route.GetRequiredString("Controller");
            var sMenu = DataGemini.SMenus.FirstOrDefault(x => x.LinkUrl != null && x.LinkUrl.Trim() != string.Empty && x.LinkUrl.Trim().ToLower().Contains(("/" + linkUrl).ToLower()));
            Guid? guidMenu = Guid.Empty;
            if (sMenu != null)
            {
                guidMenu = sMenu.Guid;
            }

            var models = new List<SControlModel>();
            var id = (FormsIdentity)User.Identity;
            FormsAuthenticationTicket authTicket = id.Ticket;

            var roleControls = DataGemini.SUsers.Where(s => s.Username.ToUpper().Equals(authTicket.Name.ToUpper())).Select(x => new { x.GuidRole }).FirstOrDefault();

            var lstControls = (from frc in DataGemini.FRoleControlMenus
                               join fcm in DataGemini.FControlMenus on frc.GuidMenu equals fcm.GuidMenu
                               where frc.GuidRole == roleControls.GuidRole && fcm.GuidMenu == guidMenu && fcm.GuidControl != null && fcm.GuidControl != Guid.Empty
                               select fcm.GuidControl).ToList();

            if (roleControls != null && roleControls.GuidRole != null)
            {
                models = (from frc in DataGemini.FRoleControlMenus
                          join er in DataGemini.SRoles on frc.GuidRole equals er.Guid
                          join ec in DataGemini.SControls on frc.GuidControl equals ec.Guid
                          where frc.GuidRole == roleControls.GuidRole && ec.Type == "TABS_CONTROL" && frc.GuidControl != null && frc.GuidControl != Guid.Empty && lstControls.Contains(frc.GuidControl)
                          select new SControlModel()
                          {
                              Guid = ec.Guid,
                              Name = ec.Name,
                              SpriteCssClass = ec.SpriteCssClass,
                              EventClick = ec.EventClick,
                              OrderBy = ec.Orderby,
                              Active = ec.Active,
                          }).OrderBy(s => s.OrderBy).ToList();
            }

            return models;
        }

        public SUserModel GetSettingUser()
        {
            var tmpUsername = GetUserInSession();

            var userInformation = (from ensd in DataGemini.SUsers
                                   join sr in DataGemini.SRoles on ensd.GuidRole equals sr.Guid into ps
                                   from p in ps.DefaultIfEmpty()
                                   where ensd.Username.ToUpper().Trim().Equals(tmpUsername.Trim().ToUpper())
                                   select new SUserModel()
                                   {
                                       Guid = ensd.Guid,
                                       RecordsInPage = ensd.RecordsInPage,
                                       Username = ensd.Username,
                                       Position = ensd.Position,
                                       Email = ensd.Email,
                                       Mobile = ensd.Mobile,
                                       IsAdmin = p.IsAdmin ?? false,
                                       Avartar = ensd.Avartar
                                   }).FirstOrDefault();
            if (userInformation != null)
            {
                ViewBag.Pagesize = (userInformation.RecordsInPage == null || userInformation.RecordsInPage == 0) ? 10 : userInformation.RecordsInPage;
                return userInformation;
            }
            return new SUserModel();
        }

        //public void SentEmails(CRM_CHIENDICH_EMAILModel crmChiendichEmail, ERP_NSD erpNsd, CRM_TASK crmTask,
        //    string codeEmail)
        //{
        //    #region crmChiendichEmail


        //    //if (crmChiendichEmail != null && codeEmail == "EMAIL1")
        //    //{
        //    //    var listEmail =
        //    //        DataGemini.CRM_CUSTOMER.Where(c => c.MA_NHOM_CUSTOMER == crmChiendichEmail.MaNhomCustomer).ToList();
        //    //    var emailConfig =
        //    //        DataGemini.CRM_EMAIL_CONFIG.FirstOrDefault(c => c.MA_EMAIL_CONFIG == crmChiendichEmail.MaEmailConfig);
        //    //    var emailTemplate =
        //    //        DataGemini.CRM_TEMPLATE.FirstOrDefault(c => c.MA_TEMPLATE == crmChiendichEmail.MaTemplate);

        //    //    var mail = new MailMessage();
        //    //    if (emailConfig != null)
        //    //    {
        //    //        var smtpServer = new SmtpClient(emailConfig.SMTP_EMAIL);
        //    //        mail.From = new MailAddress(emailConfig.EMAIL);
        //    //        // Chu để thư
        //    //        mail.Subject = crmChiendichEmail.ChuDe;
        //    //        mail.IsBodyHtml = emailConfig.IS_BODY_HTML;
        //    //        //Port
        //    //        if (emailConfig.PORT_EMAIL != null) smtpServer.Port = (int)emailConfig.PORT_EMAIL;
        //    //        //Tai khoan
        //    //        smtpServer.Credentials = new NetworkCredential(emailConfig.EMAIL, emailConfig.PASS_EMAIL);
        //    //        smtpServer.Timeout = 40000;
        //    //        smtpServer.EnableSsl = emailConfig.ENABLE_SSL;
        //    //        foreach (var item in listEmail)
        //    //        {

        //    //            try
        //    //            {
        //    //                mail.To.Add(item.EMAIL);
        //    //                if (emailTemplate != null)
        //    //                {
        //    //                    string htmlBody = HttpUtility.HtmlDecode(emailTemplate.TEMPLATE);
        //    //                    // Xử lý thay thế text
        //    //                    if (htmlBody.Contains("__XUNGHO__"))
        //    //                    {
        //    //                        htmlBody = htmlBody.Replace("__XUNGHO__",
        //    //                            (item.TEN_XUNGHO == null || item.TEN_XUNGHO == "none" || item.TEN_XUNGHO == "")
        //    //                                ? ""
        //    //                                : item.TEN_XUNGHO);
        //    //                    }
        //    //                    if (htmlBody.Contains("__HOVATEN__"))
        //    //                    {
        //    //                        var hoKhachHang = (item.HO_CUSTOMER == null || item.HO_CUSTOMER == "none" ||
        //    //                                           item.HO_CUSTOMER == "")
        //    //                            ? ""
        //    //                            : item.HO_CUSTOMER;
        //    //                        var tenKhachHang = (item.TEN_CUSTOMER == null || item.TEN_CUSTOMER == "none" ||
        //    //                                            item.TEN_CUSTOMER == "")
        //    //                            ? ""
        //    //                            : item.TEN_CUSTOMER;

        //    //                        htmlBody = htmlBody.Replace("__HOVATEN__", hoKhachHang
        //    //                                                                   + " " + tenKhachHang);
        //    //                    }
        //    //                    if (htmlBody.Contains("__TEN__"))
        //    //                    {
        //    //                        htmlBody = htmlBody.Replace("__TEN__",
        //    //                            (item.TEN_CUSTOMER == null || item.TEN_CUSTOMER == "none" ||
        //    //                             item.TEN_CUSTOMER == "")
        //    //                                ? ""
        //    //                                : item.TEN_CUSTOMER);
        //    //                    }
        //    //                    if (htmlBody.Contains("__EMAIL__"))
        //    //                    {
        //    //                        htmlBody = htmlBody.Replace("__EMAIL__",
        //    //                            (item.EMAIL == null || item.EMAIL == "none" || item.EMAIL == "")
        //    //                                ? ""
        //    //                                : item.EMAIL);
        //    //                    }
        //    //                    if (htmlBody.Contains("__DIENTHOAI__"))
        //    //                    {
        //    //                        htmlBody = htmlBody.Replace("__DIENTHOAI__",
        //    //                            (item.TEL == null || item.TEL == "none" || item.TEL == "") ? "" : item.TEL);
        //    //                    }
        //    //                    if (htmlBody.Contains("__DIACHI__"))
        //    //                    {
        //    //                        htmlBody = htmlBody.Replace("__DIACHI__",
        //    //                            (item.DIA_CHI == null || item.DIA_CHI == "none" || item.DIA_CHI == "")
        //    //                                ? ""
        //    //                                : item.DIA_CHI);
        //    //                    }
        //    //                    if (htmlBody.Contains("__CONGTY__"))
        //    //                    {
        //    //                        htmlBody = htmlBody.Replace("__CONGTY__",
        //    //                            (item.CONGTY == null || item.CONGTY == "none" || item.CONGTY == "")
        //    //                                ? ""
        //    //                                : item.CONGTY);
        //    //                    }
        //    //                    if (htmlBody.Contains("__EMAIL_OPEN__"))
        //    //                    {
        //    //                        htmlBody = htmlBody.Replace("__EMAIL_OPEN__",
        //    //                            ("<div style=\"height:0; width:0\"> <img src=\"__EMAI_TRACKING__\" height=\"0\" width=\"0\"> </div>"));
        //    //                        if (htmlBody.Contains("__EMAI_TRACKING__"))
        //    //                        {
        //    //                            //<img src="__EMAI_TRACKING__"  height="0" width="0">
        //    //                            string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
        //    //                            htmlBody = htmlBody.Replace("__EMAI_TRACKING__",
        //    //                                baseUrl + "/CrmEmailTracking/IsOpenEmail?maCustomer=" + item.MA_CUSTOMER +
        //    //                                "&maChiendich=" + crmChiendichEmail.MaChienDich + "&contactType=" +
        //    //                                crmChiendichEmail.Type);
        //    //                        }
        //    //                    }



        //    //                    mail.Body = htmlBody;
        //    //                }


        //    //                Log.Debug("GeminiController-crmChiendichEmail:" + item.EMAIL + " đã được gửi trong chiến dịch" + crmChiendichEmail.TenChienDich);
        //    //                smtpServer.Send(mail);
        //    //                System.Threading.Thread.Sleep((int)emailConfig.TIME_SENT_EMAIL);
        //    //            }
        //    //            catch
        //    //            {
        //    //                //TODO
        //    //            }

        //    //            mail.To.Clear();
        //    //        }
        //    //    }
        //    //}

        //    #endregion

        //    #region Active account

        //    if (erpNsd != null && codeEmail == "EMAIL2")
        //    {
        //        try
        //        {
        //            var emailConfig = DataGemini.CRM_EMAIL_CONFIG.FirstOrDefault(s => s.MA_EMAIL_CONFIG == "ECF003");
        //            var emailTemplate = DataGemini.CRM_TEMPLATE.FirstOrDefault(c => c.MA_TEMPLATE == "TE004");
        //            if (emailConfig != null && emailTemplate != null)
        //            {
        //                var mail = new MailMessage();
        //                var smtpServer = new SmtpClient(emailConfig.SMTP_EMAIL);
        //                mail.From = new MailAddress(emailConfig.EMAIL);
        //                mail.Subject = emailTemplate.TIEUDE + " - " + DateTime.Now;
        //                mail.IsBodyHtml = emailConfig.IS_BODY_HTML;
        //                //Port
        //                if (emailConfig.PORT_EMAIL != null) smtpServer.Port = (int)emailConfig.PORT_EMAIL;
        //                //Tai khoan
        //                smtpServer.Credentials = new NetworkCredential(emailConfig.EMAIL, emailConfig.PASS_EMAIL);
        //                smtpServer.EnableSsl = emailConfig.ENABLE_SSL;
        //                mail.To.Add(erpNsd.EMAIL);
        //                string htmlBody = HttpUtility.HtmlDecode(emailTemplate.TEMPLATE);
        //                if (htmlBody.Contains("__EMAIL_ACTIVE_ACCOUNT__"))
        //                {
        //                    htmlBody = htmlBody.Replace("__EMAIL_ACTIVE_ACCOUNT__",
        //                        ("<div style=\"height:0; width:0\"> <img src=\"__ACTIVE_ACCOUNT__\" height=\"0\" width=\"0\"> </div>"));
        //                    if (htmlBody.Contains("__ACTIVE_ACCOUNT__"))
        //                    {
        //                        if (Request.Url != null)
        //                        {
        //                            string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
        //                            htmlBody = htmlBody.Replace("__ACTIVE_ACCOUNT__",
        //                                baseUrl + "/Erpnsd/ActiveAccount?userName=" + erpNsd.USERNAME + "&email=" +
        //                                erpNsd.EMAIL);
        //                            Log.Debug("GeminiController-Active account: Gọi vào trang này " + baseUrl + "/Erpnsd/ActiveAccount?userName=" + erpNsd.USERNAME + "&email=" +
        //                                      erpNsd.EMAIL);
        //                        }
        //                    }
        //                }
        //                if (htmlBody.Contains("__HOVATEN__"))
        //                {
        //                    htmlBody = htmlBody.Replace("__HOVATEN__", erpNsd.TEN_NSD);
        //                }
        //                if (htmlBody.Contains("__USERNAME__"))
        //                {
        //                    htmlBody = htmlBody.Replace("__USERNAME__", erpNsd.USERNAME);
        //                }
        //                if (htmlBody.Contains("__PASSWORD__"))
        //                {
        //                    htmlBody = htmlBody.Replace("__PASSWORD__", erpNsd.GHICHU);
        //                }

        //                //
        //                try
        //                {
        //                    mail.Body = htmlBody;
        //                    smtpServer.Send(mail);
        //                    Log.Debug("GeminiController-Active account: Gửi mail thành công");
        //                }
        //                catch
        //                {
        //                    Log.Debug("GeminiController-Active account: Gửi mail lỗi");
        //                }

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Debug("GeminiController-Active account: Gửi mail lỗi " + ex.Message);

        //        }
        //    }

        //    #endregion

        //    #region Task

        //    if (crmTask != null && codeEmail == "EMAIL3")
        //    {

        //        var emailConfig = DataGemini.CRM_EMAIL_CONFIG.FirstOrDefault(s => s.MA_EMAIL_CONFIG == "ECF001");
        //        var firstOrDefault = DataGemini.ERP_NSD.FirstOrDefault(s => s.USERNAME == crmTask.MA_NGUOILAM);
        //        var nguoiCapNhat = DataGemini.ERP_NSD.FirstOrDefault(s => s.USERNAME == crmTask.MA_NGUOICAPNHAT);
        //        var nguoitao = DataGemini.ERP_NSD.FirstOrDefault(s => s.USERNAME == crmTask.MA_NGUOITAO);
        //        var emailSystemAdmins = DataGemini.ERP_NSD.Where(s => s.CAPDO == 1).ToList();


        //        var emailTemplate = new CRM_TEMPLATE();
        //        if (codeEmail == "EMAIL3")
        //        {
        //            emailTemplate = DataGemini.CRM_TEMPLATE.FirstOrDefault(c => c.MA_TEMPLATE == "TE001");
        //        }
        //        else if (codeEmail == "EMAIL4")
        //        {
        //            emailTemplate = DataGemini.CRM_TEMPLATE.FirstOrDefault(c => c.MA_TEMPLATE == "TE002");
        //        }

        //        if (firstOrDefault != null && nguoiCapNhat != null && emailConfig != null && emailTemplate != null)
        //        {
        //            var mail = new MailMessage();
        //            var smtpServer = new SmtpClient(emailConfig.SMTP_EMAIL);
        //            mail.From = new MailAddress(emailConfig.EMAIL);
        //            // Chu để thư
        //            if (codeEmail == "EMAIL3")
        //            {
        //                //Kế hoạch
        //                mail.Subject = "[CRS-NEW-TASK]Công việc Mã: " + crmTask.TASK_CODE + ", " + DateTime.Today.ToShortDateString();

        //            }
        //            else if (codeEmail == "EMAIL4")
        //            {
        //                //Hoàn thành
        //                mail.Subject = "[CRS-UPDATED-TASK]Công việc Mã: " + crmTask.TASK_CODE + ", " +
        //                               DateTime.Today.ToShortDateString();

        //            }


        //            mail.IsBodyHtml = emailConfig.IS_BODY_HTML;
        //            //Port
        //            if (emailConfig.PORT_EMAIL != null) smtpServer.Port = (int)emailConfig.PORT_EMAIL;
        //            //Tai khoan
        //            smtpServer.Credentials = new System.Net.NetworkCredential(emailConfig.EMAIL, emailConfig.PASS_EMAIL);
        //            smtpServer.EnableSsl = emailConfig.ENABLE_SSL;
        //            mail.To.Add(firstOrDefault.EMAIL);

        //            #region System Admin

        //            if (emailSystemAdmins.Any())
        //            {
        //                foreach (var item in emailSystemAdmins)
        //                {
        //                    mail.Bcc.Add(item.EMAIL);
        //                }
        //            }

        //            #endregion



        //            string htmlBody = HttpUtility.HtmlDecode(emailTemplate.TEMPLATE);
        //            if (htmlBody.Contains("__NAMETASK__"))
        //            {
        //                htmlBody = htmlBody.Replace("__NAMETASK__", crmTask.TEN_TASK);

        //            }

        //            if (htmlBody.Contains("__TASKCODE__"))
        //            {
        //                htmlBody = htmlBody.Replace("__TASKCODE__",
        //                    (crmTask.TASK_CODE == null || crmTask.TASK_CODE == "none" || crmTask.TASK_CODE == "")
        //                        ? ""
        //                        : crmTask.TASK_CODE);
        //            }


        //            if (htmlBody.Contains("__NGUOITAOTASK__"))
        //            {
        //                htmlBody = htmlBody.Replace("__NGUOITAOTASK__",
        //                    (crmTask.MA_NGUOITAO == null || crmTask.MA_NGUOITAO == "none" || crmTask.MA_NGUOITAO == "")
        //                        ? ""
        //                        : nguoitao.TEN_NSD);
        //            }

        //            if (htmlBody.Contains("__NGUOICAPNHATTASK__"))
        //            {
        //                htmlBody = htmlBody.Replace("__NGUOICAPNHATTASK__",
        //                    (crmTask.MA_NGUOICAPNHAT == null || crmTask.MA_NGUOICAPNHAT == "none" ||
        //                     crmTask.MA_NGUOICAPNHAT == "")
        //                        ? ""
        //                        : nguoiCapNhat.TEN_NSD);
        //            }


        //            if (htmlBody.Contains("__NTN_TAO_TASK__"))
        //            {
        //                htmlBody = crmTask.NTN_TAO != null
        //                    ? htmlBody.Replace("__NTN_TAO_TASK__",
        //                        crmTask.NTN_TAO.ToShortDateString() + " " + crmTask.NTN_TAO.ToShortTimeString())
        //                    : htmlBody.Replace("__NTN_TAO_TASK__", "");

        //            }
        //            if (htmlBody.Contains("__NTNCAPNHAT__"))
        //            {
        //                htmlBody = crmTask.NTN_CAPNHAT != null
        //                    ? htmlBody.Replace("__NTNCAPNHAT__", crmTask.NTN_CAPNHAT.ToShortDateString())
        //                    : htmlBody.Replace("__NTNCAPNHAT__", "");
        //            }

        //            if (htmlBody.Contains("__ESTIMATED_TIME__"))
        //            {
        //                htmlBody = htmlBody.Replace("__ESTIMATED_TIME__", crmTask.EstimatedTime + " ");
        //            }

        //            if (htmlBody.Contains("__CHITIETTASK__"))
        //            {
        //                htmlBody = htmlBody.Replace("__CHITIETTASK__",
        //                    (crmTask.CHI_TIET_TASK == null || crmTask.CHI_TIET_TASK == "none" ||
        //                     crmTask.CHI_TIET_TASK == "")
        //                        ? ""
        //                        : HttpUtility.HtmlDecode(crmTask.CHI_TIET_TASK));
        //            }
        //            if (htmlBody.Contains("__NSTRIENKHAITASK__"))
        //            {
        //                htmlBody = htmlBody.Replace("__NSTRIENKHAITASK__",
        //                    (crmTask.MA_NGUOILAM == null || crmTask.MA_NGUOILAM == "none" || crmTask.MA_NGUOILAM == "")
        //                        ? ""
        //                        : firstOrDefault.TEN_NSD);
        //            }

        //            if (htmlBody.Contains("__YEUCAUTASK__"))
        //            {
        //                htmlBody = htmlBody.Replace("__YEUCAUTASK__",
        //                    (crmTask.YEUCAU == null || crmTask.YEUCAU == "none" || crmTask.YEUCAU == "")
        //                        ? ""
        //                        : HttpUtility.HtmlDecode(crmTask.YEUCAU));
        //            }
        //            if (htmlBody.Contains("__TRANG_THAI_TASK__"))
        //            {
        //                var tenTrangthai = DataGemini.DM_TRANGTHAI.FirstOrDefault(s => s.TRANGTHAI_ID == crmTask.Status);
        //                if (tenTrangthai != null)
        //                {
        //                    htmlBody = htmlBody.Replace("__TRANG_THAI_TASK__", tenTrangthai.TEN_TRANGTHAI);
        //                }
        //            }

        //            if (htmlBody.Contains("__REMAINING_TASK__"))
        //            {
        //                htmlBody = htmlBody.Replace("__REMAINING_TASK__", crmTask.Remaining.ToString());
        //            }

        //            if (htmlBody.Contains("__DO_UU_TIEN_TASK__"))
        //            {
        //                var tenTrangthai = DataGemini.DM_TRANGTHAI.FirstOrDefault(s => s.TRANGTHAI_ID == crmTask.DO_UU_TIEN);
        //                if (tenTrangthai != null)
        //                {
        //                    htmlBody = htmlBody.Replace("__DO_UU_TIEN_TASK__", tenTrangthai.TEN_TRANGTHAI);
        //                }

        //            }

        //            try
        //            {
        //                mail.Body = htmlBody;
        //                smtpServer.Send(mail);
        //            }
        //            catch
        //            {
        //                // ex.FailedRecipient and ex.GetBaseException() should give you enough info.
        //            }
        //        }

        //    #endregion
        //    }
        //}

        public static string Key = "CoreworkByDuong2022";

        public static string Encrypt(string clearText)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public static string Decrypt(string cipherText)
        {
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(Key, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        #region Remove Unicode

        private static readonly string[] VietnameseSigns = new string[]
        {

            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"
        };

        public static string RemoveSign4VietnameseString(string str)
        {
            if (str.Contains(","))
            {
                str = str.Replace(" , ", " ");
                str = str.Replace(", ", " ");
                str = str.Replace(" ,", " ");
                str = str.Replace(",", "");
            }
            if (str.Contains("-"))
            {
                str = str.Replace(" - ", " ");
                str = str.Replace("- ", " ");
                str = str.Replace(" -", " ");
                str = str.Replace("-", " ");
            }
            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }
            str = str.Replace(" ", "-");
            str = str.ToLower();
            return str;
        }

        #endregion

        public static Bitmap ScaleImage(string url, int width, int height)
        {
            WebClient wc = new WebClient();
            byte[] originalData = wc.DownloadData(url);
            MemoryStream stream = new MemoryStream(originalData);
            Bitmap pic = new Bitmap(stream);
            try
            {
                Bitmap result = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(result))
                {
                    g.InterpolationMode = InterpolationMode.NearestNeighbor;
                    g.DrawImage(pic, 0, 0, width, height);
                }

                return result;
            }
            catch
            {
                return pic;
            }
        }

        public string GetImage(string sizeImage, UGalleryModel UGalleryModel)
        {
            if (UGalleryModel == null || String.IsNullOrEmpty(UGalleryModel.Image))
            {
                return DefaultImage.ImageEmpty;
            }
            else
            {
                var path = Path.GetDirectoryName(UGalleryModel.Image.Replace(ConstantsImage.Slash, "/")) + "\\" + UGalleryModel.Guid.ToString() + "-" + sizeImage + ConstantsImage.FormatJpgImage;
                if (path.Contains(sizeImage))
                {
                    path = path.Replace("Images\\san-pham", "Thumbnail");
                }
                FileInfo fileImages = new FileInfo(Server.MapPath(path));
                if (fileImages.Exists)
                {
                    return path;
                }
            }

            return DefaultImage.ImageEmpty;
        }

        public void SendEmails(string codeEmail, object data)
        {
            var sUser = new SUser();
            switch (codeEmail)
            {
                case CrmEmailTemplate_Code.ActiveAccount:
                    sUser = data as SUser;

                    try
                    {
                        var emailConfig = DataGemini.CrmEmailSettings.FirstOrDefault(s => s.Active);
                        var emailTemplate = DataGemini.CrmEmailTemplates.FirstOrDefault(c => c.Code == CrmEmailTemplate_Code.ActiveAccount);
                        if (emailConfig != null && emailTemplate != null)
                        {
                            var mail = new MailMessage();
                            var smtpServer = new SmtpClient(emailConfig.Smtp);
                            mail.From = new MailAddress(emailConfig.Email);
                            mail.Subject = "Welcome" + " - " + DateTime.Now;
                            mail.IsBodyHtml = emailConfig.IsHtml;
                            //Port
                            if (emailConfig.Port != null) smtpServer.Port = (int)emailConfig.Port;
                            //Tai khoan
                            smtpServer.UseDefaultCredentials = false;
                            smtpServer.Credentials = new NetworkCredential(emailConfig.Email, emailConfig.PassEmail);
                            smtpServer.EnableSsl = emailConfig.EnableSsl;
                            mail.To.Add(sUser.Email);
                            string htmlBody = HttpUtility.HtmlDecode(emailTemplate.ContentTemplate);
                            if (htmlBody.Contains(CrmEmailTemplate_ReplaceCode.LinkActiveAccount))
                            {
                                if (Request.Url != null)
                                {
                                    string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                                    string token = string.Format("{0}|{1}|{2}", sUser.Username, sUser.Email, DateTime.Now.AddMinutes(15)); //Username|Email|ExpiryDate
                                    htmlBody = htmlBody.Replace(CrmEmailTemplate_ReplaceCode.LinkActiveAccount, baseUrl + "/admin/Admin/ActiveAccount?token=" + Encrypt(token));
                                }
                            }
                            if (htmlBody.Contains(CrmEmailTemplate_ReplaceCode.Username))
                            {
                                htmlBody = htmlBody.Replace(CrmEmailTemplate_ReplaceCode.Username, sUser.Username);
                            }
                            try
                            {
                                mail.Body = htmlBody;
                                smtpServer.Send(mail);
                                Log.Debug("Send email Success");
                            }
                            catch
                            {
                                Log.Debug("Send email Fail");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("Send email Fail: " + ex.Message);
                    }

                    break;
                case CrmEmailTemplate_Code.ForgotPassword:
                    sUser = data as SUser;

                    try
                    {
                        var emailConfig = DataGemini.CrmEmailSettings.FirstOrDefault(s => s.Active);
                        var emailTemplate = DataGemini.CrmEmailTemplates.FirstOrDefault(c => c.Code == CrmEmailTemplate_Code.ForgotPassword);
                        if (emailConfig != null && emailTemplate != null)
                        {
                            var mail = new MailMessage();
                            var smtpServer = new SmtpClient(emailConfig.Smtp);
                            mail.From = new MailAddress(emailConfig.Email);
                            mail.Subject = "Welcome" + " - " + DateTime.Now;
                            mail.IsBodyHtml = emailConfig.IsHtml;
                            //Port
                            if (emailConfig.Port != null) smtpServer.Port = (int)emailConfig.Port;
                            //Tai khoan
                            smtpServer.UseDefaultCredentials = false;
                            smtpServer.Credentials = new NetworkCredential(emailConfig.Email, emailConfig.PassEmail);
                            smtpServer.EnableSsl = emailConfig.EnableSsl;
                            mail.To.Add(sUser.Email);
                            string htmlBody = HttpUtility.HtmlDecode(emailTemplate.ContentTemplate);
                            if (htmlBody.Contains(CrmEmailTemplate_ReplaceCode.PasswordReseted))
                            {
                                if (Request.Url != null)
                                {
                                    string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                                    htmlBody = htmlBody.Replace(CrmEmailTemplate_ReplaceCode.PasswordReseted, Decrypt(sUser.Password));
                                }
                            }
                            if (htmlBody.Contains(CrmEmailTemplate_ReplaceCode.Username))
                            {
                                htmlBody = htmlBody.Replace(CrmEmailTemplate_ReplaceCode.Username, sUser.Username);
                            }
                            try
                            {
                                mail.Body = htmlBody;
                                smtpServer.Send(mail);
                                Log.Debug("Send email Success");
                            }
                            catch
                            {
                                Log.Debug("Send email Fail");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Debug("Send email Fail: " + ex.Message);
                    }

                    break;
            }
        }

        public static IList<PosCategoryModel> BuildTreeCategory(IEnumerable<PosCategoryModel> source)
        {
            IList<PosCategoryModel> roots = new BindingList<PosCategoryModel>();
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
                        AddChildrenCategory(t, dict, ref order);
                    }
                }
            }

            return roots;
        }

        private static void AddChildrenCategory(PosCategoryModel node, IDictionary<Guid, List<PosCategoryModel>> source, ref int order)
        {
            if (source.ContainsKey(node.Guid))
            {
                node.Items = source[node.Guid];
                foreach (PosCategoryModel t in node.Items)
                {
                    order++;
                    t.RootId = order;
                    AddChildrenCategory(t, source, ref order);
                }
            }
            else
            {
                node.Items = new List<PosCategoryModel>();
            }
        }

        public static void AppendCharsCategory(PosCategoryModel posCategory, string append = "", string charToAppend = ">> ")
        {
            if (posCategory.Items != null && posCategory.Items.Any())
            {
                append += charToAppend;
                foreach (var item in posCategory.Items)
                {
                    item.Name = string.Format("{0} {1}", append, item.Name);
                    AppendCharsCategory(item, append, charToAppend);
                }
            }
        }
    }
}