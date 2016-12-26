
namespace Corp.RouterService.Message
{
  public class Message
  {

    public IProcessorData ProcessorData { get; set; }

    string _headerText;

    public string HeaderText
    {
        get { return _headerText; }
        set { _headerText = value; }
    }

    MessageType _type;
    public MessageType Type
    {
      get { return _type; }
      set { _type = value; }
    }

    MessageInfo _info;

    public MessageInfo Info
    {
      get { return _info; }
      set { _info = value; }
    }

    private string _payload;
    public string Payload
    {
      get { return _payload; }
      set { _payload = value; }
    }

    private Message()
    {
    }

    public Message(MessageType type, 
      string payload,
      MessageEndpoints incomingEndpoints, 
      string headerText = null,
      byte[] headerCopyData = null, 
      string headerSuffix = null
      )
    {
      _type = type;
      _info = new MessageInfo() { IncomingEndpoints = incomingEndpoints };
      _payload = payload;
      _headerCopyData = headerCopyData;
      if (_headerCopyData == null)
        _headerCopyData = new byte[0];
      _headerSuffix = headerSuffix;
      _headerText = headerText;
    }

    public override string ToString()
    {
      if (!string.IsNullOrEmpty(_headerSuffix))
        return _headerSuffix + Payload;
      return Payload;
    }
    //we use the ID to match the response with the request
    public string ID
    {
      get
      {
        if (this.ProcessorData != null)
          return this.ProcessorData.MessageID;
        return null;
      }
    }

    public string RetreivalID
    {
      get
      {
        if (this.ProcessorData != null)
          return this.ProcessorData.RetreivalID;
        return null;
      }
    }

    byte[] _headerCopyData;

    public byte[] HeaderCopyData
    {
      get { return _headerCopyData; }
    }

    string _headerSuffix;
    public string HeaderSuffix
    {
      get { return _headerSuffix; }
      set { _headerSuffix = value; }
    }

    public override int GetHashCode()
    {
      return ID.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      return Equals(obj as Message);
    }

    public bool Equals(Message obj)
    {
      return obj != null && obj.ID== this.ID;
    }

  }
}
