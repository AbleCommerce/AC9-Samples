using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static ExamplePaymentPlugin.EPProvider;

namespace ExamplePaymentPlugin.Models
{
    public class ConfigModel
    {
        public ConfigModel()
        {
            UseDebugMode = false;
            UseAuthCapture = false;
            ExecutionMode = GatewayExecutionMode.AlwaysAccept;
            PaymentMethods = new List<PaymentMethodModel>();
        }

        public int? GatewayId { get; set; }
        
        public bool UseDebugMode { get; set; }
        
        public bool UseAuthCapture { get; set; }
        
        public GatewayExecutionMode ExecutionMode { get; set; }

        public List<SelectListItem> ExecutionModes { get; set; }

        public List<PaymentMethodModel> PaymentMethods { get; set; }

        public string AssemblyInfo { get; set; }

        public string SaveOnly { get; set; }

        public bool ShowConfirmationMessage { get; set; }
    }

    public class PaymentMethodModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsSelected { get; set; }
    }

    public class ExamplePaymentButtonModel
    {
        public int OrderId { get; set; }

        public int OrderNumber { get; set; }


        public decimal Amount { get; set; }
    }
}
