using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HsmLibrary;


namespace Corp.RouterService.Message.DatagramProcessor
{
  public class HsmDatagramProcessor : DatagramProcessor
  {
    private static readonly LoggingLibrary.Log4Net.ILog log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
             System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


    public override void PreprocessMessage(ref Message inMessage)
    {
      if (inMessage.ProcessorData == null)
      {
        if (log.IsDebugEnabled)
        {
          log.Debug(inMessage.Payload);
        }
        var hsmdata = new HsmData(inMessage.Payload);
        inMessage.ProcessorData = hsmdata as IProcessorData;
      }
    }

    public override Message ProcessMessage(Message inMessage)
    {      
      if (log.IsWarnEnabled)
      {
        log.Warn("HsmDatagramProcessor.ProcessMessage called for message:" + inMessage);
      }      
      return null;
    }





  }
}
