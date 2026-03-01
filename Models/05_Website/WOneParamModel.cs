using System;
using System.Web;
using Gemini.Resources;
using System.ComponentModel.DataAnnotations;

namespace Gemini.Models._05_Website
{
    public class WOneParamModel
    {
        public int IsUpdate { get; set; }

        #region Properties
        [ScaffoldColumn(false)]
        public Guid Guid { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String Name { get; set; }

        [StringLength(255, ErrorMessageResourceName = "ErrorMaxLength255", ErrorMessageResourceType = typeof(Resource))]
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFill")]
        public String Code { get; set; }

        public bool Active { get; set; }

        public String Description { get; set; }

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
        public WOneParamModel()
        {
        }

        public WOneParamModel(WOneParam wOneParam)
        {
            Guid = wOneParam.Guid;
            Name = wOneParam.Name;
            Code = wOneParam.Code;
            Active = wOneParam.Active;
            CreatedAt = wOneParam.CreatedAt;
            CreatedBy = wOneParam.CreatedBy;
            UpdatedAt = wOneParam.UpdatedAt;
            UpdatedBy = wOneParam.UpdatedBy;
            Description = HttpUtility.HtmlDecode(wOneParam.Description);
        }
        #endregion

        #region Function
        public void Setvalue(WOneParam wOneParam)
        {
            if (IsUpdate == 0)
            {
                wOneParam.Guid = Guid.NewGuid();
                wOneParam.CreatedBy = CreatedBy;
                wOneParam.CreatedAt = DateTime.Now;
            }
            wOneParam.Name = Name;
            wOneParam.Code = Code;
            wOneParam.Active = Active;
            wOneParam.UpdatedAt = DateTime.Now;
            wOneParam.UpdatedBy = UpdatedBy;
            wOneParam.Description = Description;
        }
        #endregion
    }
}