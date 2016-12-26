using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corp.RouterService.Message;

namespace Corp.RouterService.Adapter.WcfAdapter
{
  public abstract class WcfAdapterSettings : AdapterSettings
  {    
    public MessageType MessageType { get; set; }    
    public abstract WcfAdapterMode AdapterMode();
    public string Guid { get; set; }
    public int TimeoutPeriodInMilliseconds { get; set; }

    
  }
  public enum WcfAdapterMode
  {
    Server,
    Client
  }
}
