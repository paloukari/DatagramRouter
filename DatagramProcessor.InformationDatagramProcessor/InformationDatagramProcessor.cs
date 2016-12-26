
namespace Corp.RouterService.Message.DatagramProcessor
{

  public class InformationDatagramProcessor : DatagramProcessor
  {
    private static LoggingLibrary.Log4Net.ILog log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
          System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public override void PreprocessMessage(ref Message inMessage)
    {
      InformationData webdata = null;
      if (!string.IsNullOrEmpty(inMessage.HeaderSuffix))
        webdata = new InformationData(inMessage.HeaderSuffix + inMessage.Payload);
      else
        webdata = new InformationData(inMessage.Payload);
      inMessage.ProcessorData = webdata;
    }

    public override Message ProcessMessage(Message inMessage)
    {
      if (log.IsWarnEnabled)
      {
        if (inMessage != null)
          log.Warn("Received message to Process:" + inMessage.ToString());
        else
          log.Warn("Received message to Process:null");
      }

      return null;

    }
  }
}
