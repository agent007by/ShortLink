using System;
using System.Web.Http;
using System.Web.Optimization;
using System.Web.Routing;
using LinksFactory;
using System.Threading.Tasks;

namespace Bitly
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_End(object sender, EventArgs e)
        {
            Task.FromResult(LinkStatisticCounter.SaveData());
        }
    }
}
