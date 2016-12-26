using System;
using Corp.RouterService.Message;
using Corp.RouterService.Message.DatagramProcessor;


namespace Corp.RouterService.Adapter.MemoryAdapter
{
  class MemoryAdapter : AdapterBase, IAdapter
  {
    private MemClientSettings _memClientSettings;


    public MemoryAdapter(MemClientSettings clientSettings)
      : base(clientSettings)
    {
      //we need the settigns for the adapter name
      _memClientSettings = clientSettings;

      State = AdapterState.Initializing;

      State = AdapterState.Initialized;
    }

    public void SendMessage(Message.Message message, CompletionDelegate completionDelegate)
    {
      if (State != AdapterState.Started)
      {
        LogWarn("SendMessage called while State = " + State.ToString() + ". Will ignore this Message");
        return;
      }



      if (message != null)
      {
        IncrementReceivedMessages();

        LogDebug("SendingMessage " + message.ToString());

        var processor = DatagramProcessorFactory.GetDatagramProcessor(message);

        if (message.ProcessorData.IsDiagnostic)
        {
          Message.Message diagResponse = processor.ProcessDiagnostic(message);

          completionDelegate(diagResponse);

          IncrementSentMessages();
        }
        else
        {
          //the processor should be able to handle stray message, at least log them
          Message.Message response = processor.ProcessMessage(message);

          completionDelegate(response);

          IncrementSentMessages();
        }

      }
      else
        LogWarn("Message is null");      
    }



    public override string ToString()
    {
      return _memClientSettings.ToString();
    }

    public void Start()
    {
      State = AdapterState.Starting;

      State = AdapterState.Started;
    }

    public void Stop()
    {
      State = AdapterState.Stopping;

      State = AdapterState.Stopped;
    }

    public void Dispose()
    {
    }

    public void Init()
    {
      State = AdapterState.Initializing;

      State = AdapterState.Initialized;
    }
  }
}
