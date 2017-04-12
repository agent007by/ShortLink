using System.Web;
using System.Web.Optimization;

namespace Bitly
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            #region JS

            #region Vendors

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                      "~/Scripts/knockout/js/knockout-{version}.js",
                      "~/Scripts/knockout/js/knockout.mapping-latest.js",
                      "~/Scripts/knockout/js/knockout.validation.js",
                      "~/Scripts/knockout/js/knockout.validation.bootstrap.js"));
            #endregion

            bundles.Add(new ScriptBundle("~/bundles/app/master").Include(
                        "~/app/modules.js",
                        "~/app/urls.js",
                        "~/app/run.js"));

            bundles.Add(new ScriptBundle("~/bundles/app/shortLink").Include(
                     "~/app/shortLink/js/MainViewModel.js",
                     "~/app/shortLink/js/app.js"));

            bundles.Add(new ScriptBundle("~/bundles/app/shortLinksStatistics").Include(
                     "~/app/shortLinksStatistics/js/MainViewModel.js",
                     "~/app/shortLinksStatistics/js/app.js"));
            #endregion

            #region CSS
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
            #endregion
            // Включить упаковку и минификацию.
            BundleTable.EnableOptimizations = true;
#if DEBUG
            BundleTable.EnableOptimizations = false;
#endif

        }
    }
}
