using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using System.Threading.Tasks;
using Corp.RouterService.Common;

namespace Corp.RouterService.Adapter
{
  //todo: purge the registry periodically
  internal class MessageCallbackRegistry
  {
    private static LoggingLibrary.Log4Net.ILog log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
       System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    int _timeoutInMilliseconds;
    int _maxMessagesPerSecond;
    DateTime _lastRegistryTime;
    System.Timers.Timer _timeoutTimer;

    private MessageCallbackRegistry()
    {

    }

    private Dictionary<Message.Message, Message.CompletionDelegate> _messagesCallbacks = null;

    public MessageCallbackRegistry(int timeout, int maxMessagesPerSecond)
    {
      _timeoutInMilliseconds = timeout;
      _maxMessagesPerSecond = maxMessagesPerSecond;

      if (_maxMessagesPerSecond > 0)
        _lastRegistryTime = DateTime.Now.AddMilliseconds(1000 / _maxMessagesPerSecond);
      else
        _lastRegistryTime = DateTime.Now;

      if (_timeoutInMilliseconds > 0)
      {
        _timeoutTimer = new System.Timers.Timer(_timeoutInMilliseconds);
        _timeoutTimer.AutoReset = true;
        _timeoutTimer.Elapsed += new ElapsedEventHandler(_timeoutTimer_Elapsed);

        _timeoutTimer.Start();
      }

      _messagesCallbacks = new Dictionary<Message.Message, Message.CompletionDelegate>();
    }

    void _timeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      lock (this)
      {
        var result = _messagesCallbacks.
          Where(msg => (DateTime.Now - msg.Key.Info.CreationTime).TotalMilliseconds > _timeoutInMilliseconds).
          Select(msg => msg.Key).ToList();
        for (int i = 0; i < result.Count; i++)
        {
          if (log.IsErrorEnabled)
          {
            log.Warn("Timeout in _timeoutTimer_Elapsed for message:" + result[i].ID);
          }
          _messagesCallbacks[result[i]](null);
          _messagesCallbacks.Remove(result[i]);
        }
      }
    }
    //register an outgoing (client adapters) message 
    internal void Register(Message.Message message, Message.CompletionDelegate completionDelegate, Message.CompletionDelegate registrationDelegate)
    {
      if (_maxMessagesPerSecond != 0)
      {
        double millisecondsToSleep = 0;
        millisecondsToSleep = (1000 / _maxMessagesPerSecond) - (DateTime.Now - _lastRegistryTime).TotalMilliseconds;
        if (millisecondsToSleep > 0)
        {
          //if (log.IsDebugEnabled)
          //{
          //  log.Debug("Sleeping for " + millisecondsToSleep + " for message :" + message.ID);            
          //}
          System.Threading.Timer timer = null;
          timer = new System.Threading.Timer((state) =>
               {

                 //if (log.IsDebugEnabled)
                 //{
                 //  log.Debug("Sleeped for " + millisecondsToSleep + " for message :" + message.ID);                  
                 //}
                 Task.Factory.StartSafeNew(() =>
                 {
                   //this is a hack to keep this object alive
                   if (timer != null)
                     timer.Dispose();
                   Register(message, completionDelegate, registrationDelegate);
                 });
               }, null, TimeSpan.FromMilliseconds(millisecondsToSleep), TimeSpan.FromMilliseconds(-1));
          return;
        }
      }

      lock (this)
      {
        if (completionDelegate != null)
        {
          if (_messagesCallbacks.ContainsKey(message))
          {
            //we have 2 messages with the same key
            //ignore the old one                        

            if (log.IsErrorEnabled)
            {
              log.Error("Found same ID in message:" + message.ToString());
            }
            _messagesCallbacks[message](null);
            _messagesCallbacks.Remove(message);
          }

          if (_timeoutInMilliseconds > 0)
          {
            if ((DateTime.Now - message.Info.CreationTime).TotalMilliseconds > _timeoutInMilliseconds)
            {
              if (log.IsErrorEnabled)
              {
                log.Warn("Timeout in registering message:" + message.ID);
              }
              registrationDelegate(null);
              completionDelegate(null);
              return;
            }
          }

          if (_maxMessagesPerSecond != 0)
          {
            double millisecondsToSleep = 0;
            millisecondsToSleep = (1000 / _maxMessagesPerSecond) - (DateTime.Now - _lastRegistryTime).TotalMilliseconds;
            if (millisecondsToSleep > 0)
            {
              //if (log.IsDebugEnabled)
              //{
              //  log.Debug("Sleeping for " + millisecondsToSleep + " for message :" + message.ID);
              //}
              var timer = new System.Threading.Timer((state) =>
              {
                //if (log.IsDebugEnabled)
                //{
                //  log.Debug("Sleeped for " + millisecondsToSleep + " for message :" + message.ID);
                //}
                Task.Factory.StartSafeNew(() =>
                {
                  Register(message, completionDelegate, registrationDelegate);
                }, TaskCreationOptions.LongRunning);
              }, null, TimeSpan.FromMilliseconds(millisecondsToSleep), TimeSpan.FromMilliseconds(-1));
              return;
            }
          }

          _messagesCallbacks[message] = completionDelegate;
          _lastRegistryTime = DateTime.Now;
        }
        else
        {
          if (log.IsWarnEnabled)
          {
            log.Warn("No completion delegate for message:" + message.ToString());
          }
        }
      }
      registrationDelegate(message);
    }
    //notify the owner about the response
    internal bool Notify(Message.Message request, Message.Message response)
    {
      lock (this)
      {
        if (_messagesCallbacks.ContainsKey(request))
        {
          var callback = _messagesCallbacks[request];
          
          Task.Factory.StartSafeNew(() => {
            callback(response);
          });
          _messagesCallbacks.Remove(request);
          return true;
          //todo: add the date of the message sent to truncate old callbacks
        }
        else
          return false;
      }
    }

    internal void Clear()
    {
      lock (this)
      {
        _messagesCallbacks.Clear();
      }
    }
  }
}
