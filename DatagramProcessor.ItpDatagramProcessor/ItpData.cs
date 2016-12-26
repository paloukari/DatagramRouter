
using ItpLibrary;
namespace Corp.RouterService.Message.DatagramProcessor
{
  public class ItpData : IProcessorData
  {
      ItpMsg _msg;



    internal ItpData(string asciiData)
    {
        _msg = ItpMsgParser.Parse(asciiData);
    }

    internal ItpMsg ItpMsg
    {
      get { return _msg; }
      set { _msg = value; }
    }

    public bool IsDiagnostic
    {
      get { return false; }
    }
    public bool IsLogon
    {
      get { return _msg is LogonRequest || _msg is LogonResponse; }
    }

    public bool IsLogoff
    {
      get { return _msg is LogoffRequest || _msg is LogoffResponse; }
    }

    public string MessageID
    {
        get { return _msg.MessageGuid; }
    }

    public string RetreivalID
    {
        get { return _msg.MessageGuid; }
    }

    public string TransactionType
    {
      get { return _msg.MessageType; }
    }

  }
}
