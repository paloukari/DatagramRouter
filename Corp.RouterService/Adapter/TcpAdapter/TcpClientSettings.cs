using System.Net;
using Corp.RouterService.TcpServer;

namespace Corp.RouterService.Adapter.TcpAdapter
{
    public class TcpClientSettings : TcpServerSettings
    {
        private IPEndPoint remoteEndPoint;

        public IPEndPoint RemoteEndPoint
        {
            get { return remoteEndPoint; }
            set { remoteEndPoint = value; }
        }

        public override string ToString()
        {
            return "Tcp Client" + Name + " " +RemoteEndPoint.ToString();
        }

        public int TimeoutInMilliseconds { get; set; }        
    }
}
