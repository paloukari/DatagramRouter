using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corp.RouterService.Message;

namespace Corp.RouterService.Adapter
{
  public class AdapterSettings
  {
    protected MessageRoutingTable _routingTable = null;

    public int MessageAuditLength { get; set; }

    public AdapterSettings()
    {
      Name = "";
    }
    internal virtual string PreformanceCountersIntanceName
    {
      get
      {
        return "HPC " + ToString();
      }
    }

    public bool IsActive { get; set; }

    public MessageRoutingTable RoutingTable
    {
      get { return _routingTable; }
      set { _routingTable = value; }
    }
    
    public bool UsePerformanceCounters { get; set; }

    public string Name { get; set; }

    public Dictionary<string, Dictionary<string, string>> AppSettings { get; set; }

    public int MaxMessagesPerSecond { get; set; }
  }
}
