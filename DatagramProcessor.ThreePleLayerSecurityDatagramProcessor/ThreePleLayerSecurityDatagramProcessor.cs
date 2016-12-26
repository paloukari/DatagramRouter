
namespace Corp.RouterService.Message.DatagramProcessor
{

  public class ThreePleLayerSecurityDatagramProcessor : DatagramProcessor
  {
    private static LoggingLibrary.Log4Net.ILog log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
          System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public override void PreprocessMessage(ref Message inMessage)
    {
      ThreePleLayerSecurityData webdata = null;
      if (!string.IsNullOrEmpty(inMessage.HeaderSuffix))
        webdata = new ThreePleLayerSecurityData(inMessage.HeaderSuffix + inMessage.Payload);
      else
        webdata = new ThreePleLayerSecurityData(inMessage.Payload);
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
