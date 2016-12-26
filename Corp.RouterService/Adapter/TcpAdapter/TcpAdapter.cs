using System;

namespace Corp.RouterService.Adapter.TcpAdapter
{
  public class TcpAdapter : AdapterBase, IAdapter
  {

    private TcpClient _client;
    TcpClientSettings _settings;
    public TcpAdapter(TcpClientSettings settings)
        :base(settings)
    {
      _settings = settings;

      State = AdapterState.Uninitialized;
    }


    public void SendMessage(Message.Message message, Message.CompletionDelegate completionDelegate)
    {
      if (State != AdapterState.Started)
      {
        LogWarn("SendMessage called while State = " + State.ToString() + ". Will ignore this Message");
        return;
      }

      _client.Send(message, completionDelegate);      
    }

    public void Init()
    {
      State = AdapterState.Initializing;

      _client = new TcpClient(_settings);

      _client.Initialize();

      State = AdapterState.Initialized;
    }

    public void Start()
    {
      State = AdapterState.Starting;

      _client.Start();

      State = AdapterState.Started;
    }

    public void Stop()
    {
      State = AdapterState.Stopping;

      _client.Stop();

      State = AdapterState.Stopped;
    }

    public override string ToString()
    {
      return _settings.ToString();
    }

    public void Dispose()
    {
      try
      {
        if (_client != null)
          _client.Dispose();
        _client = null;
      }
      catch { }
    }

    
  }
}
