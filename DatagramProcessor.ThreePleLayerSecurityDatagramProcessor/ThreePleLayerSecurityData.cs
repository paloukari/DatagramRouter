using ThreePleLayerSecurityLibrary;


namespace Corp.RouterService.Message.DatagramProcessor
{
  public class ThreePleLayerSecurityData : IProcessorData
  {
    ThreePleLayerSecurityMsg _msg;



    internal ThreePleLayerSecurityData(string asciiData)
    {
      _msg = ThreePleLayerSecurityMsgParser.Parse(asciiData);
    }

    internal ThreePleLayerSecurityMsg ThreePleLayerSecurityMsg
    {
      get { return _msg; }
      set { _msg = value; }
    }
    
    public bool IsDiagnostic
    {
      get { return false; }
    }

    public string MessageID
    {
      get { return _msg.Guid; }
    }

    public string RetreivalID
    {
      get { return _msg.Guid; }
    }

    public string TransactionType
    {
      get { return _msg.MessageType; }
    }

  }
}
