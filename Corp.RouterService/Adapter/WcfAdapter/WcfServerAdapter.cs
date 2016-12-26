using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Corp.RouterService.Message;

namespace Corp.RouterService.Adapter.WcfAdapter
{
  public class WcfServerAdapter : WcfAdapter, IAdapter
  {
    static WcfServerAdapter()
    {
      log = LoggingLibrary.LoggerManager.GetLog4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }

    private ServiceHost _serviceHost = null;

    private WcfServerAdapterSettings _configuration = null;

    private IServiceType _serviceType = null;

    private WcfClientServiceTypesDelegates _delegates = null;

    private MessageDispatcher _messageDispatcher;

    private MessageCallbackRegistry _messageRegistry;


    public WcfServerAdapter(WcfServerAdapterSettings configuration)
      : base(configuration)
    {
      _configuration = configuration;
    }

    public void Init()
    {
      State = AdapterState.Initializing;

      _messageRegistry = new MessageCallbackRegistry(_configuration.TimeoutPeriodInMilliseconds, _configuration.MaxMessagesPerSecond);

      _messageDispatcher = new MessageDispatcher(_configuration.RoutingTable, _configuration.MessageAuditLength);

      _serviceType = WcfServiceTypeFactory.GetWcfServiceType(_configuration);

      _delegates = new WcfClientServiceTypesDelegates();

      _delegates.HandleMessageDelegate = new HandleMessage(this.HandleMessage);

      _serviceType.ServiceTypeDelegates = _delegates;

      State = AdapterState.Initialized;
    }

    public void HandleMessage(Message.Message message, Message.CompletionDelegate completionDelegate)
    {

      LogDebug("Got message to handle:" + message + " in " + _configuration.ToString());

      _messageRegistry.Register(message, completionDelegate, f => {
        try
        {
          if (f == null)
          {
            if (log.IsWarnEnabled)
            {
              log.Warn("Timeout in " + _configuration + " for message " + message.ID);
            }
            return;
          }
          string retreivalID = f.RetreivalID;
          IncrementReceivedMessages();

          _messageDispatcher.DispatchMessage(f, (e) =>
          {

            if (!_messageRegistry.Notify(f, e))
            {
              if (e == null)
                LogWarn("Failed to notify requestor for retreivalID=" + retreivalID + " and response=null . Propably caller timed out..");
              else
                LogWarn("Failed to notify requestor for retreivalID=" + retreivalID + " and response=" + e.ToString() + " . Propably caller timed out..");
            }
          });

        }
        catch (Exception ex)
        {
          if (log.IsErrorEnabled)
          {
            if (f != null)
              log.Error("Error for Message:" + f.ToString() + " " + ex.ToString());
            else
              log.Error("Error " + ex.ToString());
          }

          _messageRegistry.Notify(f, null);
        }
      });

      
    }

    public void Start()
    {
      State = AdapterState.Starting;

      _serviceHost = new ServiceHost(_serviceType.GetServiceType(), new Uri[] { new Uri(_configuration.ServerURI) });
      _serviceHost.Open();

      State = AdapterState.Started;
    }

    public void Stop()
    {

      State = AdapterState.Stopping;

      if (_serviceHost != null)
        _serviceHost.Close();

      _serviceHost = null;

      State = AdapterState.Stopped;
    }

    public void SendMessage(Message.Message message, CompletionDelegate completionDelegate)
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {
      if (_serviceHost != null)
        _serviceHost.Close();
      _serviceHost = null;
    }

  }
}
