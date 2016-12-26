using System;

namespace Corp.RouterService.Adapter.WcfAdapter
{
  public class WcfServerAdapterSettings : WcfAdapterSettings
  {
    public string ServerURI { get; set; }
    public Type ServiceContract { get; set; }

    public override WcfAdapterMode AdapterMode() { return WcfAdapterMode.Server; }

    public override string ToString()
    {
      return ServerURI;
    }    
  }
}
