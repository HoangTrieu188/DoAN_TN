using System.Collections.Generic;

namespace Gemini.Models._05_Website
{
    public class WLiveChatModel
    {
        public SUser CurrentUser { get; set; }
    }

    public class PartialUnreadMessageModel
    {
        public List<WLiveChat> ListUnreadMessage { get; set; }
    }

    public class PartialFollowUserModel
    {
        public List<SUser> ListUserFollow { get; set; }
    }

    public class PartialOnlineUserModel
    {
        public List<SUser> ListUserOnline { get; set; }
    }

    public class PartialOfflineUserModel
    {
        public List<SUser> ListUserOffline { get; set; }
    }

    public class PartialChatPannelModel
    {
        public string MsgSender { get; set; }

        public string CurrentUsername { get; set; }

        public List<WLiveChat> ListLiveChat { get; set; }
    }
}