using System.Net;

namespace Corp.RouterService.Message
{
    internal class TcpMessageEndpoint
    {
        IPEndPoint _endpoint;

        public TcpMessageEndpoint(IPEndPoint endpoint)
        {
            _endpoint = endpoint;
        }
        public string EndpointIP
        {
            get { return _endpoint.Address.ToString(); }
        }

        public int EndpointPort
        {
            get { return _endpoint.Port; }
        }
    }
}
