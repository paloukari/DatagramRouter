using AntigonisTypes.Information;
using InformationLibrary;


namespace Corp.RouterService.Message.DatagramProcessor
{
  public class InformationData : IProcessorData
  {
    InformationMsg _msg;

   

    internal InformationData(string asciiData)
    {
      _msg = InformationMsgParser.Parse(asciiData);
    }

    internal InformationMsg InformationMsg
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
