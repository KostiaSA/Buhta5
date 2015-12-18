
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

[assembly: OwinStartup(typeof(Buhta.Startup))]

namespace Buhta
{

    //public class CustomViewLocationRazorViewEngine : RazorViewEngine
    //{
    //    public CustomViewLocationRazorViewEngine()
    //    {
    //        ViewLocationFormats = new[]
    //        {
    //          "~/MODULES/BUHTA/CORE/{0}.cshtml",
    //          "~/MODULES/BUHTA/CORE/HOME-PAGE/{0}.cshtml",
    //          "~/Views/{0}.cshtml",
    //          "~/RazorViews/Common/{0}.cshtml"
    //        };

    //        MasterLocationFormats = new[]
    //        {
    //          "~/RazorViews/{1}/{0}.cshtml",
    //          "~/RazorViews/Common/{0}.cshtml"
    //        };

    //        PartialViewLocationFormats = new[]
    //        {
    //          "~/RazorViews/{1}/{0}.cshtml",
    //          "~/RazorViews/Common/{0}.cshtml"
    //        };
    //    }
    //}

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            //ViewEngines.Engines.Clear();
            //var viewEngine = new CustomViewLocationRazorViewEngine();
            //ViewEngines.Engines.Add(viewEngine);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            App.Start();
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
