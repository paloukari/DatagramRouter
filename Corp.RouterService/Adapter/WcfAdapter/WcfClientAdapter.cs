using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corp.RouterService.Adapter.WcfAdapter
{
  public class WcfClientAdapter : WcfAdapter, IAdapter
  {
    WcfClientAdapterSettings _configuration = null;
    private ProxiesPool _proxiesPool;

    static WcfClientAdapter()
    {
      log = LoggingLibrary.LoggerManager.GetLog4NetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    }

    public WcfClientAdapter(WcfClientAdapterSettings configuration)
      : base(configuration)
    {
      _configuration = configuration;

      State = AdapterState.Uninitialized;
    }

    public void Init()
    {
      State = AdapterState.Initializing;

      State = AdapterState.Initialized;
    }

    public void Start()
    {
      State = AdapterState.Starting;

      _proxiesPool = new ProxiesPool((WcfClientAdapterSettings)_configuration);
      _proxiesPool.Open();

      State = AdapterState.Started;
    }

    public void Stop()
    {

      State = AdapterState.Stopping;

      if (_proxiesPool != null)
        _proxiesPool.Close();
      _proxiesPool = null;

      State = AdapterState.Stopped;
    }

    public void SendMessage(Message.Message message, Message.CompletionDelegate completionDelegate)
    {
      if (_configuration.IsActive)
      {
        _proxiesPool.Send(message, completionDelegate);
      }
      else
      {
        string error = this.ToString() + " : SendMessage call and WcfAdapter is not configured as client mode";
        if (log.IsErrorEnabled)
        {
          log.Error(error);
        }
        throw new Exception(error);
      }
    }

    public void Dispose()
    {      
      if (_proxiesPool != null)
        _proxiesPool.Close();
      _proxiesPool = null;
    }





  }

}
