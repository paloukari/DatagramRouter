using Corp.RouterService.Message;

namespace Corp.RouterService.Adapter.WcfAdapter
{
  public class WcfClientAdapterSettings : WcfAdapterSettings
  {
    public string EndpointConfigurationName { get; set; }

    public override WcfAdapterMode AdapterMode() { return WcfAdapterMode.Client; }

    public override string ToString()
    {
      return EndpointConfigurationName;
    }
  } 
}
