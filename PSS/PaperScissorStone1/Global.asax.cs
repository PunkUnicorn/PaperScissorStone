﻿using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PaperScissorStone1
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            MefConfig.Compose();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
