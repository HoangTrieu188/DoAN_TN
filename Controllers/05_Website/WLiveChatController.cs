using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Gemini.Controllers.Bussiness;
using Gemini.Models;
using Gemini.Models._05_Website;
using Gemini.Models._20_Web;
using Kendo.Mvc.Extensions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace Gemini.Controllers._05_Website
{
    public class WLiveChatController : GeminiController
    {
        #region Main

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            var models = new WLiveChatModel();

            var username = GetUserInSession();
            models.CurrentUser = DataGemini.SUsers.FirstOrDefault(x => x.Username == username);

            return View(models);
        }

        #endregion

        public ActionResult PartialUnreadMessage()
        {
            var model = new PartialUnreadMessageModel();

            var username = GetUserInSession();
            model.ListUnreadMessage = DataGemini.WLiveChats.Where(x => x.RecevierSeen == 0 && x.MsgReceiver == username).ToList();

            return PartialView(model);
        }

        public ActionResult PartialFollowUser()
        {
            var model = new PartialFollowUserModel();

            var user = GetSettingUser();
            model.ListUserFollow = DataGemini.SUsers.Where(x => x.GuidFollow.Contains(user.Guid.ToString().ToLower().Trim()) && x.Username != user.Username).ToList();

            return PartialView(model);
        }

        public ActionResult PartialOnlineUser()
        {
            var model = new PartialOnlineUserModel();

            var username = GetUserInSession();
            model.ListUserOnline = DataGemini.SUsers.Where(x => x.OnlineStatus == SUser_OnlineStatus.Online && x.Username != username).ToList();

            return PartialView(model);
        }

        public ActionResult PartialOfflineUser()
        {
            var model = new PartialOfflineUserModel();

            var username = GetUserInSession();
            model.ListUserOffline = DataGemini.SUsers.Where(x => (x.OnlineStatus == SUser_OnlineStatus.Offline || x.OnlineStatus == null) && x.Username != username).ToList();

            return PartialView(model);
        }

        public ActionResult PartialChatPannel(string msgSender)
        {
            var model = new PartialChatPannelModel();

            model.CurrentUsername = GetUserInSession();

            model.MsgSender = msgSender ?? "...";

            if (!string.IsNullOrEmpty(msgSender))
            {
                model.ListLiveChat = DataGemini.WLiveChats.Where(x => (x.MsgSender == msgSender && x.MsgReceiver == model.CurrentUsername) || (x.MsgSender == model.CurrentUsername && x.MsgReceiver == msgSender)).OrderBy(x => x.SendAt).ToList();
            }

            SeenAllMsg(model.CurrentUsername, msgSender);

            return PartialView(model);
        }

        private void SeenAllMsg(string receiver, string sender)
        {
            if (!string.IsNullOrEmpty(receiver) && !string.IsNullOrEmpty(sender))
            {
                var wLiveChat = DataGemini.WLiveChats.FirstOrDefault(x => x.MsgReceiver == receiver && x.MsgSender == sender && x.RecevierSeen == 0);

                if (wLiveChat != null)
                {
                    wLiveChat.RecevierSeen = 1;
                    DataGemini.SaveChanges();
                }
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SendMsg(string msgSender, string chatMsg)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(msgSender) || msgSender == "...")
                {
                    return Json(new { errMsg = "Chọn người nhận" });
                }
                if (string.IsNullOrWhiteSpace(chatMsg))
                {
                    return Json(new { errMsg = "Gửi tin nhắn" });
                }

                var wLiveChat = new WLiveChat()
                {
                    Guid = Guid.NewGuid(),
                    ChatMsg = chatMsg,
                    MsgSender = GetUserInSession(),
                    MsgReceiver = msgSender,
                    SendAt = DateTime.Now,
                };
                DataGemini.WLiveChats.Add(wLiveChat);

                if (SaveData("WLiveChat"))
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                }
                else
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                }

                return Json(DataReturn, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { errMsg = ex.Message });
            }
        }
    }
}