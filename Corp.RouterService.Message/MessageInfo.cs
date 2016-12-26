
using System;
namespace Corp.RouterService.Message
{
  public class MessageInfo
  {
    DateTime _creationTime;

    public MessageInfo()
    {
      _creationTime = DateTime.Now;
    }
    MessageEndpoints _incomingEndpoints;

    public MessageEndpoints IncomingEndpoints
    {
      get { return _incomingEndpoints; }
      set { _incomingEndpoints = value; }
    }
    MessageEndpoints _outgoingEndpoints;

    public MessageEndpoints OutgoingEndpoints
    {
      get { return _outgoingEndpoints; }
      set { _outgoingEndpoints = value; }
    }

    public DateTime CreationTime { get { return _creationTime; } }

    public override string ToString()
    {
      return "TimeStamp:" + _creationTime + ", " +
        "IncomingEndpoints:" + (IncomingEndpoints != null ? IncomingEndpoints.ToString() : "(NULL)") +
        "OutgoingEndpoints:" + (OutgoingEndpoints != null ? OutgoingEndpoints.ToString() : "(NULL)");
    }
  }
}
