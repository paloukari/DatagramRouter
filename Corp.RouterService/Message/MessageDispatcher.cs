using System;
using System.Threading.Tasks;
using Corp.RouterService.Adapter;
using Corp.RouterService.Common;

namespace Corp.RouterService.Message
{
  class MessageDispatcher
  {
    private static readonly LoggingLibrary.Log4Net.ILog Log = null;
    private int _messageAuditLength = -1;

    static MessageDispatcher()
    {
      Log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }

    private MessageRoutingTable _routingTable = null;
    
    private MessageDispatcher(){}
    
    public MessageDispatcher(MessageRoutingTable routingTable, int messageAuditLength)
    {
      _routingTable = routingTable;
      _messageAuditLength = messageAuditLength;
    }

    internal void DispatchMessage(Message incomingMessage, CompletionDelegate completionTask)
    {
      //create the router
      var router = RouterServiceFactory.GetRouterService(incomingMessage, _routingTable);

      Task.Factory.StartSafeNew(new Action(() =>
      {
        router.RouteMessage(ref incomingMessage);
      
        string incomingMessageText = incomingMessage != null ? incomingMessage.ToString() : "NULL";
        string incomingEndpoint = (incomingMessage != null && incomingMessage.Info != null) ? incomingMessage.Info.ToString() : "NULL";
        

        var sendAdapter = AdapterFactory.GetAdapter(incomingMessage);

        sendAdapter.SendMessage(incomingMessage, (outgoingMessage) =>
        {
          string outgoingMessageText = (outgoingMessage != null ? outgoingMessage.ToString() : "NULL");
          string outgoingEndpoint = ((outgoingMessage != null && outgoingMessage.Info != null) ? outgoingMessage.Info.ToString() : "NULL");
          completionTask.Invoke(outgoingMessage);

          if (Log.IsInfoEnabled)
          {
            if (_messageAuditLength >= 0)
            { 
              incomingMessageText=incomingMessageText.Substring(0,Math.Min(_messageAuditLength, incomingMessageText.Length)) + "...";
              outgoingMessageText = outgoingMessageText.Substring(0, Math.Min(_messageAuditLength, outgoingMessageText.Length)) + "...";                
            }
            Log.InfoFormat("IO Completed from {1} to {3} with In message={0} and Out message={2} ", incomingMessageText, incomingEndpoint, outgoingMessageText, outgoingEndpoint);
          }
        });
      }));

    }
  }
}
