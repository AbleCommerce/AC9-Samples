using CommerceBuilder.Web.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace ExamplePaymentPlugin
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            if (routes["Plugins_Payment_ExamplePaymentPlugin_Admin"] == null)
            {
                var route = routes.MapRoute(
                    "Plugins_Payment_ExamplePaymentPlugin_Admin",
                    "Admin/Payments/ExamplePaymentPlugin/{action}",
                    new { controller = "EPPAdmin", action = "Config" },
                    new[] { "ExamplePaymentPlugin.Controllers" }
                );

                route.DataTokens["area"] = "Admin";
            }

            if (routes["Plugins_Payment_ExamplePaymentPlugin_Retail"] == null)
            {
                var route = routes.MapRoute(
                    "Plugins_Payment_ExamplePaymentPlugin_Retail",
                    "EPPRetail/{action}",
                    new { controller = "EPPRetail" },
                    new[] { "ExamplePaymentPlugin.Controllers" }
                );
            }
        }
    }
}
