using System;
using System.ComponentModel.DataAnnotations;
using Gemini.Resources;

namespace Gemini.Models._05_Website
{
    public class WRatingProduceModel
    {
        public int IsUpdate { get; set; }

        #region Properties
        [ScaffoldColumn(false)]
        public Guid Guid { get; set; }

        public Guid? GuidProduce{ get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFill")]
        public String FullName { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Mobile { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFill")]
        public String Email { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "ErrorMaxLength2000", ErrorMessageResourceType = typeof(Resource))]
        public String Comment { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        public String Avatar { get; set; }

        public int? Legit { get; set; }

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

        #region Constructor
        public WRatingProduceModel()
        {
        }

        public WRatingProduceModel(WRatingProduce wRatingProduce)
        {
            Guid = wRatingProduce.Guid;
            GuidProduce = wRatingProduce.GuidProduce;
            FullName = wRatingProduce.FullName;
            Mobile = wRatingProduce.Mobile;
            Email = wRatingProduce.Email;
            Comment = wRatingProduce.Comment;
            Legit = wRatingProduce.Legit;
            Avatar = wRatingProduce.Avatar;
            CreatedAt = wRatingProduce.CreatedAt;
            CreatedBy = wRatingProduce.CreatedBy;
            UpdatedAt = wRatingProduce.UpdatedAt;
            UpdatedBy = wRatingProduce.UpdatedBy;
        }
        #endregion

        #region Function
        public void Setvalue(WRatingProduce posPartner)
        {
            if (IsUpdate == 0)
            {
                posPartner.Guid = Guid.NewGuid();
                posPartner.CreatedBy = CreatedBy;
                posPartner.CreatedAt = DateTime.Now;
            }
            posPartner.GuidProduce = GuidProduce;
            posPartner.FullName = FullName;
            posPartner.Mobile = Mobile;
            posPartner.Email = Email;
            posPartner.Comment = Comment;
            posPartner.Legit = Legit;
            posPartner.Avatar = Avatar;
            posPartner.UpdatedAt = DateTime.Now;
            posPartner.UpdatedBy = UpdatedBy;
        }
        #endregion
    }
}