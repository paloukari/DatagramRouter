using System;
using System.Collections.Generic;

namespace Corp.RouterService.Message
{
  public class TcpTrafficSettings
  {
    private TcpTrafficSettings()
    {
    }
    public TcpTrafficSettings(int NetworkBufferSize,
        List<TcpMessageSettings> tcpMessagesSettings)
    {
        this.NetworkBufferSize = NetworkBufferSize;
        this.TcpMessagesSettings = tcpMessagesSettings;
    }

    public List<TcpMessageSettings> TcpMessagesSettings { get; set; }

    public int NetworkBufferSize { get; set; }

    


    internal int CircularBufferSize { get { return NetworkBufferSize * 10; } }

    internal TcpMessageSettings GetMessageTypeSettings(MessageType messageType)
    {
        foreach (var item in TcpMessagesSettings)
        {
            if (item.MessageType == messageType)
                return item;
        }
        return null;
    }
  }
}
