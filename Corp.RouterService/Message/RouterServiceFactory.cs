using System;

namespace Corp.RouterService.Message
{
    public static class RouterServiceFactory
    {
        public static RouterService.RouterService GetRouterService(Message message, MessageRoutingTable RouterServicePredicates)
        {
            switch (message.Type)
            {
              case MessageType.Iso8583:
                return new RouterService.Iso8583RouterService(RouterServicePredicates);
              case MessageType.Iso8583ATM:
                return new RouterService.Iso8583ATMRouterService(RouterServicePredicates);
              case MessageType.IsoInternal:
                return new RouterService.IsoInternalRouterService(RouterServicePredicates);
              case MessageType.IsoInternalPos:
                return new RouterService.IsoInternalPosRouterService(RouterServicePredicates);
              case MessageType.Web:
                return new RouterService.WebRouterService(RouterServicePredicates);
              case MessageType.Operation:
                return new RouterService.OperationRouterService(RouterServicePredicates);
              case MessageType.Information:
                return new RouterService.InformationRouterService(RouterServicePredicates);
              case MessageType.Hsm:
                return new RouterService.HsmRouterService(RouterServicePredicates);
              case MessageType.ThreePleLayerSecurity:
                return new RouterService.ThreePleLayerSecurityRouterService(RouterServicePredicates);
              case MessageType.Itp:
                return new RouterService.ItpRouterService(RouterServicePredicates);
                default:
                    throw new ArgumentException("Not found implemented", "message.Type");
            }
        }
    }
}
