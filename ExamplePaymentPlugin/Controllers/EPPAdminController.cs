using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using CommerceBuilder.Common;
using ExamplePaymentPlugin.Models;
using CommerceBuilder.Utility;
using CommerceBuilder.Web.Mvc;
using CommerceBuilder.Payments;
using CommerceBuilder.Payments.Providers;

namespace ExamplePaymentPlugin.Controllers
{
    public class EPPAdminController : AbleAdminController
    {
        private readonly IPaymentGatewayRepository _gatewayRepo;
        private readonly IPaymentMethodRepository _methodRepo;

        private readonly PaymentInstrumentType[] _hiddenMethods = { PaymentInstrumentType.GiftCertificate, PaymentInstrumentType.GoogleCheckout, PaymentInstrumentType.Mail, PaymentInstrumentType.PhoneCall, PaymentInstrumentType.PurchaseOrder, PaymentInstrumentType.PayPal };

        public EPPAdminController(IPaymentGatewayRepository gatewayRepo, IPaymentMethodRepository methodRepo)
        {
            _gatewayRepo = gatewayRepo;
            _methodRepo = methodRepo;
        }

        public ActionResult Config(int? id)
        {
            int gatewayId = id ?? 0;
            PaymentGateway gateway = _gatewayRepo.Load(gatewayId);

            var aName = GetType().Assembly.GetName();
            string assemblyInfo = aName.Name.ToString() + "&nbsp;(v" + aName.Version.ToString() + ")";

            var model = new ConfigModel();
            model.AssemblyInfo = assemblyInfo;
            model.ExecutionModes = Enum.GetNames(typeof(EPProvider.GatewayExecutionMode))
                .Select(name => new SelectListItem() { Text = name, Value = name })
                .ToList();
            model.PaymentMethods = _methodRepo.LoadPaymentMethods(_hiddenMethods).Select(method => new PaymentMethodModel()
            {
                Id = method.Id,
                Name = method.Name }
            ).ToList();

            if (gateway != null)
            {
                model.GatewayId = gateway.Id;
                model.PaymentMethods.ForEach(method => method.IsSelected = gateway.PaymentMethods.Any(pm => pm.Id == method.Id));

                var configurationData = gateway.ParseConfigData();
                if (configurationData.ContainsKey("UseDebugMode"))
                    model.UseDebugMode = AlwaysConvert.ToBool(configurationData["UseDebugMode"], false);
                if (configurationData.ContainsKey("ExecutionMode"))
                    model.ExecutionMode = AlwaysConvert.ToEnum<EPProvider.GatewayExecutionMode>(configurationData["ExecutionMode"], EPProvider.GatewayExecutionMode.AlwaysAccept);
                if (configurationData.ContainsKey("UseAuthCapture"))
                    model.UseAuthCapture = AlwaysConvert.ToBool(configurationData["UseAuthCapture"], true);
            }

            return View("~/Plugins/ExamplePaymentPlugin/Views/Config.cshtml", model);
        }

        [HttpPost]
        [ValidateAjax]
        public ActionResult Config(ConfigModel model, FormCollection form)
        {
            var paymentMethods = _methodRepo.LoadPaymentMethods(_hiddenMethods);
            PaymentGateway gateway = model.GatewayId.HasValue ? _gatewayRepo.Load(model.GatewayId.Value) : null;
            model.ShowConfirmationMessage = true;
            if (gateway == null)
            {
                string classId = Misc.GetClassId(typeof(EPProvider));
                var provider = Activator.CreateInstance(Type.GetType(classId)) as IPaymentProvider;
                gateway = new PaymentGateway();
                gateway.ClassId = classId;
                gateway.Name = provider.Name;
                gateway.Store = AbleContext.Current.Store;
                model.ShowConfirmationMessage = false;
            }

            Dictionary<string, string> configData = new Dictionary<string, string>();
            configData.Add("UseDebugMode", model.UseDebugMode.ToString());
            configData.Add("ExecutionMode", model.ExecutionMode.ToString());
            configData.Add("UseAuthCapture", model.UseAuthCapture.ToString());
            gateway.UpdateConfigData(configData);
            _gatewayRepo.Save(gateway);

            //UPDATE PAYMENT METHODS

            var selectedKeys = form.AllKeys.Where(key => key.ToUpper().StartsWith("METHOD_CHK_",
                  StringComparison.InvariantCultureIgnoreCase)).ToList();

            foreach (var key in selectedKeys)
            {
                int paymentMethodId = AlwaysConvert.ToInt(key.Substring(11));
                PaymentMethod method = paymentMethods.Where(m => m.Id == paymentMethodId).FirstOrDefault();
                if (method != null)
                {
                    if (form[key].Contains("true"))
                    {
                        method.PaymentGateway = gateway;
                    }
                    else if (method.PaymentGateway != null && method.PaymentGateway.Id == gateway.Id)
                    {
                        method.PaymentGateway = null;
                    }

                    _methodRepo.Save(method);
                }
            }

            if (model.SaveOnly.Equals("true"))
            {
                return Json(new { status = "Success", Id = gateway.Id, showMessage = model.ShowConfirmationMessage });
            }
            else
            {
                return Json(new { status = "Redirect", Url = Url.Action("Gateways", "Payments") });
            }
        }
    }
}
