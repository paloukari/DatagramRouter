using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corp.RouterService.Adapter.WcfAdapter
{
  public static class WcfServiceTypeFactory
  {
    public static IServiceType GetWcfServiceType(WcfServerAdapterSettings settings)
    {
      switch (settings.MessageType)
      {
        case Corp.RouterService.Message.MessageType.Iso8583:
          break;
        case Corp.RouterService.Message.MessageType.Iso8583ATM:
          break;
        case Corp.RouterService.Message.MessageType.IsoInternal:
          break;
        case Corp.RouterService.Message.MessageType.Web:
          break;
        case Corp.RouterService.Message.MessageType.Operation:
          return new OperationService(settings.Name, settings.AppSettings);
        case Corp.RouterService.Message.MessageType.Information:
          return new InformationService(settings.Name, settings.AppSettings);
        case Corp.RouterService.Message.MessageType.Hsm:
          return new HsmService(settings.Name, settings.AppSettings);
        case Corp.RouterService.Message.MessageType.ThreePleLayerSecurity:
          return new ThreePleLayerSecurityService(settings.Name, settings.AppSettings);
        case Corp.RouterService.Message.MessageType.Unknown:
          break;
        default:
          break;
      }

      throw new Exception("Could not find IServiceType for settings:" + settings);
    }
  }
}
