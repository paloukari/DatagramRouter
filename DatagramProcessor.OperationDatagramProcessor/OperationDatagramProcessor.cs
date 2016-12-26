
using System.Text;
namespace Corp.RouterService.Message.DatagramProcessor
{

  public class OperationDatagramProcessor : DatagramProcessor
  {
    private static LoggingLibrary.Log4Net.ILog log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
          System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public override void PreprocessMessage(ref Message inMessage)
    {
      OperationData webdata = null;
      if (!string.IsNullOrEmpty(inMessage.HeaderSuffix))
        webdata = new OperationData(inMessage.HeaderSuffix + inMessage.Payload);
      else
        webdata = new OperationData(inMessage.Payload);

      inMessage.ProcessorData = webdata;
    }

    public override Message ProcessMessage(Message inMessage)
    {
      if (log.IsWarnEnabled)
      {
        log.Warn("Received message to Process:" + inMessage.ToString());
      }

      return null;
      //never echo the operation messages here

      //  Message response = new Message(inMessage.Type,
      //      inMessage.Payload, 
      //      inMessage.Info.OutgoingEndpoints,
      //      inMessage.HeaderCopyData);
      //  PreprocessMessage(ref response);
      //return response;
    }
  }
}
