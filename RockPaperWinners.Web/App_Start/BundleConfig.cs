using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Transformers;
using System.Web;
using System.Web.Optimization;

namespace RockPaperWinners.Web
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/js/jquery").Include(
                        "~/public/js/lib/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/js/jqueryval").Include(
                        "~/public/js/lib/jquery.unobtrusive*",
                        "~/public/js/lib/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/js/bootstrap")
                .Include("~/public/js/lib/bootstrap.js")
                .Include("~/public/js/lib/bootstrap-mvc.js"));

            var css = new Bundle("~/bundles/css/site");
            css.Include("~/public/css/bootstrap.css")
                .Include("~/public/css/bootstrap-responsive.css")
                .Include("~/public/css/site.less");
            css.Transforms.Add(new CssTransformer());
            css.Orderer = new NullOrderer();
            bundles.Add(css);
        }
    }
}