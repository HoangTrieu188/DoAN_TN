using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Gemini.Models._20_Web;
using Gemini.Resources;
using SINNOVA.Core;

namespace Gemini.Models._01_Hethong
{
    public class SMenuModel
    {
        public int IsUpdate { get; set; }

        #region Properties
        [ScaffoldColumn(false)]
        public Guid Guid { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String Name { get; set; }

        public bool Active { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String Note { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String FriendUrl { get; set; }

        public Guid? GuidLanguage { get; set; }

        public Guid? GuidParent { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Icon { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String LinkUrl { get; set; }

        public int? OrderMenu { get; set; }

        public int? RootId { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String RouterUrl { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Type { get; set; }

        public bool IsRoleMenu { get; set; }

        [Editable(false)]
        public DateTime? CreatedAt { get; set; }

        [Editable(false)]
        [StringLength(25, ErrorMessageResourceName = "ErrorMaxLength25", ErrorMessageResourceType = typeof(Resource))]
        public String CreatedBy { get; set; }

        [Editable(false)]
        public DateTime? UpdatedAt { get; set; }

        [Editable(false)]
        [StringLength(25, ErrorMessageResourceName = "ErrorMaxLength25", ErrorMessageResourceType = typeof(Resource))]
        public String UpdatedBy { get; set; }
        #endregion

        public List<SMenuModel> Items { get; set; }

        #region Constructor
        public SMenuModel()
        {
        }

        public SMenuModel(SMenu sMenu)
        {
            Guid = sMenu.Guid;
            Name = sMenu.Name;
            Active = sMenu.Active;
            Note = sMenu.Note;
            FriendUrl = sMenu.FriendUrl;
            GuidLanguage = sMenu.GuidLanguage;
            GuidParent = sMenu.GuidParent;
            Icon = sMenu.Icon;
            LinkUrl = sMenu.LinkUrl;
            OrderMenu = sMenu.OrderMenu;
            RouterUrl = sMenu.RouterUrl;
            Type = sMenu.Type;
            CreatedAt = sMenu.CreatedAt;
            CreatedBy = vString.GetValueTostring(sMenu.CreatedBy);
            UpdatedAt = sMenu.UpdatedAt;
            UpdatedBy = vString.GetValueTostring(sMenu.UpdatedBy);
        }
        #endregion

        #region Function
        public void Setvalue(SMenu sMenu)
        {
            if (IsUpdate == 0)
            {
                sMenu.Guid = Guid.NewGuid();
                sMenu.CreatedBy = CreatedBy;
                sMenu.CreatedAt = DateTime.Now;
            }
            sMenu.Name = Name;
            sMenu.Active = Active;
            sMenu.Note = Note;
            sMenu.UpdatedAt = DateTime.Now;
            sMenu.UpdatedBy = UpdatedBy;
            sMenu.FriendUrl = FriendUrl;
            sMenu.GuidLanguage = GuidLanguage;
            sMenu.GuidParent = GuidParent;
            sMenu.LinkUrl = LinkUrl;
            sMenu.OrderMenu = OrderMenu;
            sMenu.RouterUrl = RouterUrl;
            sMenu.Type = Type;
        }
        #endregion
    }
}