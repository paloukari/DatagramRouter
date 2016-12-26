using System;
using System.ServiceModel;

namespace Corp.RouterService.Adapter.WcfAdapter
{
  public class WcfAdapter : AdapterBase
  {    
    WcfAdapterSettings _adapterConfiguration = null;    

    public WcfAdapter(WcfAdapterSettings configuration)
      :base(configuration)
    {
      _adapterConfiguration = configuration;
    }       
  }
}
