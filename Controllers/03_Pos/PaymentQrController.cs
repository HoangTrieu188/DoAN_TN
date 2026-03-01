using Gemini.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;
using Gemini.Controllers.Bussiness;

namespace Gemini.Controllers._03_Pos
{
    public class PaymentQrController : GeminiController
    {
        private GeminiEntities db = new GeminiEntities();

        public ActionResult Index()
        {
            return View("~/Views/03_Pos/PaymentQr/Index.cshtml");
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var data = db.PosPaymentQrs.OrderByDescending(x => x.CreatedAt).ToList();
            return Json(data.ToDataSourceResult(request));
        }

        public ActionResult Create()
        {
            var model = new PosPaymentQr();
            model.Active = true;
            return View("~/Views/03_Pos/PaymentQr/Create.cshtml", model);
        }

        [HttpPost]
        public ActionResult Create(PosPaymentQr model)
        {
            if (ModelState.IsValid)
            {
                if (model.Guid == Guid.Empty)
                {
                    model.Guid = Guid.NewGuid();
                    model.CreatedAt = DateTime.Now;
                    model.CreatedBy = User.Identity.Name;
                    db.PosPaymentQrs.Add(model);
                }
                else
                {
                    var edit = db.PosPaymentQrs.FirstOrDefault(x => x.Guid == model.Guid);
                    if (edit != null)
                    {
                        edit.BankCode = model.BankCode;
                        edit.BankName = model.BankName;
                        edit.AccountName = model.AccountName;
                        edit.AccountNumber = model.AccountNumber;
                        edit.Template = model.Template;
                        edit.Active = model.Active;
                        edit.Note = model.Note;
                    }
                }

                // If this one is active, deactivate others
                if (model.Active)
                {
                    var others = db.PosPaymentQrs.Where(x => x.Guid != model.Guid && x.Active).ToList();
                    foreach (var item in others)
                    {
                        item.Active = false;
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("~/Views/03_Pos/PaymentQr/Create.cshtml", model);
        }

        public ActionResult Edit(Guid guid)
        {
            var model = db.PosPaymentQrs.FirstOrDefault(x => x.Guid == guid);
            return View("~/Views/03_Pos/PaymentQr/Create.cshtml", model);
        }

        public ActionResult Delete(Guid guid)
        {
            var model = db.PosPaymentQrs.FirstOrDefault(x => x.Guid == guid);
            if (model != null)
            {
                db.PosPaymentQrs.Remove(model);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
