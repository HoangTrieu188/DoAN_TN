using Gemini.Controllers.Bussiness;
using Gemini.Models._05_Website;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace Gemini.Controllers._05_Website
{
    [CustomAuthorizeAttribute]
    public class WRevenueReportController : GeminiController
    {
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        [HttpGet]
        public ActionResult List()
        {
            WRevenueReportModel model = new WRevenueReportModel();

            model.FromAt = DateTime.Now.AddDays(-1);
            model.ToAt = DateTime.Now;

            var lstRevenue = (from wod in DataGemini.WOrderDetails
                              join wo in DataGemini.WOrders on wod.GuidOrder equals wo.Guid
                              where model.FromAt <= wo.CreatedAt && model.ToAt >= wo.CreatedAt
                              select new WOrderDetailModel
                              {
                                  OrderCreatedAt = wo.CreatedAt,
                                  Quantity = wod.Quantity,
                                  Price = wod.Price,
                                  Size = wod.Size,
                                  Color = wod.Color
                              }).OrderBy(x => x.OrderCreatedAt).ToList();

            lstRevenue.ForEach(x => x.OrderCreatedAtString = x.OrderCreatedAt.ToString("dd/MM/yyyy"));

            var listLabel = lstRevenue.Select(x => x.OrderCreatedAtString).Distinct().ToList();
            var listRevenue = new List<long>();
            foreach (var item in listLabel)
            {
                var revenue = lstRevenue.Where(x => x.OrderCreatedAtString == item).Sum(x => (long)(x.Quantity * x.Price));
                listRevenue.Add(revenue);
            }

            model.ListLabel = Newtonsoft.Json.JsonConvert.SerializeObject(listLabel);
            model.ListRevenue = Newtonsoft.Json.JsonConvert.SerializeObject(listRevenue);

            return View(model);
        }

        [HttpPost]
        public ActionResult List(WRevenueReportModel model)
        {
            var lstRevenue = (from wod in DataGemini.WOrderDetails
                              join wo in DataGemini.WOrders on wod.GuidOrder equals wo.Guid
                              where model.FromAt <= wo.CreatedAt && model.ToAt >= wo.CreatedAt
                                    && wo.Status == WOrder_Status.Confirm
                              select new WOrderDetailModel
                              {
                                  OrderCreatedAt = wo.CreatedAt,
                                  Quantity = wod.Quantity,
                                  Price = wod.Price,
                                  Size = wod.Size,
                                  Color = wod.Color
                              }).OrderBy(x => x.OrderCreatedAt).ToList();

            lstRevenue.ForEach(x => x.OrderCreatedAtString = x.OrderCreatedAt.ToString("dd/MM/yyyy"));

            var listLabel = lstRevenue.Select(x => x.OrderCreatedAtString).Distinct().ToList();
            var listRevenue = new List<long>();
            foreach (var item in listLabel)
            {
                var revenue = lstRevenue.Where(x => x.OrderCreatedAtString == item).Sum(x => (long)(x.Quantity * x.Price));
                listRevenue.Add(revenue);
            }

            model.ListLabel = Newtonsoft.Json.JsonConvert.SerializeObject(listLabel);
            model.ListRevenue = Newtonsoft.Json.JsonConvert.SerializeObject(listRevenue);

            return View(model);
        }
    }
}