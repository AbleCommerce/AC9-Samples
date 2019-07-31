using CommerceBuilder.Common;
using CommerceBuilder.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommerceBuilder.Utility;
using CommerceBuilder.Payments;

namespace ExamplePaymentPlugin
{
    public class EPPlugin : PluginBase
    {
        public EPPlugin(PluginManifest manifest)
            :base(manifest)
        {
        }

        public override bool Install()
        {
            return base.Install();
        }

        public override bool UnInstall()
        {
            if (base.UnInstall())
            {
                var gatewayRepo = AbleContext.Container.Resolve<IPaymentGatewayRepository>();
                string classId = Misc.GetClassId(typeof(EPProvider));
                int gatewayId = gatewayRepo.GetPaymentGatewayIdByClassId(classId);
                var gateway = gatewayRepo.Load(gatewayId);
                if (gateway != null)
                {
                    gatewayRepo.Delete(gateway);
                }

                return true;
            }

            return false;
        }
    }
}
