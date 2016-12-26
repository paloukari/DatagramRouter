
namespace Corp.RouterService.Adapter
{
    public static class AdapterFactory
    {
        private static object syncObj = new object();
        public static IAdapter GetAdapter(Message.Message message)
        {
            IAdapter adapter =null;

            lock (syncObj)
            {
                if (AdapterCache.Contains(message.Info.OutgoingEndpoints.RemoteEndpoind, ref adapter))
                    return adapter;

                //switch (message.Info.Type)
                //{
                //    case Corp.RouterService.Message.MessageType.Iso:
                //        switch (message.Info.OutgoingEndpoints.RemoteEndpoind.EndpointType)
                //        {
                //            case Corp.RouterService.Message.MessageEndpointType.tcp:
                //                {
                //                    adapter = new TcpAdapter.TcpAdapter(message.Info.OutgoingEndpoints.RemoteEndpoind);
                //                } break;
                //            case Corp.RouterService.Message.MessageEndpointType.WCF:
                //                break;
                //            default:
                //                break;
                //        }
                //        break;
                //    case Corp.RouterService.Message.MessageType.Web:
                //        switch (message.Info.OutgoingEndpoints.RemoteEndpoind.EndpointType)
                //        {
                //          case Corp.RouterService.Message.MessageEndpointType.tcp:
                //            {
                //              adapter = new TcpAdapter.TcpAdapter(message.Info.OutgoingEndpoints.RemoteEndpoind);
                //            } break;
                //          case Corp.RouterService.Message.MessageEndpointType.WCF:
                //            break;
                //          default:
                //            break;
                //        }
                //        break;
                //    case Corp.RouterService.Message.MessageType.Unknown:
                //        break;
                //    default:
                //        break;
                //}

                //Debug.Assert(adapter != null);
                //AdapterCache.Add(message.Info.OutgoingEndpoints.RemoteEndpoind, adapter);
            }
            return adapter;
        }
    }
}
