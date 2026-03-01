using Gemini.Controllers.Bussiness;
using Gemini.Models;
using Gemini.Models._01_Hethong;
using Gemini.Models._02_Cms.U;
using Gemini.Models._03_Pos;
using Gemini.Models._05_Website;
using Gemini.Models._20_Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Gemini.Controllers._20_Web.Home
{
    public class HomeCommonController : GeminiController
    {
        public string GetLanguageService()
        {
            return Request.Cookies["language"] != null ? (Request.Cookies["language"].Value) : ("VI");
        }

        [ChildActionOnly]
        public ActionResult Header()
        {
            ViewBag.CurrentUsername = GetUserInSession();

            var model = new HeaderModel();
            model.ListMenu = new List<SMenuModel>();

            try
            {
                model.ListMenu = (from menu in DataGemini.SMenus
                                  where menu.Active && menu.Type == "WEB"
                                  select new SMenuModel
                                  {
                                      LinkUrl = menu.LinkUrl,
                                      Name = menu.Name,
                                      OrderMenu = menu.OrderMenu,
                                  }).OrderBy(s => s.OrderMenu).ToList();

                var username = GetUserInSession();
                model.CurrentUsername = String.IsNullOrWhiteSpace(username) ? "Đăng nhập" : username;
            }
            catch (Exception ex)
            {
                model.ListMenu = new List<SMenuModel>();
            }

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult Footer()
        {
            var model = new FooterModel();
            model.ListPosCategory = new List<PosCategoryModel>();

            try
            {
                var dataCat = (from cat in DataGemini.PosCategories
                               where cat.Active
                               select new PosCategoryModel
                               {
                                   SeoFriendUrl = cat.SeoFriendUrl,
                                   Name = cat.Name,
                                   OrderBy = cat.OrderBy,
                                   Guid = cat.Guid,
                                   ParentGuid = cat.ParentGuid
                               }).OrderBy(x => x.OrderBy).ToList();

                var roots = BuildTreeCategory(dataCat);
                foreach (var item in roots)
                {
                    AppendCharsCategory(item, string.Empty, string.Empty);
                }
                model.ListPosCategory = roots.OrderBy(x => x.RootId).ThenBy(x => x.OrderBy).Where(x => x.ParentGuid == null || x.ParentGuid == Guid.Empty).ToList();

                model.Address = DataGemini.WOneParams.FirstOrDefault(x => x.Code == WOneParam_Code.Address)?.Description;
            }
            catch (Exception)
            {
                model.ListPosCategory = new List<PosCategoryModel>();
            }

            return PartialView(model);
        }

        #region Home

        public ActionResult PartialFavouriteProduct()
        {
            ViewBag.CurrentUsername = GetUserInSession();

            var model = new PartialLatestProductModel();

            model.ListPosProduce = (from pp in DataGemini.PosProduces
                                    join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                    where pp.Active && pc.Active
                                    select new PosProduceModel
                                    {
                                        Guid = pp.Guid,
                                        Code = pp.Code,
                                        Name = pp.Name,
                                        NameCategory = pc.Name,
                                        CategorySeoFriendUrl = pc.SeoFriendUrl,
                                        SeoFriendUrl = pp.SeoFriendUrl,
                                        Price = pp.Price,
                                        Unit = pp.Unit,
                                        LinkImg0 = (from fr in DataGemini.FProduceGalleries
                                                    join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                    where fr.GuidProduce == pp.Guid
                                                    orderby im.CreatedAt
                                                    select im.Image).FirstOrDefault(),
                                        CreatedAt = pp.CreatedAt,
                                        CreatedBy = pp.CreatedBy,
                                        Legit = pp.Legit,
                                        LegitCount = pp.LegitCount
                                    }).OrderByDescending(s => s.CreatedAt).Take(20).ToList();

            foreach (var item in model.ListPosProduce)
            {
                if (string.IsNullOrEmpty(item.LinkImg0))
                {
                    item.LinkImg0 = "/Content/Custom/empty-album.png";
                }
            }

            if (model.ListPosProduce != null && model.ListPosProduce.Any())
            {
                model.ListPosCategory = model.ListPosProduce.GroupBy(x => new { x.NameCategory, x.CategorySeoFriendUrl }).Select(x => new PosCategoryModel()
                {
                    Name = x.Key.NameCategory,
                    SeoFriendUrl = x.Key.CategorySeoFriendUrl
                }).OrderBy(x => x.Name).ToList();
            }

            return PartialView(model);
        }

        public ActionResult PartialCommonProduct()
        {
            ViewBag.CurrentUsername = GetUserInSession();

            var model = new PartialLatestProductModel();

            model.ListPosProduce = (from pp in DataGemini.PosProduces
                                    join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                    where pp.Active && pc.Active
                                    select new PosProduceModel
                                    {
                                        Guid = pp.Guid,
                                        Code = pp.Code,
                                        Name = pp.Name,
                                        NameCategory = pc.Name,
                                        CategorySeoFriendUrl = pc.SeoFriendUrl,
                                        SeoFriendUrl = pp.SeoFriendUrl,
                                        Price = pp.Price,
                                        Unit = pp.Unit,
                                        LinkImg0 = (from fr in DataGemini.FProduceGalleries
                                                    join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                    where fr.GuidProduce == pp.Guid
                                                    orderby im.CreatedAt
                                                    select im.Image).FirstOrDefault(),
                                        CreatedAt = pp.CreatedAt,
                                        CreatedBy = pp.CreatedBy,
                                        Legit = pp.Legit,
                                        LegitCount = pp.LegitCount
                                    }).OrderByDescending(s => s.CreatedAt).Take(20).ToList();

            foreach (var item in model.ListPosProduce)
            {
                if (string.IsNullOrEmpty(item.LinkImg0))
                {
                    item.LinkImg0 = "/Content/Custom/empty-album.png";
                }
            }

            if (model.ListPosProduce != null && model.ListPosProduce.Any())
            {
                model.ListPosCategory = model.ListPosProduce.GroupBy(x => new { x.NameCategory, x.CategorySeoFriendUrl }).Select(x => new PosCategoryModel()
                {
                    Name = x.Key.NameCategory,
                    SeoFriendUrl = x.Key.CategorySeoFriendUrl
                }).OrderBy(x => x.Name).ToList();
            }

            return PartialView(model);
        }

        public ActionResult PartialLatestProduct()
        {
            ViewBag.CurrentUsername = GetUserInSession();

            var model = new PartialLatestProductModel();

            model.ListPosProduce = (from pp in DataGemini.PosProduces
                                    join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                    where pp.Active && pc.Active
                                    select new PosProduceModel
                                    {
                                        Guid = pp.Guid,
                                        Code = pp.Code,
                                        Name = pp.Name,
                                        NameCategory = pc.Name,
                                        CategorySeoFriendUrl = pc.SeoFriendUrl,
                                        SeoFriendUrl = pp.SeoFriendUrl,
                                        Price = pp.Price,
                                        Unit = pp.Unit,
                                        LinkImg0 = (from fr in DataGemini.FProduceGalleries
                                                    join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                    where fr.GuidProduce == pp.Guid
                                                    orderby im.CreatedAt
                                                    select im.Image).FirstOrDefault(),
                                        CreatedAt = pp.CreatedAt,
                                        CreatedBy = pp.CreatedBy,
                                        Legit = pp.Legit,
                                        LegitCount = pp.LegitCount
                                    }).OrderByDescending(s => s.CreatedAt).Take(20).ToList();

            foreach (var item in model.ListPosProduce)
            {
                if (string.IsNullOrEmpty(item.LinkImg0))
                {
                    item.LinkImg0 = "/Content/Custom/empty-album.png";
                }
            }

            if (model.ListPosProduce != null && model.ListPosProduce.Any())
            {
                model.ListPosCategory = model.ListPosProduce.GroupBy(x => new { x.NameCategory, x.CategorySeoFriendUrl }).Select(x => new PosCategoryModel()
                {
                    Name = x.Key.NameCategory,
                    SeoFriendUrl = x.Key.CategorySeoFriendUrl
                }).OrderBy(x => x.Name).ToList();
            }

            return PartialView(model);
        }

        public ActionResult PartialFeaturedProduct()
        {
            var model = new PartialFeaturedProductModel();

            model.ListPosProduce = (from pp in DataGemini.PosProduces
                                    join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                    where pp.Active && pp.HotProduce && pc.Active
                                    select new PosProduceModel
                                    {
                                        Guid = pp.Guid,
                                        Code = pp.Code,
                                        Name = pp.Name,
                                        NameCategory = pc.Name,
                                        CategorySeoFriendUrl = pc.SeoFriendUrl,
                                        SeoFriendUrl = pp.SeoFriendUrl,
                                        Price = pp.Price,
                                        Unit = pp.Unit,
                                        LinkImg0 = (from fr in DataGemini.FProduceGalleries
                                                    join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                    where fr.GuidProduce == pp.Guid
                                                    orderby im.CreatedAt
                                                    select im.Image).FirstOrDefault(),
                                        CreatedAt = pp.CreatedAt,
                                        Legit = pp.Legit,
                                        LegitCount = pp.LegitCount
                                    }).OrderBy(s => s.CreatedAt).Take(9).ToList();

            foreach (var item in model.ListPosProduce)
            {
                if (string.IsNullOrEmpty(item.LinkImg0))
                {
                    item.LinkImg0 = "/Content/Custom/empty-album.png";
                }
            }

            return PartialView(model);
        }

        #endregion Home

        #region Live Chat

        [ChildActionOnly]
        public ActionResult PartialLiveChat()
        {
            var models = new WLiveChatModel();

            var username = GetUserInSession();
            models.CurrentUser = DataGemini.SUsers.FirstOrDefault(x => x.Username == username);
            ViewBag.UserFullName = models.CurrentUser?.FullName ?? "bạn";

            return PartialView(models);
        }

        #endregion Live Chat

        #region Gemini Chatbot
        [HttpPost]
        public ActionResult ChatWithAI(string userMessage)
        {
            try
            {
                string apiKey = "AIzaSyDr1fK3OC2dOqldL-SiziXbjAuE4JLysPM";
                string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key=" + apiKey;

                using (var client = new System.Net.WebClient())
                {
                    client.Headers[System.Net.HttpRequestHeader.ContentType] = "application/json";
                    client.Encoding = System.Text.Encoding.UTF8;

                    // Get current user's name
                    var username = GetUserInSession();
                    var currentUser = DataGemini.SUsers.FirstOrDefault(x => x.Username == username);
                    string customerName = currentUser?.FullName ?? "bạn";

                    // 1. Fetch Product Data (Active & In Stock)
                    // Get top 20 newest active products first
                    var topProductGuids = DataGemini.PosProduces
                        .Where(p => p.Active)
                        .OrderByDescending(p => p.CreatedAt)
                        .Select(p => p.Guid)
                        .Take(20)
                        .ToList();

                    // Fetch inventory for these products where Qty > 0
                    var inventoryData = (from i in DataGemini.PosInventories
                                         join p in DataGemini.PosProduces on i.GuidProduce equals p.Guid
                                         where topProductGuids.Contains(p.Guid) && i.Quantity > 0
                                         select new { p.Name, p.Code, p.Price, i.Size, i.Color, i.Quantity })
                                         .ToList();

                    // Group and Format
                    var productContext = string.Join("\n", inventoryData
                        .GroupBy(x => new { x.Name, x.Code, x.Price })
                        .Select(g => $"- {g.Key.Name} (Mã: {g.Key.Code}, Giá: {g.Key.Price:N0}đ):\n" +
                                     string.Join("\n", g.Select(v => $"  + Size {v.Size}, Màu {v.Color} (Còn {v.Quantity})"))));

                    // 2. Build System Prompt
                    string systemInstruction = $"Bạn là nhân viên tư vấn bán hàng chuyên nghiệp của cửa hàng. " +
                        $"Nhiệm vụ: Tư vấn ngắn gọn, đi thẳng vào vấn đề. CÓ SỬ DỤNG dữ liệu sản phẩm bên dưới.\n\n" +
                        $"TÊN KHÁCH HÀNG: {customerName}\n" +
                        $"Khi trả lời, hãy xưng hô bằng tên khách hàng (ví dụ: 'Chào {customerName}', '{customerName} ơi', v.v.)\n\n" +
                        "DANH SÁCH SẢN PHẨM HIỆN CÓ (Chỉ tư vấn các sản phẩm này):\n" + productContext + "\n\n" +
                        "HƯỚNG DẪN TƯ VẤN SIZE:\n" +
                        "- Cao <1m60, Nặng <50kg: Size S hoặc 28-29\n" +
                        "- Cao 1m60-1m70, Nặng 50-60kg: Size M hoặc 30-31\n" +
                        "- Cao 1m70-1m75, Nặng 60-70kg: Size L hoặc 32\n" +
                        "- Cao >1m75, Nặng >75kg: Size XL hoặc 33-34\n" +
                        "Tuyệt đối KHÔNG bịa ra sản phẩm không có trong danh sách.\n\n" +
                        "Câu hỏi của khách: " + userMessage;

                    var payload = new
                    {
                        contents = new[]
                        {
                            new {
                                parts = new[]
                                {
                                    new { text = systemInstruction }
                                }
                            }
                        }
                    };

                    string jsonPayload = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(payload);
                    string responseJson = client.UploadString(apiUrl, "POST", jsonPayload);

                    var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    dynamic result = serializer.Deserialize<dynamic>(responseJson);
                    string answer = result["candidates"][0]["content"]["parts"][0]["text"];

                    return Json(new { success = true, answer = answer });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, answer = "Xin lỗi, hệ thống đang bận. Vui lòng thử lại sau! (" + ex.Message + ")" });
            }
        }
        #endregion

        #region Produce List By Loved

        public ActionResult ProduceListByLoved(string page, string sortBy)
        {
            var username = GetUserInSession();
            ViewBag.CurrentUsername = username;

            int recordMax = 12;
            page = page ?? "page-1";
            string[] arrPage = page.Split('-');
            int numberPage = Convert.ToInt16(arrPage[1]) * recordMax;
            int numberPageActive = Convert.ToInt16(arrPage[1]);

            var models = new ProduceListByLovedModel();
            models.ListPosProduceByLoved = new List<PosProduceModel>();

            var dataCat = (from cat in DataGemini.PosCategories
                           where cat.Active
                           select new PosCategoryModel
                           {
                               SeoFriendUrl = cat.SeoFriendUrl,
                               Name = cat.Name,
                               OrderBy = cat.OrderBy,
                               Guid = cat.Guid,
                               ParentGuid = cat.ParentGuid
                           }).OrderBy(x => x.OrderBy).ToList();

            var roots = BuildTreeCategory(dataCat);
            foreach (var item in roots)
            {
                AppendCharsCategory(item, string.Empty, string.Empty);
            }
            models.ListPosCategory = roots.OrderBy(x => x.RootId).ThenBy(x => x.OrderBy).Where(x => x.ParentGuid == null || x.ParentGuid == Guid.Empty).ToList();

            models.ListPosProduceLatest = (from pp in DataGemini.PosProduces
                                           join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                           where pp.Active && pc.Active
                                           select new PosProduceModel
                                           {
                                               Guid = pp.Guid,
                                               Code = pp.Code,
                                               Name = pp.Name,
                                               NameCategory = pc.Name,
                                               CategorySeoFriendUrl = pc.SeoFriendUrl,
                                               SeoFriendUrl = pp.SeoFriendUrl,
                                               Price = pp.Price,
                                               Unit = pp.Unit,
                                               ListGallery = (from fr in DataGemini.FProduceGalleries
                                                              join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                              where fr.GuidProduce == pp.Guid
                                                              select new UGalleryModel
                                                              {
                                                                  Image = im.Image,
                                                                  CreatedAt = im.CreatedAt
                                                              }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                               CreatedAt = pp.CreatedAt,
                                               Legit = pp.Legit,
                                               LegitCount = pp.LegitCount
                                           }).OrderBy(s => s.CreatedAt).Take(9).ToList();

            foreach (var item in models.ListPosProduceLatest)
            {
                var tmpLinkImg = item.ListGallery;
                if (tmpLinkImg.Count == 0)
                {
                    item.LinkImg0 = "/Content/Custom/empty-album.png";
                }
                else
                {
                    item.LinkImg0 = tmpLinkImg[0].Image;
                }
            }

            string cookieName = "loveProduce_" + username;
            if (Request.Cookies[cookieName] != null)
            {
                var lstGuidProduceString = string.IsNullOrEmpty(Request.Cookies[cookieName].Value) ? String.Empty : Request.Cookies[cookieName].Value;
                var lstGuidProduce = lstGuidProduceString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.ToLower().Trim()).ToList();
                if (lstGuidProduce != null && lstGuidProduce.Any())
                {
                    var listPosProduceByLoved = from pp in DataGemini.PosProduces
                                                join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                                where pp.Active && pc.Active && lstGuidProduce.Contains(pp.Guid.ToString().ToLower().Trim())
                                                select new PosProduceModel
                                                {
                                                    Guid = pp.Guid,
                                                    Code = pp.Code,
                                                    Name = pp.Name,
                                                    NameCategory = pc.Name,
                                                    CategorySeoFriendUrl = pc.SeoFriendUrl,
                                                    SeoFriendUrl = pp.SeoFriendUrl,
                                                    Price = pp.Price,
                                                    Unit = pp.Unit,
                                                    ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                   join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                   where fr.GuidProduce == pp.Guid
                                                                   select new UGalleryModel
                                                                   {
                                                                       Image = im.Image,
                                                                       CreatedAt = im.CreatedAt
                                                                   }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                    CreatedAt = pp.CreatedAt,
                                                    Legit = pp.Legit,
                                                    LegitCount = pp.LegitCount
                                                };

                    if (!string.IsNullOrEmpty(sortBy))
                    {
                        switch (sortBy)
                        {
                            case "newest":
                                listPosProduceByLoved = listPosProduceByLoved.OrderByDescending(x => x.CreatedAt);
                                break;

                            case "oldest":
                                listPosProduceByLoved = listPosProduceByLoved.OrderBy(x => x.CreatedAt);
                                break;

                            case "a-z":
                                listPosProduceByLoved = listPosProduceByLoved.OrderBy(x => x.Name);
                                break;

                            case "z-a":
                                listPosProduceByLoved = listPosProduceByLoved.OrderByDescending(x => x.Name);
                                break;

                            case "priceH-L":
                                listPosProduceByLoved = listPosProduceByLoved.OrderByDescending(x => x.Price);
                                break;

                            case "priceL-H":
                                listPosProduceByLoved = listPosProduceByLoved.OrderBy(x => x.Price);
                                break;

                            default:
                                listPosProduceByLoved = listPosProduceByLoved.OrderByDescending(x => x.CreatedAt);
                                break;
                        }
                    }
                    else
                    {
                        listPosProduceByLoved = listPosProduceByLoved.OrderByDescending(x => x.CreatedAt);
                    }

                    //Sent data to view caculate
                    ViewData["perpage"] = recordMax;
                    ViewData["total"] = listPosProduceByLoved.Count();
                    ViewData["pageActive"] = numberPageActive;

                    //Check page start
                    if (Convert.ToInt16(arrPage[1]) == 1)
                    {
                        numberPage = 0;
                    }
                    else
                    {
                        numberPage = numberPage - recordMax;
                    }

                    models.ListPosProduceByLoved = listPosProduceByLoved.Skip(numberPage).Take(recordMax).ToList();

                    foreach (var item in models.ListPosProduceByLoved)
                    {
                        var tmpLinkImg = item.ListGallery;
                        if (tmpLinkImg.Count == 0)
                        {
                            item.LinkImg0 = "/Content/Custom/empty-album.png";
                        }
                        else
                        {
                            item.LinkImg0 = tmpLinkImg[0].Image;
                        }
                    }

                    return View("~/Views/HomeCommon/ProduceListByLoved.cshtml", models);
                }
                else
                {
                    //Sent data to view caculate
                    ViewData["perpage"] = recordMax;
                    ViewData["total"] = 0;
                    ViewData["pageActive"] = numberPageActive;

                    return View("~/Views/HomeCommon/ProduceListByLoved.cshtml", models);
                }
            }
            else
            {
                return View("~/Views/HomeCommon/Error_404.cshtml");
            }
        }

        #endregion Produce List By Loved

        #region Produce Cart

        [HttpGet]
        public ActionResult ProduceCart()
        {
            var username = GetUserInSession();
            ViewBag.CurrentUsername = username;

            if (string.IsNullOrEmpty(username))
            {
                return View("~/Views/HomeCommon/Error_404.cshtml");
            }
            else
            {
                var models = new ProduceCartModel();
                models.ListPosProduceCart = new List<PosProduceModel>();

                var dataCat = (from cat in DataGemini.PosCategories
                               where cat.Active
                               select new PosCategoryModel
                               {
                                   SeoFriendUrl = cat.SeoFriendUrl,
                                   Name = cat.Name,
                                   OrderBy = cat.OrderBy,
                                   Guid = cat.Guid,
                                   ParentGuid = cat.ParentGuid
                               }).OrderBy(x => x.OrderBy).ToList();

                var roots = BuildTreeCategory(dataCat);
                foreach (var item in roots)
                {
                    AppendCharsCategory(item, string.Empty, string.Empty);
                }
                models.ListPosCategory = roots.OrderBy(x => x.RootId).ThenBy(x => x.OrderBy).Where(x => x.ParentGuid == null || x.ParentGuid == Guid.Empty).ToList();

                var cookieName = "cartProduce_" + username;
                if (Request.Cookies[cookieName] != null)
                {
                    var cookieData = string.IsNullOrEmpty(Request.Cookies[cookieName].Value) ? String.Empty : Request.Cookies[cookieName].Value;
                    try
                    {
                        byte[] data = Convert.FromBase64String(cookieData);
                        var decodedData = Encoding.UTF8.GetString(data);
                        decodedData = Uri.UnescapeDataString(decodedData);
                        var lstProduce = JsonConvert.DeserializeObject<List<ProduceCartCookieModel>>(decodedData);
                        if (lstProduce != null && lstProduce.Any())
                        {
                            var lstGuidProduce = string.Join(",", lstProduce.Select(x => x.GuidProduce));
                            var listPosProduceCart = (from pp in DataGemini.PosProduces
                                                      join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                                      where pp.Active && pc.Active && lstGuidProduce.Contains(pp.Guid.ToString().ToLower().Trim())
                                                      select new PosProduceModel
                                                      {
                                                          Guid = pp.Guid,
                                                          Code = pp.Code,
                                                          Name = pp.Name,
                                                          NameCategory = pc.Name,
                                                          CategorySeoFriendUrl = pc.SeoFriendUrl,
                                                          SeoFriendUrl = pp.SeoFriendUrl,
                                                          Price = pp.Price,
                                                          Unit = pp.Unit,
                                                          ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                         join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                         where fr.GuidProduce == pp.Guid
                                                                         select new UGalleryModel
                                                                         {
                                                                             Image = im.Image,
                                                                             CreatedAt = im.CreatedAt
                                                                         }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                          CreatedAt = pp.CreatedAt,
                                                          Legit = pp.Legit,
                                                          LegitCount = pp.LegitCount,
                                                      }).ToList();

                            foreach (var item in listPosProduceCart)
                            {
                                item.Quantity = lstProduce.FirstOrDefault(x => x.GuidProduce == item.Guid.ToString().ToLower().Trim())?.Quantity;
                                item.Size = lstProduce.FirstOrDefault(x => x.GuidProduce == item.Guid.ToString().ToLower().Trim())?.Size;
                                item.Color = lstProduce.FirstOrDefault(x => x.GuidProduce == item.Guid.ToString().ToLower().Trim())?.Color;

                                var tmpLinkImg = item.ListGallery;
                                if (tmpLinkImg.Count == 0)
                                {
                                    item.LinkImg0 = "/Content/Custom/empty-album.png";
                                }
                                else
                                {
                                    item.LinkImg0 = tmpLinkImg[0].Image;
                                }
                            }

                            models.ListPosProduceCart = listPosProduceCart.Where(x => x.Quantity > 0).ToList();

                            models.ListTotalByUnit = new Dictionary<string, decimal>();
                            foreach (var itemGroup in models.ListPosProduceCart.GroupBy(x => x.Unit))
                            {
                                models.ListTotalByUnit.Add(itemGroup.Key, itemGroup.Sum(x => x.Price.GetValueOrDefault(0) * x.Quantity.GetValueOrDefault(0)));
                            }

                            return View("~/Views/HomeCommon/ProduceCart.cshtml", models);
                        }
                        else
                        {
                            return View("~/Views/HomeCommon/ProduceCart.cshtml", models);
                        }
                    }
                    catch
                    {
                        return View("~/Views/HomeCommon/ProduceCart.cshtml", models);
                    }
                }
                else
                {
                    return View("~/Views/HomeCommon/Error_404.cshtml");
                }
            }
        }


        [HttpGet]
        public ActionResult GetActiveQr()
        {
            try
            {
                using (var db = new GeminiEntities())
                {
                    var qr = db.PosPaymentQrs.FirstOrDefault(x => x.Active);
                    if (qr != null)
                    {
                        return Json(new { StatusCode = 200, Data = qr }, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(new { StatusCode = 404, Message = "Chưa cấu hình QR" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { StatusCode = 500, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult PayCart(string discountCode, string paymentMethod = "VNPAY")
        {
            //Get Config Info
            string vnp_Returnurl = ConfigurationManager.AppSettings["vnp_Returnurl"]; //URL nhan ket qua tra ve
            string vnp_Url = ConfigurationManager.AppSettings["vnp_Url"]; //URL thanh toan cua VNPAY
            string vnp_TmnCode = ConfigurationManager.AppSettings["vnp_TmnCode"]; //Ma website
            string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
            string vnp_BankCode = ConfigurationManager.AppSettings["vnp_BankCode"]; //Ma ngan hang - Mặc định: NCB (Thanh toán trực tuyến)
            string vnp_OrderCategory = ConfigurationManager.AppSettings["vnp_OrderCategory"]; //Loại hàng hóa - Mặc định: other (Thanh toán trực tuyến)

            string msg = "";
            int statusCode = (int)Convert.ToInt16(HttpStatusCode.Conflict);
            try
            {
                var username = GetUserInSession();
                var user = DataGemini.SUsers.FirstOrDefault(x => x.Username == username);
                if (user != null)
                {
                    var cookieName = "cartProduce_" + user.Username;
                    if (Request.Cookies[cookieName] != null)
                    {
                        var cookieData = string.IsNullOrEmpty(Request.Cookies[cookieName].Value) ? String.Empty : Request.Cookies[cookieName].Value;

                        byte[] data = Convert.FromBase64String(cookieData);
                        var decodedData = Encoding.UTF8.GetString(data);
                        decodedData = Uri.UnescapeDataString(decodedData);
                        var lstProduce = JsonConvert.DeserializeObject<List<ProduceCartCookieModel>>(decodedData);

                        if (lstProduce != null && lstProduce.Any())
                        {
                            var lstGuidProduce = string.Join(",", lstProduce.Select(x => x.GuidProduce));
                            var listPosProduceCart = (from pp in DataGemini.PosProduces
                                                      join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                                      where pp.Active && pc.Active && lstGuidProduce.Contains(pp.Guid.ToString().ToLower().Trim())
                                                      select new PosProduceModel
                                                      {
                                                          Guid = pp.Guid,
                                                          Code = pp.Code,
                                                          Price = pp.Price,
                                                      }).ToList();

                            // Calculate Total Amount
                            decimal totalAmount = 0;
                            List<WOrderDetail> orderDetails = new List<WOrderDetail>();
                            foreach (var item in listPosProduceCart)
                            {
                                int quantity = (lstProduce.FirstOrDefault(x => x.GuidProduce == item.Guid.ToString().ToLower().Trim())?.Quantity).GetValueOrDefault(0);
                                totalAmount += (item.Price.GetValueOrDefault(0) * quantity);

                                orderDetails.Add(new WOrderDetail()
                                {
                                    Guid = Guid.NewGuid(),
                                    GuidProduce = item.Guid,
                                    Quantity = quantity,
                                    Price = item.Price,
                                    Size = lstProduce.FirstOrDefault(x => x.GuidProduce == item.Guid.ToString().ToLower().Trim())?.Size,
                                    Color = lstProduce.FirstOrDefault(x => x.GuidProduce == item.Guid.ToString().ToLower().Trim())?.Color
                                });
                            }

                            // Calculate Discount
                            decimal discountAmount = 0;
                            if (!string.IsNullOrEmpty(discountCode))
                            {
                                using (var db = new Gemini.Models.GeminiPromotionContext())
                                {
                                    var promo = db.PosPromotions.FirstOrDefault(x => x.Code == discountCode && x.Active);
                                    if (promo != null && promo.StartDate <= DateTime.Now && promo.EndDate >= DateTime.Now)
                                    {
                                        if (promo.IsPercent)
                                        {
                                            discountAmount = totalAmount * promo.Value / 100;
                                        }
                                        else
                                        {
                                            discountAmount = promo.Value;
                                        }
                                    }
                                }
                            }
                            if (discountAmount > totalAmount) discountAmount = totalAmount;
                            decimal finalAmount = totalAmount - discountAmount;

                            //Save order to db
                            WOrder order = new WOrder();
                            order.Guid = Guid.NewGuid();
                            order.GuidUser = user.Guid;
                            order.FullAddress = user.FullAddress;
                            order.Mobile = user.Mobile;
                            order.Status = WOrder_Status.Inprogress;
                            order.CreatedAt = DateTime.Now;
                            order.CreatedBy = username;
                            order.DiscountCode = discountCode;
                            order.DiscountAmount = discountAmount;
                            // Add Payment Method note if needed, but WOrder might not have field. 
                            // Assuming we can proceed without saving payment method explicitly or use Note field if exists.

                            DataGemini.WOrders.Add(order);

                            foreach (var detail in orderDetails)
                            {
                                detail.GuidOrder = order.Guid;
                            }
                            DataGemini.WOrderDetails.AddRange(orderDetails);

                            DataGemini.SaveChanges();

                            // QR Payment Logic
                            if (paymentMethod == "QR")
                            {
                                statusCode = (int)Convert.ToInt16(HttpStatusCode.OK);
                                return Json(new
                                {
                                    StatusCode = statusCode,
                                    PaymentType = "QR",
                                    Amount = finalAmount,
                                    OrderCode = "DH" + order.Guid.ToString().Substring(0, 8).ToUpper(),
                                    OrderGuid = order.Guid.ToString()
                                }, "text/plain");
                            }

                            //Build URL for VNPAY
                            VnPayLibrary vnpay = new VnPayLibrary();

                            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
                            vnpay.AddRequestData("vnp_Command", "pay");
                            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                            vnpay.AddRequestData("vnp_Amount", (finalAmount * 100).ToString("0")); // Updated to use finalAmount
                            vnpay.AddRequestData("vnp_BankCode", vnp_BankCode);
                            vnpay.AddRequestData("vnp_CreateDate", order.CreatedAt.ToString("yyyyMMddHHmmss"));
                            vnpay.AddRequestData("vnp_CurrCode", "VND");
                            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress());
                            vnpay.AddRequestData("vnp_Locale", "vn");
                            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.Guid.ToString().ToLower());
                            vnpay.AddRequestData("vnp_OrderType", vnp_OrderCategory); //default value: other
                            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
                            vnpay.AddRequestData("vnp_TxnRef", order.Guid.ToString().ToLower());

                            //Add Params of 2.1.0 Version
                            vnpay.AddRequestData("vnp_ExpireDate", order.CreatedAt.AddMinutes(15).ToString("yyyyMMddHHmmss"));

                            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
                            statusCode = (int)Convert.ToInt16(HttpStatusCode.OK);
                            return Json(new { StatusCode = statusCode, PaymentUrl = paymentUrl }, "text/plain");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return Json(new { StatusCode = statusCode, Message = msg }, "text/plain");
        }

        [HttpPost]
        public ActionResult CheckDiscount(string code)
        {
            try
            {
                using (var db = new Gemini.Models.GeminiPromotionContext())
                {
                    var promo = db.PosPromotions.FirstOrDefault(x => x.Code == code && x.Active);
                    if (promo == null)
                    {
                        return Json(new { StatusCode = 404, Message = "Mã khuyến mãi không tồn tại hoặc đã hết hạn" });
                    }

                    if (promo.StartDate > DateTime.Now || promo.EndDate < DateTime.Now)
                    {
                        return Json(new { StatusCode = 400, Message = "Mã khuyến mãi chưa bắt đầu hoặc đã kết thúc" });
                    }

                    return Json(new { StatusCode = 200, Value = promo.Value, IsPercent = promo.IsPercent });
                }
            }
            catch (Exception ex)
            {
                return Json(new { StatusCode = 500, Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ConfirmQrPayment(string orderGuid)
        {
            try
            {
                Guid guid = Guid.Parse(orderGuid);
                var order = DataGemini.WOrders.FirstOrDefault(x => x.Guid == guid);

                if (order == null)
                {
                    return Json(new { StatusCode = 404, Message = "Không tìm thấy đơn hàng" });
                }

                // Update order status to Paid (waiting for admin approval)
                order.Status = WOrder_Status.Paid;
                order.UpdatedAt = DateTime.Now;
                order.UpdatedBy = GetUserInSession();

                DataGemini.SaveChanges();

                return Json(new { StatusCode = 200, Message = "Xác nhận đơn hàng thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { StatusCode = 500, Message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult CancelQrPayment(string orderGuid)
        {
            try
            {
                Guid guid = Guid.Parse(orderGuid);
                var order = DataGemini.WOrders.FirstOrDefault(x => x.Guid == guid);

                if (order == null)
                {
                    return Json(new { StatusCode = 404, Message = "Không tìm thấy đơn hàng" });
                }

                // Delete order and order details
                var orderDetails = DataGemini.WOrderDetails.Where(x => x.GuidOrder == guid).ToList();
                DataGemini.WOrderDetails.RemoveRange(orderDetails);
                DataGemini.WOrders.Remove(order);

                DataGemini.SaveChanges();

                return Json(new { StatusCode = 200, Message = "Hủy đơn hàng thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { StatusCode = 500, Message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult VnPayIPN()
        {
            string returnContent = string.Empty;
            if (Request.QueryString.Count > 0)
            {
                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Secret key
                var vnpayData = Request.QueryString;
                VnPayLibrary vnpay = new VnPayLibrary();
                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                //Lay danh sach tham so tra ve tu VNPAY
                //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay
                //vnp_TransactionNo: Ma GD tai he thong VNPAY
                //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
                //vnp_SecureHash: HmacSHA512 cua du lieu tra ve

                string orderId = vnpay.GetResponseData("vnp_TxnRef");
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    //Cap nhat ket qua GD
                    //Yeu cau: Truy van vao CSDL cua  Merchant => lay ra duoc OrderInfo
                    //Giả sử OrderInfo lấy ra được như giả lập bên dưới
                    WOrder order = DataGemini.WOrders.FirstOrDefault(x => x.Guid == new Guid(orderId));
                    List<WOrderDetail> orderDetails = DataGemini.WOrderDetails.Where(x => x.GuidOrder == new Guid(orderId)).ToList();

                    //Kiem tra tinh trang Order
                    if (order != null)
                    {
                        if (orderDetails.Sum(x => x.Price * x.Quantity) == vnp_Amount)
                        {
                            if (order.Status == WOrder_Status.Inprogress)
                            {
                                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                                {
                                    //Thanh toan thanh cong
                                    order.Status = WOrder_Status.Paid;
                                }
                                else
                                {
                                    //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                                    order.Status = WOrder_Status.Error;
                                }

                                //Thêm code Thực hiện cập nhật vào Database
                                //Update Database
                                order.PaymentTranId = vnpayTranId;
                                DataGemini.SaveChanges();

                                returnContent = "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}";
                            }
                            else
                            {
                                returnContent = "{\"RspCode\":\"02\",\"Message\":\"Order already confirmed\"}";
                            }
                        }
                        else
                        {
                            returnContent = "{\"RspCode\":\"04\",\"Message\":\"invalid amount\"}";
                        }
                    }
                    else
                    {
                        returnContent = "{\"RspCode\":\"01\",\"Message\":\"Order not found\"}";
                    }
                }
                else
                {
                    returnContent = "{\"RspCode\":\"97\",\"Message\":\"Invalid signature\"}";
                }
            }
            else
            {
                returnContent = "{\"RspCode\":\"99\",\"Message\":\"Input data required\"}";
            }

            return VnPayReturn(Request.QueryString);
        }
        [HttpGet]
        public ActionResult VnPayReturn()
        {
            return VnPayReturn(Request.QueryString);
        }
        public ActionResult VnPayReturn(NameValueCollection requestString)
        {
            ViewBag.CurrentUsername = GetUserInSession();

            try
            {
                var model = new VnPayReturnModel();

                string vnp_HashSecret = ConfigurationManager.AppSettings["vnp_HashSecret"]; //Chuoi bi mat
                var vnpayData = requestString;
                VnPayLibrary vnpay = new VnPayLibrary();

                foreach (string s in vnpayData)
                {
                    //get all querystring data
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        vnpay.AddResponseData(s, vnpayData[s]);
                    }
                }
                //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay
                //vnp_TransactionNo: Ma GD tai he thong VNPAY
                //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
                //vnp_SecureHash: HmacSHA512 cua du lieu tra ve

                string orderId = vnpay.GetResponseData("vnp_TxnRef");
                long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
                string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
                string vnp_TransactionStatus = vnpay.GetResponseData("vnp_TransactionStatus");
                String vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
                String TerminalID = Request.QueryString["vnp_TmnCode"];
                long vnp_Amount = Convert.ToInt64(vnpay.GetResponseData("vnp_Amount")) / 100;
                String bankCode = Request.QueryString["vnp_BankCode"];

                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        //Thanh toan thanh cong
                        model.Message = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ";

                        // Cập nhật trạng thái đơn hàng
                        Guid guidOrder;
                        if (Guid.TryParse(orderId, out guidOrder))
                        {
                            var order = DataGemini.WOrders.FirstOrDefault(x => x.Guid == guidOrder);
                            if (order != null && order.Status == WOrder_Status.Inprogress)
                            {
                                order.Status = WOrder_Status.Paid;
                                order.PaymentTranId = vnpayTranId;
                                order.UpdatedAt = DateTime.Now;
                                DataGemini.SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                        model.Message = "Có lỗi xảy ra trong quá trình xử lý. Mã lỗi: " + vnp_ResponseCode;
                    }
                    model.TxnRef = "Mã giao dịch thanh toán:" + orderId;
                    model.VnpayTranNo = "Mã giao dịch tại VNPAY:" + vnpayTranId.ToString();
                    model.Amount = "Số tiền thanh toán (VND):" + String.Format("{0:n0}", vnp_Amount);
                    model.BankCode = "Ngân hàng thanh toán:" + bankCode;
                }
                else
                {
                    model.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }

                return View("VnPayReturn", model);
            }
            catch
            {
                return View("~/Views/HomeCommon/Error_404.cshtml");
            }
        }

        #endregion Produce Cart

        #region Produce List By CreatedBy

        public ActionResult ProduceListByCreatedBy(string createdBy, string page, string sortBy)
        {
            ViewBag.CurrentUsername = GetUserInSession();

            if (!string.IsNullOrEmpty(createdBy))
            {
                int recordMax = 12;
                page = page ?? "page-1";
                string[] arrPage = page.Split('-');
                int numberPage = Convert.ToInt16(arrPage[1]) * recordMax;
                int numberPageActive = Convert.ToInt16(arrPage[1]);

                var models = new ProduceListByCreatedByModel();

                models.ListPosCategory = new List<PosCategoryModel>();
                models.ListPosProduceLatest = new List<PosProduceModel>();
                models.ListPosProduceByCreatedBy = new List<PosProduceModel>();

                var dataCat = (from cat in DataGemini.PosCategories
                               where cat.Active
                               select new PosCategoryModel
                               {
                                   SeoFriendUrl = cat.SeoFriendUrl,
                                   Name = cat.Name,
                                   OrderBy = cat.OrderBy,
                                   Guid = cat.Guid,
                                   ParentGuid = cat.ParentGuid
                               }).OrderBy(x => x.OrderBy).ToList();

                var roots = BuildTreeCategory(dataCat);
                foreach (var item in roots)
                {
                    AppendCharsCategory(item, string.Empty, string.Empty);
                }
                models.ListPosCategory = roots.OrderBy(x => x.RootId).ThenBy(x => x.OrderBy).Where(x => x.ParentGuid == null || x.ParentGuid == Guid.Empty).ToList();

                models.ListPosProduceLatest = (from pp in DataGemini.PosProduces
                                               join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                               where pp.Active && pc.Active
                                               select new PosProduceModel
                                               {
                                                   Guid = pp.Guid,
                                                   Code = pp.Code,
                                                   Name = pp.Name,
                                                   NameCategory = pc.Name,
                                                   CategorySeoFriendUrl = pc.SeoFriendUrl,
                                                   SeoFriendUrl = pp.SeoFriendUrl,
                                                   Price = pp.Price,
                                                   Unit = pp.Unit,
                                                   ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                  join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                  where fr.GuidProduce == pp.Guid
                                                                  select new UGalleryModel
                                                                  {
                                                                      Image = im.Image,
                                                                      CreatedAt = im.CreatedAt
                                                                  }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                   CreatedAt = pp.CreatedAt,
                                                   Legit = pp.Legit,
                                                   LegitCount = pp.LegitCount
                                               }).OrderBy(s => s.CreatedAt).Take(9).ToList();

                foreach (var item in models.ListPosProduceLatest)
                {
                    var tmpLinkImg = item.ListGallery;
                    if (tmpLinkImg.Count == 0)
                    {
                        item.LinkImg0 = "/Content/Custom/empty-album.png";
                    }
                    else
                    {
                        item.LinkImg0 = tmpLinkImg[0].Image;
                    }
                }

                models.PosProduceCreatedBy = new SUserModel(DataGemini.SUsers.FirstOrDefault(x => x.Username == createdBy));
                if (models.PosProduceCreatedBy != null && string.IsNullOrEmpty(models.PosProduceCreatedBy.Avartar))
                {
                    models.PosProduceCreatedBy.Avartar = DefaultImage.ImageEmpty;
                    models.PosProduceCreatedBy.CountFollow = DataGemini.SUsers.Count(x => x.GuidFollow.Contains(models.PosProduceCreatedBy.Guid.ToString().ToLower().Trim()));
                }

                if (models.PosProduceCreatedBy != null)
                {
                    var listPosProduceByCreatedBy = from pp in DataGemini.PosProduces
                                                    join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                                    where pp.Active && pc.Active && pp.CreatedBy == models.PosProduceCreatedBy.Username
                                                    select new PosProduceModel
                                                    {
                                                        Guid = pp.Guid,
                                                        Code = pp.Code,
                                                        Name = pp.Name,
                                                        NameCategory = pc.Name,
                                                        CategorySeoFriendUrl = pc.SeoFriendUrl,
                                                        SeoFriendUrl = pp.SeoFriendUrl,
                                                        Price = pp.Price,
                                                        Unit = pp.Unit,
                                                        ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                       join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                       where fr.GuidProduce == pp.Guid
                                                                       select new UGalleryModel
                                                                       {
                                                                           Image = im.Image,
                                                                           CreatedAt = im.CreatedAt
                                                                       }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                        CreatedAt = pp.CreatedAt,
                                                        Legit = pp.Legit,
                                                        LegitCount = pp.LegitCount
                                                    };

                    if (!string.IsNullOrEmpty(sortBy))
                    {
                        switch (sortBy)
                        {
                            case "newest":
                                listPosProduceByCreatedBy = listPosProduceByCreatedBy.OrderByDescending(x => x.CreatedAt);
                                break;

                            case "oldest":
                                listPosProduceByCreatedBy = listPosProduceByCreatedBy.OrderBy(x => x.CreatedAt);
                                break;

                            case "a-z":
                                listPosProduceByCreatedBy = listPosProduceByCreatedBy.OrderBy(x => x.Name);
                                break;

                            case "z-a":
                                listPosProduceByCreatedBy = listPosProduceByCreatedBy.OrderByDescending(x => x.Name);
                                break;

                            case "priceH-L":
                                listPosProduceByCreatedBy = listPosProduceByCreatedBy.OrderByDescending(x => x.Price);
                                break;

                            case "priceL-H":
                                listPosProduceByCreatedBy = listPosProduceByCreatedBy.OrderBy(x => x.Price);
                                break;

                            default:
                                listPosProduceByCreatedBy = listPosProduceByCreatedBy.OrderByDescending(x => x.CreatedAt);
                                break;
                        }
                    }
                    else
                    {
                        listPosProduceByCreatedBy = listPosProduceByCreatedBy.OrderByDescending(x => x.CreatedAt);
                    }

                    //Sent data to view caculate
                    ViewData["perpage"] = recordMax;
                    ViewData["total"] = listPosProduceByCreatedBy.Count();
                    ViewData["pageActive"] = numberPageActive;

                    //Check page start
                    if (Convert.ToInt16(arrPage[1]) == 1)
                    {
                        numberPage = 0;
                    }
                    else
                    {
                        numberPage = numberPage - recordMax;
                    }

                    models.ListPosProduceByCreatedBy = listPosProduceByCreatedBy.Skip(numberPage).Take(recordMax).ToList();

                    foreach (var item in models.ListPosProduceByCreatedBy)
                    {
                        var tmpLinkImg = item.ListGallery;
                        if (tmpLinkImg.Count == 0)
                        {
                            item.LinkImg0 = "/Content/Custom/empty-album.png";
                        }
                        else
                        {
                            item.LinkImg0 = tmpLinkImg[0].Image;
                        }
                    }

                    return View("~/Views/HomeCommon/ProduceListByCreatedBy.cshtml", models);
                }
                else
                {
                    return View("~/Views/HomeCommon/Error_404.cshtml");
                }
            }

            return View("~/Views/HomeCommon/Error_404.cshtml");
        }

        #endregion Produce List By CreatedBy

        #region Produce List By Search

        public ActionResult ProduceListBySearch(string keyWord, string page, string sortBy)
        {
            ViewBag.CurrentUsername = GetUserInSession();

            int recordMax = 12;
            page = page ?? "page-1";
            string[] arrPage = page.Split('-');
            int numberPage = Convert.ToInt16(arrPage[1]) * recordMax;
            int numberPageActive = Convert.ToInt16(arrPage[1]);

            var models = new ProduceListBySearchModel();
            models.ListPosProduceBySearch = new List<PosProduceModel>();
            models.KeyWord = keyWord;

            var dataCat = (from cat in DataGemini.PosCategories
                           where cat.Active
                           select new PosCategoryModel
                           {
                               SeoFriendUrl = cat.SeoFriendUrl,
                               Name = cat.Name,
                               OrderBy = cat.OrderBy,
                               Guid = cat.Guid,
                               ParentGuid = cat.ParentGuid
                           }).OrderBy(x => x.OrderBy).ToList();

            var roots = BuildTreeCategory(dataCat);
            foreach (var item in roots)
            {
                AppendCharsCategory(item, string.Empty, string.Empty);
            }
            models.ListPosCategory = roots.OrderBy(x => x.RootId).ThenBy(x => x.OrderBy).Where(x => x.ParentGuid == null || x.ParentGuid == Guid.Empty).ToList();

            models.ListPosProduceLatest = (from pp in DataGemini.PosProduces
                                           join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                           where pp.Active && pc.Active
                                           select new PosProduceModel
                                           {
                                               Guid = pp.Guid,
                                               Code = pp.Code,
                                               Name = pp.Name,
                                               NameCategory = pc.Name,
                                               CategorySeoFriendUrl = pc.SeoFriendUrl,
                                               SeoFriendUrl = pp.SeoFriendUrl,
                                               Price = pp.Price,
                                               Unit = pp.Unit,
                                               ListGallery = (from fr in DataGemini.FProduceGalleries
                                                              join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                              where fr.GuidProduce == pp.Guid
                                                              select new UGalleryModel
                                                              {
                                                                  Image = im.Image,
                                                                  CreatedAt = im.CreatedAt
                                                              }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                               CreatedAt = pp.CreatedAt,
                                               Legit = pp.Legit,
                                               LegitCount = pp.LegitCount
                                           }).OrderBy(s => s.CreatedAt).Take(9).ToList();

            foreach (var item in models.ListPosProduceLatest)
            {
                var tmpLinkImg = item.ListGallery;
                if (tmpLinkImg.Count == 0)
                {
                    item.LinkImg0 = "/Content/Custom/empty-album.png";
                }
                else
                {
                    item.LinkImg0 = tmpLinkImg[0].Image;
                }
            }

            if (!string.IsNullOrEmpty(keyWord))
            {
                var listPosProduceBySearch = from pp in DataGemini.PosProduces
                                             join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                             where pp.Active && pc.Active
                                             && (pp.Name.Contains(keyWord)
                                                || pp.Note.Contains(keyWord)
                                                || pp.Description.Contains(keyWord)
                                                || pp.CreatedBy.Contains(keyWord)
                                                || pc.Name.Contains(keyWord))
                                             select new PosProduceModel
                                             {
                                                 Guid = pp.Guid,
                                                 Code = pp.Code,
                                                 Name = pp.Name,
                                                 NameCategory = pc.Name,
                                                 CategorySeoFriendUrl = pc.SeoFriendUrl,
                                                 SeoFriendUrl = pp.SeoFriendUrl,
                                                 Price = pp.Price,
                                                 Unit = pp.Unit,
                                                 ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                where fr.GuidProduce == pp.Guid
                                                                select new UGalleryModel
                                                                {
                                                                    Image = im.Image,
                                                                    CreatedAt = im.CreatedAt
                                                                }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                 CreatedAt = pp.CreatedAt,
                                                 Legit = pp.Legit,
                                                 LegitCount = pp.LegitCount
                                             };

                if (!string.IsNullOrEmpty(sortBy))
                {
                    switch (sortBy)
                    {
                        case "newest":
                            listPosProduceBySearch = listPosProduceBySearch.OrderByDescending(x => x.CreatedAt);
                            break;

                        case "oldest":
                            listPosProduceBySearch = listPosProduceBySearch.OrderBy(x => x.CreatedAt);
                            break;

                        case "a-z":
                            listPosProduceBySearch = listPosProduceBySearch.OrderBy(x => x.Name);
                            break;

                        case "z-a":
                            listPosProduceBySearch = listPosProduceBySearch.OrderByDescending(x => x.Name);
                            break;

                        case "priceH-L":
                            listPosProduceBySearch = listPosProduceBySearch.OrderByDescending(x => x.Price);
                            break;

                        case "priceL-H":
                            listPosProduceBySearch = listPosProduceBySearch.OrderBy(x => x.Price);
                            break;

                        default:
                            listPosProduceBySearch = listPosProduceBySearch.OrderByDescending(x => x.CreatedAt);
                            break;
                    }
                }
                else
                {
                    listPosProduceBySearch = listPosProduceBySearch.OrderByDescending(x => x.CreatedAt);
                }

                //Sent data to view caculate
                ViewData["perpage"] = recordMax;
                ViewData["total"] = listPosProduceBySearch.Count();
                ViewData["pageActive"] = numberPageActive;

                //Check page start
                if (Convert.ToInt16(arrPage[1]) == 1)
                {
                    numberPage = 0;
                }
                else
                {
                    numberPage = numberPage - recordMax;
                }

                models.ListPosProduceBySearch = listPosProduceBySearch.Skip(numberPage).Take(recordMax).ToList();

                foreach (var item in models.ListPosProduceBySearch)
                {
                    var tmpLinkImg = item.ListGallery;
                    if (tmpLinkImg.Count == 0)
                    {
                        item.LinkImg0 = "/Content/Custom/empty-album.png";
                    }
                    else
                    {
                        item.LinkImg0 = tmpLinkImg[0].Image;
                    }
                }

                return View("~/Views/HomeCommon/ProduceListBySearch.cshtml", models);
            }
            else
            {
                //Sent data to view caculate
                ViewData["perpage"] = recordMax;
                ViewData["total"] = 0;
                ViewData["pageActive"] = numberPageActive;

                return View("~/Views/HomeCommon/ProduceListBySearch.cshtml", models);
            }
        }

        #endregion Produce List By Search

        #region Produce List By Category

        public ActionResult ProduceListByCategory(string seoFriendUrl, string page, string sortBy)
        {
            ViewBag.CurrentUsername = GetUserInSession();

            if (!string.IsNullOrEmpty(seoFriendUrl))
            {
                int recordMax = 12;
                page = page ?? "page-1";
                string[] arrPage = page.Split('-');
                int numberPage = Convert.ToInt16(arrPage[1]) * recordMax;
                int numberPageActive = Convert.ToInt16(arrPage[1]);

                var models = new ProduceListByCategoryModel();

                models.PosCategory = new PosCategoryModel();
                models.ListPosCategory = new List<PosCategoryModel>();
                models.ListPosProduceLatest = new List<PosProduceModel>();
                models.ListPosProduceByCategory = new List<PosProduceModel>();

                var dataCat = (from cat in DataGemini.PosCategories
                               where cat.Active
                               select new PosCategoryModel
                               {
                                   SeoFriendUrl = cat.SeoFriendUrl,
                                   Name = cat.Name,
                                   OrderBy = cat.OrderBy,
                                   Guid = cat.Guid,
                                   ParentGuid = cat.ParentGuid
                               }).OrderBy(x => x.OrderBy).ToList();

                var roots = BuildTreeCategory(dataCat);
                foreach (var item in roots)
                {
                    AppendCharsCategory(item, string.Empty, string.Empty);
                }
                models.ListPosCategory = roots.OrderBy(x => x.RootId).ThenBy(x => x.OrderBy).Where(x => x.ParentGuid == null || x.ParentGuid == Guid.Empty).ToList();

                models.ListPosProduceLatest = (from pp in DataGemini.PosProduces
                                               join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                               where pp.Active && pc.Active
                                               select new PosProduceModel
                                               {
                                                   Guid = pp.Guid,
                                                   Code = pp.Code,
                                                   Name = pp.Name,
                                                   NameCategory = pc.Name,
                                                   CategorySeoFriendUrl = pc.SeoFriendUrl,
                                                   SeoFriendUrl = pp.SeoFriendUrl,
                                                   Price = pp.Price,
                                                   Unit = pp.Unit,
                                                   ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                  join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                  where fr.GuidProduce == pp.Guid
                                                                  select new UGalleryModel
                                                                  {
                                                                      Image = im.Image,
                                                                      CreatedAt = im.CreatedAt
                                                                  }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                   CreatedAt = pp.CreatedAt,
                                                   Legit = pp.Legit,
                                                   LegitCount = pp.LegitCount
                                               }).OrderBy(s => s.CreatedAt).Take(9).ToList();

                foreach (var item in models.ListPosProduceLatest)
                {
                    var tmpLinkImg = item.ListGallery;
                    if (tmpLinkImg.Count == 0)
                    {
                        item.LinkImg0 = "/Content/Custom/empty-album.png";
                    }
                    else
                    {
                        item.LinkImg0 = tmpLinkImg[0].Image;
                    }
                }

                models.PosCategory = (from cat in DataGemini.PosCategories
                                      where cat.Active && cat.SeoFriendUrl.ToLower().Trim() == seoFriendUrl
                                      select new PosCategoryModel
                                      {
                                          Guid = cat.Guid,
                                          SeoFriendUrl = cat.SeoFriendUrl,
                                          Name = cat.Name
                                      }).FirstOrDefault();

                if (models.PosCategory != null)
                {
                    var listPosProduceByCategory = from pp in DataGemini.PosProduces
                                                   join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                                   where pp.Active && pc.Active && pp.GuidCategory == models.PosCategory.Guid
                                                   select new PosProduceModel
                                                   {
                                                       Guid = pp.Guid,
                                                       Code = pp.Code,
                                                       Name = pp.Name,
                                                       NameCategory = pc.Name,
                                                       CategorySeoFriendUrl = pc.SeoFriendUrl,
                                                       SeoFriendUrl = pp.SeoFriendUrl,
                                                       Price = pp.Price,
                                                       Unit = pp.Unit,
                                                       ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                      join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                      where fr.GuidProduce == pp.Guid
                                                                      select new UGalleryModel
                                                                      {
                                                                          Image = im.Image,
                                                                          CreatedAt = im.CreatedAt
                                                                      }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                       CreatedAt = pp.CreatedAt,
                                                       Legit = pp.Legit,
                                                       LegitCount = pp.LegitCount
                                                   };

                    if (!string.IsNullOrEmpty(sortBy))
                    {
                        switch (sortBy)
                        {
                            case "newest":
                                listPosProduceByCategory = listPosProduceByCategory.OrderByDescending(x => x.CreatedAt);
                                break;

                            case "oldest":
                                listPosProduceByCategory = listPosProduceByCategory.OrderBy(x => x.CreatedAt);
                                break;

                            case "a-z":
                                listPosProduceByCategory = listPosProduceByCategory.OrderBy(x => x.Name);
                                break;

                            case "z-a":
                                listPosProduceByCategory = listPosProduceByCategory.OrderByDescending(x => x.Name);
                                break;

                            case "priceH-L":
                                listPosProduceByCategory = listPosProduceByCategory.OrderByDescending(x => x.Price);
                                break;

                            case "priceL-H":
                                listPosProduceByCategory = listPosProduceByCategory.OrderBy(x => x.Price);
                                break;

                            default:
                                listPosProduceByCategory = listPosProduceByCategory.OrderByDescending(x => x.CreatedAt);
                                break;
                        }
                    }
                    else
                    {
                        listPosProduceByCategory = listPosProduceByCategory.OrderByDescending(x => x.CreatedAt);
                    }

                    //Sent data to view caculate
                    ViewData["perpage"] = recordMax;
                    ViewData["total"] = listPosProduceByCategory.Count();
                    ViewData["pageActive"] = numberPageActive;

                    //Check page start
                    if (Convert.ToInt16(arrPage[1]) == 1)
                    {
                        numberPage = 0;
                    }
                    else
                    {
                        numberPage = numberPage - recordMax;
                    }

                    models.ListPosProduceByCategory = listPosProduceByCategory.Skip(numberPage).Take(recordMax).ToList();

                    foreach (var item in models.ListPosProduceByCategory)
                    {
                        var tmpLinkImg = item.ListGallery;
                        if (tmpLinkImg.Count == 0)
                        {
                            item.LinkImg0 = "/Content/Custom/empty-album.png";
                        }
                        else
                        {
                            item.LinkImg0 = tmpLinkImg[0].Image;
                        }
                    }

                    return View("~/Views/HomeCommon/ProduceListByCategory.cshtml", models);
                }
                else
                {
                    return View("~/Views/HomeCommon/Error_404.cshtml");
                }
            }

            return View("~/Views/HomeCommon/Error_404.cshtml");
        }

        #endregion Produce List By Category

        #region Produce Detail

        public ActionResult ProduceDetail(string seoFriendUrl)
        {
            ViewBag.CurrentUsername = GetUserInSession();

            if (!string.IsNullOrEmpty(seoFriendUrl))
            {
                var models = new ProduceDetailModel();

                models.SUser = GetSettingUser();

                var dataCat = (from cat in DataGemini.PosCategories
                               where cat.Active
                               select new PosCategoryModel
                               {
                                   SeoFriendUrl = cat.SeoFriendUrl,
                                   Name = cat.Name,
                                   OrderBy = cat.OrderBy,
                                   Guid = cat.Guid,
                                   ParentGuid = cat.ParentGuid
                               }).OrderBy(x => x.OrderBy).ToList();

                var roots = BuildTreeCategory(dataCat);
                foreach (var item in roots)
                {
                    AppendCharsCategory(item, string.Empty, string.Empty);
                }
                models.ListPosCategory = roots.OrderBy(x => x.RootId).ThenBy(x => x.OrderBy).Where(x => x.ParentGuid == null || x.ParentGuid == Guid.Empty).ToList();

                models.PosProduce = (from pp in DataGemini.PosProduces
                                     join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                     join pb in DataGemini.PosBatches on pp.GuidBatch equals pb.Guid
                                     where pp.Active && pp.SeoFriendUrl.ToLower().Trim() == seoFriendUrl
                                     select new PosProduceModel
                                     {
                                         Guid = pp.Guid,
                                         Code = pp.Code,
                                         Name = pp.Name,
                                         Status = pp.Status,
                                         Size = pp.Size,
                                         Color = pp.Color,
                                         ListGallery = (from fr in DataGemini.FProduceGalleries
                                                        join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                        where fr.GuidProduce == pp.Guid
                                                        select new UGalleryModel
                                                        {
                                                            Image = im.Image
                                                        }).ToList(),
                                         Legit = pp.Legit,
                                         LegitCount = pp.LegitCount,
                                         Price = pp.Price,
                                         Unit = pp.Unit,
                                         Note = pp.Note,
                                         Description = pp.Description,
                                         GuidCategory = pp.GuidCategory,
                                         CreatedBy = pp.CreatedBy,
                                         NameBatch = pb.Name,
                                         NameCategory = pc.Name,
                                         CategorySeoFriendUrl = pc.SeoFriendUrl
                                     }).FirstOrDefault();

                if (models.PosProduce != null)
                {
                    var linkImg = models.PosProduce.ListGallery;
                    if (linkImg.Count == 0)
                    {
                        models.PosProduce.LinkImg0 = "/Content/Custom/empty-album.png";
                    }
                    else
                    {
                        models.PosProduce.LinkImg0 = linkImg[0].Image;
                    }

                    models.PosProduceCreatedBy = DataGemini.SUsers.FirstOrDefault(x => x.Username == models.PosProduce.CreatedBy);
                    if (models.PosProduceCreatedBy != null && string.IsNullOrEmpty(models.PosProduceCreatedBy.Avartar))
                    {
                        models.PosProduceCreatedBy.Avartar = DefaultImage.ImageEmpty;
                    }

                    models.ListProduceSameCreatedBy = (from pp in DataGemini.PosProduces
                                                       join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                                       where pp.Active && pc.Active && pp.CreatedBy == models.PosProduce.CreatedBy && pp.Guid != models.PosProduce.Guid
                                                       select new PosProduceModel
                                                       {
                                                           Guid = pp.Guid,
                                                           Name = pp.Name,
                                                           NameCategory = pc.Name,
                                                           CategorySeoFriendUrl = pc.SeoFriendUrl,
                                                           SeoFriendUrl = pp.SeoFriendUrl,
                                                           Price = pp.Price,
                                                           Unit = pp.Unit,
                                                           ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                          join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                          where fr.GuidProduce == pp.Guid
                                                                          select new UGalleryModel
                                                                          {
                                                                              Image = im.Image,
                                                                              CreatedAt = im.CreatedAt
                                                                          }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                           CreatedAt = pp.CreatedAt,
                                                           CreatedBy = pp.CreatedBy,
                                                           Legit = pp.Legit,
                                                           LegitCount = pp.LegitCount
                                                       }).OrderByDescending(s => s.CreatedAt).Take(4).ToList();

                    foreach (var item in models.ListProduceSameCreatedBy)
                    {
                        var tmpLinkImg = item.ListGallery;
                        if (tmpLinkImg.Count == 0)
                        {
                            item.LinkImg0 = "/Content/Custom/empty-album.png";
                        }
                        else
                        {
                            item.LinkImg0 = tmpLinkImg[0].Image;
                        }
                    }

                    models.ListProduceSameCategory = (from pp in DataGemini.PosProduces
                                                      join pc in DataGemini.PosCategories on pp.GuidCategory equals pc.Guid
                                                      where pp.Active && pc.Active && pp.GuidCategory == models.PosProduce.GuidCategory && pp.Guid != models.PosProduce.Guid
                                                      select new PosProduceModel
                                                      {
                                                          Guid = pp.Guid,
                                                          Name = pp.Name,
                                                          NameCategory = pc.Name,
                                                          CategorySeoFriendUrl = pc.SeoFriendUrl,
                                                          SeoFriendUrl = pp.SeoFriendUrl,
                                                          Price = pp.Price,
                                                          Unit = pp.Unit,
                                                          ListGallery = (from fr in DataGemini.FProduceGalleries
                                                                         join im in DataGemini.UGalleries on fr.GuidGallery equals im.Guid
                                                                         where fr.GuidProduce == pp.Guid
                                                                         select new UGalleryModel
                                                                         {
                                                                             Image = im.Image,
                                                                             CreatedAt = im.CreatedAt
                                                                         }).OrderBy(x => x.CreatedAt).Take(1).ToList(),
                                                          CreatedAt = pp.CreatedAt,
                                                          CreatedBy = pp.CreatedBy,
                                                          Legit = pp.Legit,
                                                          LegitCount = pp.LegitCount
                                                      }).OrderByDescending(s => s.CreatedAt).Take(4).ToList();

                    foreach (var item in models.ListProduceSameCategory)
                    {
                        var tmpLinkImg = item.ListGallery;
                        if (tmpLinkImg.Count == 0)
                        {
                            item.LinkImg0 = "/Content/Custom/empty-album.png";
                        }
                        else
                        {
                            item.LinkImg0 = tmpLinkImg[0].Image;
                        }
                    }

                    models.NewRatingProduce = new WRatingProduceModel()
                    {
                        GuidProduce = models.PosProduce.Guid
                    };

                    return View("~/Views/HomeCommon/ProduceDetail.cshtml", models);
                }
                else
                {
                    return View("~/Views/HomeCommon/Error_404.cshtml");
                }
            }

            return View("~/Views/HomeCommon/Error_404.cshtml");
        }

        public ActionResult PartialProduceDetailRating(string page, string seoFriendUrl)
        {
            ViewBag.CurrentUsername = GetUserInSession();

            int recordMax = 3;
            page = page ?? "page-1";
            string[] arrPage = page.Split('-');
            int numberPage = Convert.ToInt16(arrPage[1]) * recordMax;
            int numberPageActive = Convert.ToInt16(arrPage[1]);

            var model = new PartialProduceDetailRatingModel();

            var arrUrl = Request.Path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (arrUrl != null && arrUrl.Length > 1)
            {
                seoFriendUrl = string.IsNullOrEmpty(seoFriendUrl) ? arrUrl[1] : seoFriendUrl;

                var listRatingProduce = (from rp in DataGemini.WRatingProduces
                                         join pp in DataGemini.PosProduces on rp.GuidProduce equals pp.Guid
                                         where pp.SeoFriendUrl == seoFriendUrl && pp.Active
                                         select new WRatingProduceModel
                                         {
                                             Guid = rp.Guid,
                                             FullName = rp.FullName,
                                             Comment = rp.Comment,
                                             CreatedAt = rp.CreatedAt,
                                             Avatar = rp.Avatar,
                                             Legit = rp.Legit
                                         }).OrderByDescending(s => s.CreatedAt);

                //Sent data to view caculate
                ViewData["perpage"] = recordMax;
                ViewData["total"] = listRatingProduce.Count();
                ViewData["pageActive"] = numberPageActive;

                //Check page start
                if (Convert.ToInt16(arrPage[1]) == 1)
                {
                    numberPage = 0;
                }
                else
                {
                    numberPage = numberPage - recordMax;
                }

                model.ListRatingProduce = listRatingProduce.Skip(numberPage).Take(recordMax).ToList();
            }

            return PartialView(model);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RatingProduce(ProduceDetailModel model)
        {
            var rating = new WRatingProduce();
            try
            {
                if (model.NewRatingProduce.GuidProduce != null && model.NewRatingProduce.GuidProduce != Guid.Empty)
                {
                    model.NewRatingProduce.Setvalue(rating);
                    var user = GetSettingUser();
                    rating.FullName = user != null && !string.IsNullOrEmpty(user.Username) ? user.Username : rating.FullName;
                    rating.Mobile = user != null && !string.IsNullOrEmpty(user.Mobile) ? user.Mobile : rating.Mobile;
                    rating.Email = user != null && !string.IsNullOrEmpty(user.Email) ? user.Email : rating.Email;
                    rating.Avatar = user != null && !string.IsNullOrEmpty(user.Avartar) ? user.Avartar : DefaultImage.ImageEmpty;
                    rating.UpdatedAt = rating.CreatedAt = DateTime.Now;
                    rating.UpdatedBy = rating.CreatedBy = rating.FullName;

                    if (IsValidEmail(rating.Email) && IsValidMobile(rating.Mobile) && model.NewRatingProduce.Legit > 0)
                    {
                        DataGemini.WRatingProduces.Add(rating);

                        if (SaveData("WRatingProduce") && rating != null)
                        {
                            var posProduce = DataGemini.PosProduces.FirstOrDefault(x => x.Guid == rating.GuidProduce.Value);
                            var lstRating = DataGemini.WRatingProduces.Where(x => x.GuidProduce == posProduce.Guid).ToList();
                            posProduce.LegitCount = lstRating.Count;
                            posProduce.Legit = (int)Math.Round((decimal)lstRating.Sum(x => x.Legit) / (decimal)posProduce.LegitCount, 0);

                            if (SaveData("PosProduce") && posProduce != null)
                            {
                                var sUser = DataGemini.SUsers.FirstOrDefault(x => x.Username == posProduce.CreatedBy);
                                var lstProduce = DataGemini.PosProduces.Where(x => x.LegitCount > 0 && x.CreatedBy == posProduce.CreatedBy);
                                sUser.LegitCount = lstProduce.Sum(x => x.LegitCount);
                                sUser.Legit = (int)Math.Round((decimal)lstProduce.Sum(x => x.Legit * x.LegitCount) / (decimal)sUser.LegitCount, 0);

                                if (SaveData("SUser") && sUser != null)
                                {
                                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                                }
                                else
                                {
                                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                                    DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                                }
                            }
                            else
                            {
                                DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                                DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                            }
                        }
                        else
                        {
                            DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                            DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                        }
                    }
                    else
                    {
                        DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                        DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                    }
                }
                else
                {
                    DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                    DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                DataGemini.WRatingProduces.Remove(rating);

                DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
            }
            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        private bool IsValidEmail(string emailaddress)
        {
            try
            {
                if (string.IsNullOrEmpty(emailaddress))
                    return false;

                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private bool IsValidMobile(string mobile)
        {
            try
            {
                if (string.IsNullOrEmpty(mobile))
                    return false;

                var r = new Regex(@"^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$");

                return r.IsMatch(mobile);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        #endregion Produce Detail

        #region User List By Follow

        public ActionResult UserListByFollow(string page)
        {
            ViewBag.CurrentUsername = GetUserInSession();

            int recordMax = 9;
            page = page ?? "page-1";
            string[] arrPage = page.Split('-');
            int numberPage = Convert.ToInt16(arrPage[1]) * recordMax;
            int numberPageActive = Convert.ToInt16(arrPage[1]);

            var models = new UserListByFollowModel();
            models.ListUserFollow = new List<SUserModel>();

            var dataCat = (from cat in DataGemini.PosCategories
                           where cat.Active
                           select new PosCategoryModel
                           {
                               SeoFriendUrl = cat.SeoFriendUrl,
                               Name = cat.Name,
                               OrderBy = cat.OrderBy,
                               Guid = cat.Guid,
                               ParentGuid = cat.ParentGuid
                           }).OrderBy(x => x.OrderBy).ToList();

            var roots = BuildTreeCategory(dataCat);
            foreach (var item in roots)
            {
                AppendCharsCategory(item, string.Empty, string.Empty);
            }
            models.ListPosCategory = roots.OrderBy(x => x.RootId).ThenBy(x => x.OrderBy).Where(x => x.ParentGuid == null || x.ParentGuid == Guid.Empty).ToList();

            var username = GetUserInSession();
            var currentUser = DataGemini.SUsers.FirstOrDefault(x => x.Username == username);
            if (currentUser != null)
            {
                var lstGuidFollowString = currentUser.GuidFollow ?? string.Empty;
                var listUserListByFollow = (from su in DataGemini.SUsers
                                            where su.Active && lstGuidFollowString.Contains(su.Guid.ToString().ToLower().Trim())
                                            select new SUserModel
                                            {
                                                Guid = su.Guid,
                                                CreatedAt = su.CreatedAt,
                                                Avartar = su.Avartar,
                                                FullName = su.FullName,
                                                Username = su.Username,
                                                Note = su.Note,
                                                Legit = su.Legit,
                                                LegitCount = su.LegitCount,
                                            }).OrderByDescending(x => x.CreatedAt);

                //Sent data to view caculate
                ViewData["perpage"] = recordMax;
                ViewData["total"] = listUserListByFollow.Count();
                ViewData["pageActive"] = numberPageActive;

                //Check page start
                if (Convert.ToInt16(arrPage[1]) == 1)
                {
                    numberPage = 0;
                }
                else
                {
                    numberPage = numberPage - recordMax;
                }

                models.ListUserFollow = listUserListByFollow.Skip(numberPage).Take(recordMax).ToList();

                foreach (var item in models.ListUserFollow)
                {
                    item.Avartar = !string.IsNullOrEmpty(item.Avartar) ? item.Avartar : DefaultImage.ImageEmpty;
                }

                return View("~/Views/HomeCommon/UserListByFollow.cshtml", models);
            }
            else
            {
                return View("~/Views/HomeCommon/Error_404.cshtml");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GetFollowUser()
        {
            var username = GetUserInSession();
            var user = DataGemini.SUsers.FirstOrDefault(x => x.Username == username);

            if (user != null)
            {
                return Json(new { guidFollow = user.GuidFollow });
            }
            else
            {
                return Json(new { guidFollow = string.Empty });
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FollowUser(FollowUserModel model)
        {
            var username = GetUserInSession();
            var user = DataGemini.SUsers.FirstOrDefault(x => x.Username == username);
            try
            {
                if (user != null && user.Guid != null && user.Guid != Guid.Empty)
                {
                    if (!string.IsNullOrEmpty(user.GuidFollow) && user.GuidFollow.Contains(model.guidUser))
                    {
                        user.GuidFollow = user.GuidFollow.Replace(model.guidUser + ",", "");
                    }
                    else
                    {
                        user.GuidFollow += model.guidUser + ",";
                    }

                    if (SaveData("SUser"))
                    {
                        DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.OK);
                    }
                    else
                    {
                        DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                        DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
                    }
                }
                else
                {
                    return Json(new { url = "/admin" });
                }
            }
            catch (Exception ex)
            {
                DataReturn.StatusCode = Convert.ToInt16(HttpStatusCode.BadRequest);
                DataReturn.MessagError = Constants.CannotUpdate + " Date : " + DateTime.Now;
            }
            return Json(DataReturn, JsonRequestBehavior.AllowGet);
        }

        #endregion User List By Follow

        #region Contact Us

        public ActionResult ContactUs()
        {
            var models = new ContactUsModel();

            var dataCat = (from cat in DataGemini.PosCategories
                           where cat.Active
                           select new PosCategoryModel
                           {
                               SeoFriendUrl = cat.SeoFriendUrl,
                               Name = cat.Name,
                               OrderBy = cat.OrderBy,
                               Guid = cat.Guid,
                               ParentGuid = cat.ParentGuid
                           }).OrderBy(x => x.OrderBy).ToList();

            var roots = BuildTreeCategory(dataCat);
            foreach (var item in roots)
            {
                AppendCharsCategory(item, string.Empty, string.Empty);
            }
            models.ListPosCategory = roots.OrderBy(x => x.RootId).ThenBy(x => x.OrderBy).Where(x => x.ParentGuid == null || x.ParentGuid == Guid.Empty).ToList();

            models.Address = DataGemini.WOneParams.FirstOrDefault(x => x.Code == WOneParam_Code.Address)?.Description;

            return View(models);
        }

        #endregion Contact Us

        #region Change Password

        [HttpGet]
        public ActionResult ChangePassword()
        {
            ViewBag.CurrentUsername = GetUserInSession();

            UserChangePasswordModel model = new UserChangePasswordModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePassword(UserChangePasswordModel model)
        {
            var username = GetUserInSession();
            ViewBag.CurrentUsername = username;

            string msg = "";
            int statusCode = (int)Convert.ToInt16(HttpStatusCode.Conflict);
            try
            {
                if (!string.IsNullOrWhiteSpace(model.Password) && !string.IsNullOrWhiteSpace(model.PasswordAgain))
                {
                    if (!model.Password.Equals(model.PasswordAgain))
                    {
                        msg = "Mật khẩu không trùng nhau";
                    }
                    else
                    {
                        var user = DataGemini.SUsers.FirstOrDefault(x => x.Username == username);
                        if (user == null)
                        {
                            msg = "Có lỗi xảy ra. Vui lòng thử lại sau ít phút";
                        }
                        else if (user.Password != Encrypt(model.OldPassword))
                        {
                            msg = "Mật khẩu cũ không chính xác";
                        }
                        else
                        {
                            user.Password = Encrypt(model.Password);
                            user.UpdatedBy = user.Username;
                            user.UpdatedAt = DateTime.Now;

                            DataGemini.SaveChanges();

                            statusCode = (int)Convert.ToInt16(HttpStatusCode.OK);
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(model.PasswordAgain))
                {
                    msg = "Mật khẩu mới không được bỏ trống";
                }
                else if (model.PasswordAgain.Length < 6)
                {
                    msg = "Mật khẩu mới không được ngắn hơn 6 ký tự";
                }

                if (string.IsNullOrWhiteSpace(model.Password))
                {
                    msg = "Mật khẩu mới không được bỏ trống";
                }
                else if (model.Password.Length < 6)
                {
                    msg = "Mật khẩu mới không được ngắn hơn 6 ký tự";
                }

                if (string.IsNullOrWhiteSpace(model.OldPassword))
                {
                    msg = "Mật khẩu cũ không được bỏ trống";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return Json(new { StatusCode = statusCode, Message = msg }, "text/plain");
        }

        #endregion Change Password

        #region User Profile

        public static void VerifyDir(string path)
        {
            try
            {
                var list = path.Split(new string[] { "\\" }, StringSplitOptions.None);
                var directory = path.Replace("\\" + list[list.Count() - 1], "");
                DirectoryInfo dir = new DirectoryInfo(directory);
                if (!dir.Exists)
                {
                    dir.Create();
                }
            }
            catch { }
        }

        public static void WriteFileFromStream(Stream stream, string toFile)
        {
            using (FileStream fileToSave = new FileStream(toFile, FileMode.Create))
            {
                stream.CopyTo(fileToSave);
            }
        }

        [HttpPost]
        public ActionResult MyProfile(SUser model, HttpPostedFileBase avatar)
        {
            var username = GetUserInSession();
            ViewBag.CurrentUsername = username;
            if (string.IsNullOrEmpty(username))
            {
                return View("~/Views/HomeCommon/Error_404.cshtml");
            }

            var user = DataGemini.SUsers.FirstOrDefault(x => x.Username == username);
            try
            {
                user.FullName = model.FullName;
                user.Mobile = model.Mobile;
                user.FullAddress = model.FullAddress;
                user.Skype = model.Skype;
                user.Facebook = model.Facebook;
                user.Skype = model.Skype;
                var img = "";
                if (avatar != null)
                {
                    img = user.Avartar;
                    var nameFile = Path.GetFileName(avatar.FileName);
                    var g = Guid.NewGuid();

                    var physicalPath = Path.Combine(Server.MapPath("~/Content/UserFiles/Images/User/" + g + "/"), nameFile);

                    VerifyDir(physicalPath);
                    WriteFileFromStream(avatar.InputStream, physicalPath);

                    user.Avartar = "/Content/UserFiles/Images/User/" + g + "/" + nameFile;
                }
                DataGemini.SUsers.AddOrUpdate(user);
                DataGemini.SaveChanges();
                if (!string.IsNullOrEmpty(img))
                {
                    if (System.IO.File.Exists(img))
                    {
                        System.IO.File.Delete(img);
                    }
                }
                ViewBag.Message = "Cập nhật thông tin thành công";
                return View(user);
            }
            catch
            {
                ViewBag.Message = "Cập nhật thất bại";
                return View(user);
            }
        }

        [HttpGet]
        public ActionResult MyProfile()
        {
            var username = GetUserInSession();
            ViewBag.CurrentUsername = username;
            if (string.IsNullOrEmpty(username))
            {
                return View("~/Views/HomeCommon/Error_404.cshtml");
            }

            var user = DataGemini.SUsers.FirstOrDefault(x => x.Username == username);

            return View(user);
        }

        #endregion User Profile

        public ActionResult Error_404()
        {
            ViewBag.Title = "";
            ViewBag.Description = "";

            return View();
        }
        [HttpPost]
        public ActionResult GetInventory(Guid guidProduce, string size, string color)
        {
            try
            {
                var inventory = DataGemini.PosInventories.FirstOrDefault(x =>
                    x.GuidProduce == guidProduce &&
                    x.Size == size &&
                    x.Color == color);

                if (inventory != null)
                {
                    return Json(new { StatusCode = 200, Quantity = inventory.Quantity }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { StatusCode = 200, Quantity = 0 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { StatusCode = 500, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetProductInventory(Guid guidProduce)
        {
            try
            {
                var inventories = DataGemini.PosInventories
                    .Where(x => x.GuidProduce == guidProduce)
                    .Select(x => new
                    {
                        x.Size,
                        x.Color,
                        x.Quantity
                    })
                    .ToList();

                return Json(new { StatusCode = 200, Data = inventories }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { StatusCode = 500, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}