using System;
using System.Web;
using Gemini.Resources;
using System.ComponentModel.DataAnnotations;

namespace Gemini.Models._05_Website
{
    public class WOrderModel
    {
        public int IsUpdate { get; set; }

        #region Properties
        [ScaffoldColumn(false)]
        public Guid Guid { get; set; }

        public Guid? GuidUser { get; set; }

        public String FullAddress { get; set; }

        public String Mobile { get; set; }

        public long? PaymentTranId { get; set; }

        public int? Status { get; set; }

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

        public string OrderCode { get; set; }

        public string Username { get; set; }

        public string StatusName { get; set; }

        #region Constructor
        public WOrderModel()
        {
        }

        public WOrderModel(WOrder wOrder)
        {
            Guid = wOrder.Guid;
            GuidUser = wOrder.GuidUser;
            FullAddress = wOrder.FullAddress;
            Mobile = wOrder.Mobile;
            PaymentTranId = wOrder.PaymentTranId;
            Status = wOrder.Status;
            CreatedAt = wOrder.CreatedAt;
            CreatedBy = wOrder.CreatedBy;
            UpdatedAt = wOrder.UpdatedAt;
            UpdatedBy = wOrder.UpdatedBy;
        }
        #endregion

        #region Function
        public void Setvalue(WOrder wOrder)
        {
            if (IsUpdate == 0)
            {
                wOrder.Guid = Guid.NewGuid();
                wOrder.CreatedBy = CreatedBy;
                wOrder.CreatedAt = DateTime.Now;
            }
            wOrder.GuidUser = GuidUser;
            wOrder.FullAddress = FullAddress;
            wOrder.Mobile = Mobile;
            wOrder.PaymentTranId = PaymentTranId;
            wOrder.Status = Status;
            wOrder.UpdatedAt = UpdatedAt;
            wOrder.UpdatedBy = UpdatedBy;
        }
        #endregion
    }
}