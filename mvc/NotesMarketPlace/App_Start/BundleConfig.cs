using System.Web;
using System.Web.Optimization;

namespace NotesMarketPlace
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/js").Include(
                      "~/Scripts/jquery-3.5.1.min.js",
                      "~/Scripts/bootstrap/bootstrap.bundle.min.js",
                      "~/Scripts/sweetalert.min.js",
                      "~/Scripts/DataTables/jquery.dataTables.js",
                      "~/Scripts/DataTables/dataTables.bootstrap.js",
                      "~/Scripts/pagination.js",
                      "~/Scripts/script.js"));

            bundles.Add(new ScriptBundle("~/bundles/headerjs").Include(
                      "~/Scripts/header.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/CSS/bootstrap/bootstrap.min.css",
                      "~/Content/CSS/font-awesome/css/font-awesome.min.css",
                      "~/Content/DataTables/css/dataTables.bootstrap.css",
                      "~/Content/CSS/header.css",
                      "~/Content/CSS/footer.css",
                      "~/Content/CSS/table.css",
                      "~/Content/CSS/pagination.css",
                      "~/Content/CSS/collapse.css",
                      "~/Content/CSS/style.css",
                      "~/Content/CSS/responsive.css"));
        }
    }
}
