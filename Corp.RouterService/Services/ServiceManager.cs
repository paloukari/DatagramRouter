using System.Collections.Generic;
using Corp.RouterService.TcpServer;

namespace Corp.RouterService.Services
{
    internal class ServiceManager
    {
        Dictionary<string, object> _services = null;
        private TcpServerSettings _settings;

        private ServiceManager()
        {
            
        }

        internal ServiceManager(TcpServerSettings settings)
        {            
            _settings = settings;
            _services = new Dictionary<string, object>();
        }


        internal void Initialize()
        {
         
            
        }
    }
}
