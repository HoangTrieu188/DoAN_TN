using Gemini.Controllers.Bussiness;
using Gemini.Models;
using Gemini.Models._02_Cms.U;
using Gemini.Models._05_Website;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Net;

namespace Gemini.Controllers._05_Website
{
    [CustomAuthorizeAttribute]
    public class WOrderController : GeminiController
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
            List<WOrderModel> wOrders = (from wo in DataGemini.WOrders
                                         join su in DataGemini.SUsers on wo.GuidUser equals su.Guid
                                         //where wo.Status >= WOrder_Status.Paid
                                         select new WOrderModel
                                         {
                                             Guid = wo.Guid,
                                             OrderCode = wo.Guid.ToString(),
                                             Username = su.Username,
                                             Status = wo.Status,
                                             Mobile = wo.Mobile,
                                             FullAddress = wo.FullAddress,
                                             CreatedAt = wo.CreatedAt
                                         }).ToList();

            foreach (var item in wOrders)
            {
                item.StatusName = WOrder_Status.dicDesc[item.Status.GetValueOrDefault(WOrder_Status.Inprogress)];
            }

            DataSourceResult result = wOrders.OrderByDescending(x => x.CreatedAt).ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult ReadTabc1([DataSourceRequest] DataSourceRequest request, string guid)
        {
            List<WOrderDetailModel> wOrderDetails = (from wod in DataGemini.WOrderDetails
                                                     join pp in DataGemini.PosProduces on wod.GuidProduce equals pp.Guid
                                                     where wod.GuidOrder != null && wod.GuidOrder.ToString().ToLower() == guid
                                                     select new WOrderDetailModel
                                                     {
                                                         ProduceCode = pp.Code,
                                                         ProduceName = pp.Name,
                                                         Quantity = wod.Quantity,
                                                         Price = wod.Price,
                                                         Size = wod.Size,
                                                         Color = wod.Color,
                                                         ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                        join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                        where fr.GuidProduce == pp.Guid
                                                                        select new UGalleryModel
                                                                        {
                                                                            Image = im.Image,
                                                                            CreatedAt = im.CreatedAt
                                                                        }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                     }).ToList();

            foreach (var item in wOrderDetails)
            {
                var tmpLinkImg = item.ListGallery;
                if (tmpLinkImg.Count == 0)
                {
                    item.ProduceLinkImg0 = "/Content/Custom/empty-album.png";
                }
                else
                {
                    item.ProduceLinkImg0 = tmpLinkImg[0].Image;
                }
            }

            DataSourceResult result = wOrderDetails.ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult ConfirmOrder(Guid guid)
        {
            var wOrder = new WOrder();
            try
            {
                wOrder = DataGemini.WOrders.FirstOrDefault(c => c.Guid == guid);
                var lstErrMsg = Validate_Approval(wOrder);

                if (lstErrMsg.Count > 0)
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.Conflict);
                    DataReturn.MessagError = String.Join("<br/>", lstErrMsg);
                }
                else
                {
                    wOrder.Status = WOrder_Status.Confirm;
                    wOrder.UpdatedAt = DateTime.Now;
                    wOrder.UpdatedBy = GetUserInSession();

                    // Deduct inventory quantities by Size and Color
                    var orderDetails = DataGemini.WOrderDetails.Where(x => x.GuidOrder == guid).ToList();
                    foreach (var detail in orderDetails)
                    {
                        // Find inventory record matching Product + Size + Color
                        var inventory = DataGemini.PosInventories.FirstOrDefault(x => 
                            x.GuidProduce == detail.GuidProduce && 
                            x.Size == detail.Size && 
                            x.Color == detail.Color);
                        
                        if (inventory != null)
                        {
                            // Deduct quantity
                            int currentQty = inventory.Quantity;
                            int orderQty = detail.Quantity ?? 0;
                            inventory.Quantity = currentQty - orderQty;
                            inventory.UpdatedAt = DateTime.Now;
                            inventory.UpdatedBy = GetUserInSession();
                            
                            // If quantity becomes 0 or negative, set to 0
                            if (inventory.Quantity < 0)
                            {
                                inventory.Quantity = 0;
                            }
                        }
                        else
                        {
                            // If inventory record doesn't exist, create one with 0 quantity
                            // This shouldn't happen in normal flow, but handle it gracefully
                            var newInventory = new PosInventory
                            {
                                Guid = Guid.NewGuid(),
                                GuidProduce = detail.GuidProduce.GetValueOrDefault(),
                                Size = detail.Size,
                                Color = detail.Color,
                                Quantity = 0,
                                CreatedAt = DateTime.Now,
                                CreatedBy = GetUserInSession(),
                                UpdatedAt = DateTime.Now,
                                UpdatedBy = GetUserInSession()
                            };
                            DataGemini.PosInventories.Add(newInventory);
                        }
                        
                        // Check if all variants of this product are out of stock
                        var product = DataGemini.PosProduces.FirstOrDefault(x => x.Guid == detail.GuidProduce);
                        if (product != null)
                        {
                            var allInventories = DataGemini.PosInventories.Where(x => x.GuidProduce == detail.GuidProduce).ToList();
                            bool allOutOfStock = allInventories.All(inv => inv.Quantity <= 0);
                            
                            if (allOutOfStock && allInventories.Any())
                            {
                                product.Status = "Hết hàng";
                            }
                        }
                    }

                    if (SaveData("WOrder") && wOrder != null)
                    {
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
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RejectOrder(Guid guid, string reason = "")
        {
            var wOrder = new WOrder();
            try
            {
                wOrder = DataGemini.WOrders.FirstOrDefault(c => c.Guid == guid);
                var lstErrMsg = Validate_Approval(wOrder);

                if (lstErrMsg.Count > 0)
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.Conflict);
                    DataReturn.MessagError = String.Join("<br/>", lstErrMsg);
                }
                else
                {
                    wOrder.Status = WOrder_Status.Reject;
                    wOrder.UpdatedAt = DateTime.Now;
                    wOrder.UpdatedBy = GetUserInSession();
                    if (!string.IsNullOrEmpty(reason))
                    {
                        // wOrder.Note field does not exist. 
                        // Only using reason for email sending.
                    }

                    if (SaveData("WOrder") && wOrder != null)
                    {
                        // Send Email
                        if (!string.IsNullOrEmpty(reason))
                        {
                            SendEmailReject(wOrder, reason);
                        }

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
                HandleError(ex);
            }

            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        private void SendEmailReject(WOrder wOrder, string reason)
        {
            try
            {
                var user = DataGemini.SUsers.FirstOrDefault(x => x.Guid == wOrder.GuidUser);
                if (user == null || string.IsNullOrEmpty(user.Email)) return;

                var emailConfig = DataGemini.CrmEmailSettings.FirstOrDefault(x => x.Active);
                if (emailConfig == null) return;

                var mail = new MailMessage();
                mail.From = new MailAddress(emailConfig.Email);
                mail.To.Add(user.Email);
                mail.Subject = "Thông báo hủy đơn hàng #" + wOrder.Guid.ToString().Substring(0, 8).ToUpper();
                mail.IsBodyHtml = true;
                mail.Body = $@"
                    <h3>Xin chào {user.Username},</h3>
                    <p>Đơn hàng của bạn (Mã: <b>{wOrder.Guid.ToString().Substring(0, 8).ToUpper()}</b>) đã bị hủy.</p>
                    <p><b>Lý do hủy:</b> {reason}</p>
                    <p>Vui lòng liên hệ bộ phận CSKH nếu có thắc mắc.</p>
                    <p>Trân trọng,</p>
                ";

                var smtpServer = new SmtpClient(emailConfig.Smtp);
                smtpServer.Port = emailConfig.Port;
                smtpServer.Credentials = new NetworkCredential(emailConfig.Email, emailConfig.PassEmail);
                smtpServer.EnableSsl = emailConfig.EnableSsl;
                
                smtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                Log.Error("SendEmailReject Error", ex);
            }
        }

        private List<string> Validate_Approval(WOrder wOrder)
        {
            List<string> lstErrMsg = new List<string>();

            //if (wOrder.Status < WOrder_Status.Paid)
            //{
            //    lstErrMsg.Add("Đơn hàng chưa thanh toán, không thể xác nhận/từ chối!");
            //}

            if (wOrder.Status >= WOrder_Status.Confirm)
            {
                lstErrMsg.Add("Đơn hàng đã qua bước xác nhận, không thể sửa!");
            }

            return lstErrMsg;
        }
    }
}