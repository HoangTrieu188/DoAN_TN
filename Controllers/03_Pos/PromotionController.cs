using Gemini.Models;
using Gemini.Models._03_Pos;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Gemini.Controllers.Bussiness;

namespace Gemini.Controllers._03_Pos
{
    public class PromotionController : GeminiController
    {
        private GeminiPromotionContext PromotionContext = new GeminiPromotionContext();

        public ActionResult Index()
        {
            return View("~/Views/03_Pos/Promotion/Index.cshtml");
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var data = PromotionContext.PosPromotions.OrderByDescending(x => x.CreatedAt).ToList();
            return Json(data.ToDataSourceResult(request));
        }

        public ActionResult Create()
        {
            var model = new PosPromotion();
            model.Active = true;
            model.StartDate = DateTime.Now;
            model.EndDate = DateTime.Now.AddDays(7);
            return View("~/Views/03_Pos/Promotion/Create.cshtml", model);
        }

        [HttpPost]
        public ActionResult Create(PosPromotion model)
        {
            if (ModelState.IsValid)
            {
                // Check duplicate code
                var exist = PromotionContext.PosPromotions.FirstOrDefault(x => x.Code.ToLower() == model.Code.ToLower());
                if (exist != null)
                {
                    if (exist.Guid != model.Guid)
                    {
                        ModelState.AddModelError("Code", "Mã khuyến mãi đã tồn tại");
                        return View("~/Views/03_Pos/Promotion/Create.cshtml", model);
                    }
                }

                if (model.Guid == Guid.Empty)
                {
                    model.Guid = Guid.NewGuid();
                    model.CreatedAt = DateTime.Now;
                    model.CreatedBy = User.Identity.Name;
                    PromotionContext.PosPromotions.Add(model);
                }
                else
                {
                    var edit = PromotionContext.PosPromotions.FirstOrDefault(x => x.Guid == model.Guid);
                    if (edit != null)
                    {
                        edit.Code = model.Code;
                        edit.Value = model.Value;
                        edit.IsPercent = model.IsPercent;
                        edit.StartDate = model.StartDate;
                        edit.EndDate = model.EndDate;
                        edit.Active = model.Active;
                        edit.Note = model.Note;
                        edit.UpdatedAt = DateTime.Now;
                        edit.UpdatedBy = User.Identity.Name;
                    }
                }
                PromotionContext.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("~/Views/03_Pos/Promotion/Create.cshtml", model);
        }

        public ActionResult Edit(Guid guid)
        {
            var model = PromotionContext.PosPromotions.FirstOrDefault(x => x.Guid == guid);
            return View("~/Views/03_Pos/Promotion/Create.cshtml", model);
        }

        public ActionResult Delete(Guid guid)
        {
            var model = PromotionContext.PosPromotions.FirstOrDefault(x => x.Guid == guid);
            if (model != null)
            {
                PromotionContext.PosPromotions.Remove(model);
                PromotionContext.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                PromotionContext.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
