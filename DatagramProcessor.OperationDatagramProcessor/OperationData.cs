using AntigonisTypes.Operation;


namespace Corp.RouterService.Message.DatagramProcessor
{
  public class OperationData : IProcessorData
  {
    OperationMessageBase _msg;

    internal OperationMessageBase OperationMessage
    {
      get { return _msg; }
      set { _msg = value; }
    }

    internal OperationData(string asciiData)
    {
      _msg = OperationMessageBase.Parse(asciiData);
    }

    internal OperationData(OperationMessageBase message)
    {
      _msg = message;
    }

    public bool IsDiagnostic
    {
      get { return false; }
    }

    public string TransactionType
    {
      get { return _msg.TransactionType; }
    }

    public string MessageID
    {
      get { return _msg.MessageID; }
    }

    public string RetreivalID
    {
      get { return _msg.MessageID; }
    }
  }
}
