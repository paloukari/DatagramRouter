using System;
using System.ServiceModel;

namespace Corp.RouterService.Adapter.WcfAdapter
{
  class ProxiesPool
  {
    private WcfClientAdapterSettings _wcfAdapterClientSettings;
    //private ICommunicationObject _proxiesPool = null;

    private ProxiesPool()
    {
    }

    public ProxiesPool(WcfClientAdapterSettings wcfAdapterClientSettings)
    {      
      _wcfAdapterClientSettings = wcfAdapterClientSettings;

      //TODO: fix the clients proxies pool

      //_proxiesPool = new ClientBasePool<_wcfAdapterClientSettings.Channel>(_wcfAdapterClientSettings.EndpointConfigurationName);
    }

    internal void Open()
    {
      throw new NotImplementedException();
    }

    internal void Close()
    {
      throw new NotImplementedException();
    }

    internal void Send(Message.Message message, Message.CompletionDelegate completionDelegate)
    {
      throw new NotImplementedException();
    }
  }
}

