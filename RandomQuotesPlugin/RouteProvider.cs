using CommerceBuilder.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace RandomQuotesPlugin
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            if (routes["Plugins_General_RandomQuotesPlugin_Admin"] == null)
            {
                var route = routes.MapRoute(
                    "Plugins_General_RandomQuotesPlugin_Admin",
                    "Admin/RandomQuotes/{action}",
                    new { controller = "RQPAdmin", action = "Index" },
                    new[] { "RandomQuotesPlugin.Controllers" }
                );

                route.DataTokens["area"] = "Admin";
            }

            if (routes["Plugins_General_RandomQuotesPlugin_Retail"] == null)
            {
                var route = routes.MapRoute(
                    "Plugins_General_RandomQuotesPlugin_Retail",
                    "RQPRetail/{action}",
                    new { controller = "EPPRetail" },
                    new[] { "RandomQuotesPlugin.Controllers" }
                );
            }
        }
    }
}
