using Gemini.Models._02_Cms.U;
using Gemini.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Gemini.Models._03_Pos
{
    public class PosProduceModel
    {
        public int IsUpdate { get; set; }

        #region Properties

        [ScaffoldColumn(false)]
        public Guid Guid { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String Name { get; set; }

        public bool Active { get; set; }

        public bool HotProduce { get; set; }

        public int? Legit { get; set; }

        public int? LegitCount { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String Note { get; set; }

        public String Description { get; set; }

        public Decimal? Price { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Unit { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public Guid GuidBatch { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public Guid GuidCategory { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String SeoTitle { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String SeoDescription { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String SeoFriendUrl { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String Code { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Status { get; set; }

        public String Color { get; set; }
        public String Size { get; set; }

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

        #endregion Properties

        public List<UGalleryModel> ListGallery { get; set; }

        public String LinkImg0 { get; set; }

        public String LinkImg1 { get; set; }

        public String LinkImg2 { get; set; }

        public List<SReplaceCode> ReplaceCode { get; set; }

        public String NameCategory { get; set; }

        public String NameBatch { get; set; }

        public String CategorySeoFriendUrl { get; set; }

        public int? Quantity { get; set; }

        #region Constructor

        public PosProduceModel()
        {
        }

        public PosProduceModel(PosProduce posProduce)
        {
            Guid = posProduce.Guid;
            Name = posProduce.Name;
            Active = posProduce.Active;
            HotProduce = posProduce.HotProduce;
            Note = HttpUtility.HtmlDecode(posProduce.Note);
            CreatedAt = posProduce.CreatedAt;
            CreatedBy = posProduce.CreatedBy;
            UpdatedAt = posProduce.UpdatedAt;
            UpdatedBy = posProduce.UpdatedBy;
            Description = HttpUtility.HtmlDecode(posProduce.Description);
            Price = posProduce.Price;
            Unit = posProduce.Unit;
            GuidBatch = posProduce.GuidBatch;
            GuidCategory = posProduce.GuidCategory;
            SeoTitle = posProduce.SeoTitle;
            SeoDescription = posProduce.SeoDescription;
            SeoFriendUrl = posProduce.SeoFriendUrl;
            Code = posProduce.Code;
            Status = posProduce.Status;
            Legit = posProduce.Legit;
            LegitCount = posProduce.LegitCount;
            Size = posProduce.Size;
            Color = posProduce.Color;

        }

        #endregion Constructor

        #region Function

        public void Setvalue(PosProduce posProduce, Guid? guid = null)
        {
            if (IsUpdate == 0)
            {
                if (guid == null || guid == Guid.Empty)
                {
                    posProduce.Guid = Guid.NewGuid();
                }
                else
                {
                    posProduce.Guid = guid.Value;
                }
                posProduce.CreatedBy = CreatedBy;
                posProduce.CreatedAt = DateTime.Now;
            }
            posProduce.Name = Name;
            posProduce.Active = Active;
            posProduce.HotProduce = HotProduce;
            posProduce.Note = Note;
            posProduce.UpdatedAt = DateTime.Now;
            posProduce.UpdatedBy = UpdatedBy;
            posProduce.Description = Description;
            posProduce.Price = Price;
            posProduce.Unit = Unit;
            posProduce.GuidBatch = GuidBatch;
            posProduce.GuidCategory = GuidCategory;
            posProduce.SeoTitle = SeoTitle;
            posProduce.SeoDescription = SeoDescription;
            posProduce.SeoFriendUrl = SeoFriendUrl;
            posProduce.Code = Code;
            posProduce.Status = Status;
            posProduce.Legit = Legit;
            posProduce.LegitCount = LegitCount;
            posProduce.Size = Size;
            posProduce.Color = Color;

        }

        #endregion Function
    }
}