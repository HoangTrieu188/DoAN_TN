using Gemini.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Gemini
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");



            try
            {

                #region Add portal admin in router
                var db = new GeminiEntities();

                routes.MapRoute(
                    name: "GetActiveQr",
                    url: "get-active-qr",
                    defaults: new { controller = "HomeCommon", action = "GetActiveQr" }
                );

                routes.MapRoute(
                    name: "admin",
                    url: "admin" + "/{controller}/{action}/{id}/{Menu}",
                    defaults: new { Portal = "admin", controller = "Admin", action = "Index", id = UrlParameter.Optional, Menu = "start" }
                );
                #endregion

                #region Web
                routes.MapRoute(
                    name: "DoiMatKhau",
                    url: "doi-mat-khau",
                    defaults: new { controller = "HomeCommon", action = "ChangePassword" }
                );


                routes.MapRoute(
                    name: "TaiKhoan",
                    url: "tai-khoan",
                    defaults: new { controller = "HomeCommon", action = "MyProfile" }
                );

                routes.MapRoute(
                    name: "TheoDoi",
                    url: "theo-doi",
                    defaults: new { controller = "HomeCommon", action = "UserListByFollow" }
                );

                routes.MapRoute(
                    name: "SanPhamDaLuu",
                    url: "san-pham-da-luu",
                    defaults: new { controller = "HomeCommon", action = "ProduceListByLoved" }
                );

                routes.MapRoute(
                    name: "VnPayReturn",
                    url: "payment/vnpay-payment",
                    defaults: new { controller = "HomeCommon", action = "VnPayReturn" }
                );

                routes.MapRoute(
                    name: "GioHang",
                    url: "gio-hang",
                    defaults: new { controller = "HomeCommon", action = "ProduceCart" }
                );

                routes.MapRoute(
                    name: "TimKiem",
                    url: "tim-kiem",
                    defaults: new { controller = "HomeCommon", action = "ProduceListBySearch" }
                );

                routes.MapRoute(
                    name: "CuaHang",
                    url: "cua-hang/{createdBy}",
                    defaults: new { controller = "HomeCommon", action = "ProduceListByCreatedBy", createdBy = UrlParameter.Optional }
                );

                routes.MapRoute(
                    name: "DanhMucSanPham",
                    url: "danh-muc/{seoFriendUrl}",
                    defaults: new { controller = "HomeCommon", action = "ProduceListByCategory", seoFriendUrl = UrlParameter.Optional }
                );

                routes.MapRoute(
                    name: "ChiTietSanPham",
                    url: "san-pham/{seoFriendUrl}",
                    defaults: new { controller = "HomeCommon", action = "ProduceDetail", seoFriendUrl = UrlParameter.Optional }
                );

                routes.MapRoute(
                    name: "LienHe",
                    url: "lien-he",
                    defaults: new { controller = "HomeCommon", action = "ContactUs" }
                );

                var sMenus = db.SMenus.Where(p => p.Type == "WEB").ToList();

                foreach (SMenu itemMenu in sMenus)
                {
                    bool isAdd = true;
                    string routeUrl = (itemMenu.RouterUrl);
                    string name = (itemMenu.Guid.ToString());
                    string url = (itemMenu.LinkUrl);
                    if (!string.IsNullOrEmpty(routeUrl))
                    {
                        //Kiem tra trung
                        foreach (Route route in routes)
                        {
                            if ((url == route.Url))
                            {
                                isAdd = false;
                            }
                        }
                        //Add route neu khong trung
                        if (isAdd)
                        {
                            var routerDic = new RouteValueDictionary();
                            string[] arr = routeUrl.Split(';');
                            foreach (string item in arr)
                            {
                                if (!string.IsNullOrEmpty(item))
                                {
                                    string[] arr1 = item.Split('=');
                                    if (arr1.Length > 1)
                                    {
                                        routerDic.Add(arr1[0], arr1[1]);
                                    }
                                }

                            }

                            routes.MapRoute(
                                name: name,
                                url: url,
                                defaults: routerDic
                            );
                        }
                    }
                }
                #endregion
            }
            catch
            {
                // Handle exceptions if necessary, or leave empty if not needed.
            }

            #region Router defaults
            //==================================================================//
            // Router cho trang chu va cac PartialView
            // Default route moved to top
            #endregion
            // Router cho trang chu va cac PartialView
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}/{id1}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, id1 = UrlParameter.Optional }
            );

        }
    }
}