using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HsmLibrary
{
  public class HsmMsg_NOResponse : HsmMsgResponse
  {
    private const string C_MESSAGE_CODE = "NP";

    public static new string MessageCode { get { return C_MESSAGE_CODE; } }

    public HsmMsg_NOResponse(string body)
      : base(body)
    {
      Debug.Assert(ResponseCode == C_MESSAGE_CODE);
    }

    public HsmMsg_NOResponse(string header, string body)
      : base(header,  body)
    {
      Debug.Assert(ResponseCode == C_MESSAGE_CODE);
    }

    public string BufferSize
    {
      get
      {
        return _message.Substring(HeaderLength + MessageTypeLength + ErrorCode.Length, 1);
      }
    }

    public string EthernetType
    {
      get
      {
        return _message.Substring(HeaderLength + MessageTypeLength + ErrorCode.Length + BufferSize.Length, 1);
      }
    }

    public string NumberOfTcpSockets
    {
      get
      {
        return _message.Substring(HeaderLength + MessageTypeLength + ErrorCode.Length + BufferSize.Length + EthernetType.Length, 2);
      }
    }

    public string FirmwareNumber
    {
      get
      {
        return _message.Substring(HeaderLength + MessageTypeLength + ErrorCode.Length + BufferSize.Length + EthernetType.Length + NumberOfTcpSockets.Length, 9);
      }
    }

    public string Reserved1
    {
      get
      {
        return _message.Substring(HeaderLength + MessageTypeLength + ErrorCode.Length + BufferSize.Length + EthernetType.Length + NumberOfTcpSockets.Length + FirmwareNumber.Length, 1);
      }
    }

    public string Reserved2
    {
      get
      {
        return _message.Substring(HeaderLength + MessageTypeLength + ErrorCode.Length + BufferSize.Length + EthernetType.Length + NumberOfTcpSockets.Length + FirmwareNumber.Length + Reserved1.Length, 4);
      }
    }
  }
}
