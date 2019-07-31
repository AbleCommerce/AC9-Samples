using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CommerceBuilder.Web;
using CommerceBuilder.Web.Mvc;
using ExamplePaymentPlugin.Models;

namespace ExamplePaymentPlugin.Controllers
{
    public class EPPRetailController : AbleController
    {
        [RegisterWidget]
        public ActionResult ExamplePaymentButton()
        {
            var model = new ExamplePaymentButtonModel() { OrderId = 123, Amount = 10 };
            return PartialView("~/Plugins/ExamplePaymentPlugin/Views/Retail/_ExamplePaymentButton.cshtml", model);
        }
    }
}
