using System.Threading.Tasks;
using Corp.RouterService.Common;

using Corp.RouterService.TcpServer;

namespace Corp.RouterService.Message
{
    class TcpMessageDispatcher : MessageDispatcher
    {
      private static LoggingLibrary.Log4Net.ILog log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private TcpServerLiveStatistics _liveStatistics;
        private TcpServerSettings _settings;

        private TcpMessageDispatcher()
            : base(null, -1)
        {

        }

        public TcpMessageDispatcher(TcpServerSettings settings, TcpServerLiveStatistics _liveStatistics)
          : base(settings.RoutingTable, settings.MessageAuditLength)
        {
            this._liveStatistics = _liveStatistics;
            _settings = settings;
        }

        internal void DispatchMessage(TcpMessageToken incomingMessageToken, TcpMessageToken outgoingMessageToken, Task completionTask)
        {
            //create the dto
            Message inMessage = incomingMessageToken.MessageHandler.ArmedTcpMessage.ToMessage();

            DispatchMessage(inMessage, (message) =>
            {
              _liveStatistics.IncrementDispatchedMessages();

              if (message == null)
              {
                //something went wrong or it's one way message
                  if (log.IsWarnEnabled)
                  {
                      log.Warn(" message:" + inMessage.ToString() + " did not get a response!");
                  }                
              }
              else
              {                
                TcpMessage outTcpMessage = message.ToTcpMessage(_settings.OutgoingDirectionSettings);

                outgoingMessageToken.MessageHandler.AddOutgoingTcpMessage(outTcpMessage);

                completionTask.Start();
              }

            });
        }

    }
}
