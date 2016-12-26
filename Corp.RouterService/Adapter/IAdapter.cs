using System;
using Corp.RouterService.Message;

namespace Corp.RouterService.Adapter
{  
  public interface IAdapter : IDisposable
  {
    void Init();
    void Start();
    void Stop();
    void SendMessage(Message.Message message, CompletionDelegate completionDelegate);   
  }
}
