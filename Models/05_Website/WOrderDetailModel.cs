using Gemini.Models._02_Cms.U;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gemini.Models._05_Website
{
    public class WOrderDetailModel
    {
        public int IsUpdate { get; set; }

        #region Properties

        [ScaffoldColumn(false)]
        public Guid Guid { get; set; }

        public Guid? GuidOrder { get; set; }

        public Guid? GuidProduce { get; set; }

        public int? Quantity { get; set; }

        public decimal? Price { get; set; }

        #endregion Properties

        public string ProduceLinkImg0 { get; set; }

        public string ProduceCode { get; set; }

        public string ProduceName { get; set; }

        public DateTime OrderCreatedAt { get; set; }

        public string OrderCreatedAtString { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }

        public List<UGalleryModel> ListGallery { get; set; }

        #region Constructor

        public WOrderDetailModel()
        {
        }

        public WOrderDetailModel(WOrderDetail wOrderDetail)
        {
            Guid = wOrderDetail.Guid;
            GuidOrder = wOrderDetail.GuidOrder;
            GuidProduce = wOrderDetail.GuidProduce;
            Quantity = wOrderDetail.Quantity;
            Price = wOrderDetail.Price;
        }

        #endregion Constructor

        #region Function

        public void Setvalue(WOrderDetail wOrderDetail)
        {
            if (IsUpdate == 0)
            {
                wOrderDetail.Guid = Guid.NewGuid();
            }
            wOrderDetail.GuidOrder = GuidOrder;
            wOrderDetail.GuidProduce = GuidProduce;
            wOrderDetail.Quantity = Quantity;
            wOrderDetail.Price = Price;
        }

        #endregion Function
    }
}