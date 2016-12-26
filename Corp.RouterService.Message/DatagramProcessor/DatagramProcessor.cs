
namespace Corp.RouterService.Message.DatagramProcessor
{
    public class DatagramProcessor
    {
        private static readonly LoggingLibrary.Log4Net.ILog log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
       System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public virtual void PreprocessMessage(ref Message inMessage)
        {
            if (log.IsWarnEnabled)
            {
                log.Warn("PreprocessMessage not implemented for message:" + inMessage.ToString());
            }
        }

        public virtual Message ProcessDiagnostic(Message inMessage)
        {
            if (log.IsWarnEnabled)
            {
                log.Warn("ProcessDiagnostic not implemented for message:" + inMessage.ToString());
            }
            return null;
        }
        public virtual Message ProcessMessage(Message inMessage)
        {
            if (log.IsWarnEnabled)
            {
                log.Warn("ProcessMessage not implemented for message:" + inMessage.ToString());
            }
            return null;
        }
    }
}
