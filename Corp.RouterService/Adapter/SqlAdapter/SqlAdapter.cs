using System;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;

using Corp.RouterService.Adapter.SqlAdapter;
using Corp.RouterService.Common;
using Corp.RouterService.Message;

namespace Corp.RouterService.Adapter.SqlAdapter
{
  internal class SqlAdapter : AdapterBase, IAdapter
  {
    static SqlAdapter()
    {
      log = LoggingLibrary.LoggerManager.GetLog4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }

    private MessageDispatcher _messageDispatcher;

    private SqlAdapterSettings _settings;

    private IQueues _queues = null;

    MessageCallbackRegistry _messageRegistry;

    internal SqlAdapter(SqlAdapterSettings sqlServerSettings)
      : base(sqlServerSettings)
    {
      _settings = sqlServerSettings;

      State = AdapterState.Uninitialized;
    }

    public void Init()
    {
      State = AdapterState.Initializing;

      _messageDispatcher = new MessageDispatcher(_settings.RoutingTable, _settings.MessageAuditLength);

      _queues = SqlQueueFactory.GetSqlQueues(_settings);

      _messageRegistry = new MessageCallbackRegistry(_settings.Timeout, _settings.MaxMessagesPerSecond);

      State = AdapterState.Initialized;

    }

    public void SendMessage(Message.Message message, CompletionDelegate completionDelegate)
    {
      if (log.IsDebugEnabled)
      {
        log.Debug("Got message to send:" + message + " in " + _settings.ToString());
      }
      _messageRegistry.Register(message, completionDelegate, (e) => {
        try
        {
          if (e == null)
          {
            if (log.IsWarnEnabled)
            {
              log.Warn("Timeout in " + _settings + " for message " + message.ID);
            }
            return;
          }
          Enqueue(e);

          IncrementReceivedMessages();
        }
        catch (Exception ex)
        {
          if (log.IsErrorEnabled)
          {
            if (e != null)
              log.Error("Error for Message:" + e.ToString() + " " + ex.ToString());
            else
              log.Error("Error " + ex.ToString());
          }

          _messageRegistry.Notify(e, null);
        }
      });     
    }


    public void Start()
    {
      State = AdapterState.Starting;


      Task.Factory.StartSafeNew(
          () =>
          {
            State = AdapterState.Started;

            while (State == AdapterState.Started)
            {
              Message.Message message = null;
              try
              {

                message = Dequeue();

                if (message != null)
                {



                  if (log.IsDebugEnabled)
                  {
                    log.Debug("Got message to reply:" + message + " in " + _settings.ToString());
                  }

                  if (_settings.AdapterMode() == SqlAdapterMode.Server)
                  {
                    IncrementReceivedMessages();

                    Enqueue(message);

                    IncrementSentMessages();
                  }
                  else
                  {
                    if (!_messageRegistry.Notify(message, message))
                    {
                      //we got a request from the queue that has no match
                      _messageDispatcher.DispatchMessage(message, new CompletionDelegate(Enqueue));
                      if (log.IsWarnEnabled)
                      {
                        log.Warn("Received Message with ID " + message.ID + " with no recipient!");
                      }
                    }
                    IncrementSentMessages();
                  }
                  continue;
                }
                Thread.Sleep(_settings.PollingTimeout);
              }
              catch (ThreadAbortException tae)
              {
                if (log.IsErrorEnabled)
                {
                  if (message != null)
                    log.Error("Error for Message:" + message.ToString() + " " + tae.ToString());
                  else
                    log.Error("Error " + tae.ToString());
                }
                break;
              }
              catch (Exception ex)
              {
                if (log.IsErrorEnabled)
                {
                  if (message != null)
                    log.Error("Error for Message:" + message.ToString() + " " + ex.ToString());
                  else
                    log.Error("Error " + ex.ToString());
                }
              }
            }


          });

    }

    public void Stop()
    {
      State = AdapterState.Stopping;

      State = AdapterState.Stopped;
    }
    public Message.Message Dequeue()
    {
      if (_settings.AdapterMode() == SqlAdapterMode.Server)
        return _queues.InputQueueDequeue();
      else
      {
        return _queues.OutputQueueDequeue();
      }
    }

    public void Enqueue(Message.Message message)
    {
      try
      {
        if (message != null)
        {
          if (_settings.AdapterMode() == SqlAdapterMode.Server)
          {
            _messageDispatcher.DispatchMessage(message, new CompletionDelegate((response) =>
            {
              _queues.OutputQueueEnqueue(response);
            }));
          }
          else
          {
            Task.Factory.StartSafeNew(() =>
            {
              _queues.InputQueueEnqueue(message);
            });
          }
        }
      }
      catch (Exception ex)
      {
        if (log.IsErrorEnabled)
        {
          if (message != null)
            log.Error("Error for Message:" + message.ToString() + " " + ex.ToString());
          else
            log.Error("Error " + ex.ToString());
        }
        throw;
      }
    }
    public override string ToString()
    {
      return _settings.ToString();
    }

    public void Dispose()
    {
    }
  }
}
