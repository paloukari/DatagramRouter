using System;
using System.Net;
using Corp.RouterService.Message;
using Corp.RouterService.Adapter;

namespace Corp.RouterService.TcpServer
{
  public class TcpServerSettings : AdapterSettings
  {    
    private TcpTrafficSettings incomingTrafficSettings;

    public TcpTrafficSettings IncomingDirectionSettings
    {
      get { return incomingTrafficSettings; }
      set { incomingTrafficSettings = value; }
    }

    private TcpTrafficSettings outgoingTrafficSettings;

    public TcpTrafficSettings OutgoingDirectionSettings
    {
      get { return outgoingTrafficSettings; }
      set { outgoingTrafficSettings = value; }
    }

    private IPEndPoint localEndPoint;

    public IPEndPoint LocalEndPoint
    {
      get { return localEndPoint; }
      set { localEndPoint = value; }
    }



    public TcpServerSettings()
    {
    }

    public int PoolSize { get; set; }

    public void UpdateSettings(TcpServerSettings value)
    {
      throw new NotImplementedException();
    }

    public int ConnectionsBacklog { get; set; }


    public override string ToString()
    {
      try
      {
        return "Tcp Server " + Name +" "+ LocalEndPoint.ToString();
      }
      catch
      {
        return "";
      }
    }
  }
}
