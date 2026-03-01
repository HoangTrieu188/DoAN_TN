using System.Web.Optimization;

namespace Gemini
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/kendo").Include(
                        "~/Scripts/kendo/kendo.all.min.js",
                        "~/Scripts/kendo/kendo.timezones.min.js", // uncomment if using the Scheduler
                        "~/Scripts/kendo/kendo.aspnetmvc.min.js"));


            bundles.Add(new StyleBundle("~/Content/kendo/css").Include(
                        "~/Content/kendo/kendo.common.min.css",
                        "~/Content/kendo/kendo.default.min.css"));


            bundles.Add(new StyleBundle("~/Content/themecss").Include(
                        "~/assets/theme/css/bootstrap.min.css",
                        "~/assets/theme/css/elegant-icons.css",
                        "~/assets/theme/css/font-awesome.min.css",
                        "~/assets/theme/css/jquery-ui.min.css",
                        "~/assets/theme/css/nice-select.css",
                        "~/assets/theme/css/owl.carousel.min.css",
                        "~/assets/theme/css/slicknav.min.css",
                        "~/assets/theme/css/style.css"));


            bundles.Add(new ScriptBundle("~/bundles/themejs").Include(
                        "~/assets/theme/js/bootstrap.min.js",
                        "~/assets/theme/js/jquery.nice-select.min.js",
                        "~/assets/theme/js/jquery-ui.min.js",
                        "~/assets/theme/js/jquery.slicknav.js",
                        "~/assets/theme/js/owl.carousel.min.js",
                        "~/assets/theme/js/mixitup.min.js",
                        "~/assets/theme/js/main.js"));


            bundles.IgnoreList.Clear();
            BundleTable.EnableOptimizations = true;
        }
    }
}