using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Gemini.Resources;
using SINNOVA.Core;

namespace Gemini.Models._02_Cms.U
{
    public class UGalleryModel
    {
        public int IsUpdate { get; set; }

        #region Properties
        [ScaffoldColumn(false)]
        public Guid Guid { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFill")]
        public String Name { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String Description { get; set; }

        public Guid? GuidGroup { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String Link { get; set; }

        [AllowHtml]
        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String Image { get; set; }

        public bool Active { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String Note { get; set; }

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

        public bool IsProduce { get; set; }

        [AllowHtml]
        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String UrlAnh { get { return vString.FindString(vString.GetValueTostring(Image), "src=\"", "\""); } }

        #region Constructor
        public UGalleryModel()
        {
        }

        public UGalleryModel(UGallery uGallery)
        {
            Guid = uGallery.Guid;
            Name = uGallery.Name;
            Description = HttpUtility.HtmlDecode(uGallery.Description);
            GuidGroup = uGallery.GuidGroup;
            Link = uGallery.Link;
            Image = string.IsNullOrEmpty(uGallery.Image) ? "" : "<img  src=\"" + HttpUtility.UrlDecode(uGallery.Image) + "\"  />";
            Active = uGallery.Active;
            Note = uGallery.Note;
            CreatedAt = uGallery.CreatedAt;
            CreatedBy = uGallery.CreatedBy;
            UpdatedAt = uGallery.UpdatedAt;
            UpdatedBy = uGallery.UpdatedBy;
        }
        #endregion

        #region Function
        public void Setvalue(UGallery uGallery)
        {
            if (IsUpdate == 0)
            {
                uGallery.Guid = Guid.NewGuid();
                uGallery.CreatedBy = CreatedBy;
                uGallery.CreatedAt = DateTime.Now;
            }
            uGallery.GuidGroup = GuidGroup;
            uGallery.Name = Name;
            uGallery.Description = Description;
            uGallery.Link = Link;
            uGallery.Image = WebUtility.HtmlDecode(UrlAnh).Replace("%2F", "/").Replace("%20", " ");
            uGallery.Active = Active;
            uGallery.Note = Note;
            uGallery.UpdatedAt = DateTime.Now;
            uGallery.UpdatedBy = UpdatedBy;
        }
        #endregion
    }
}