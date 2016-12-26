using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corp.RouterService.Adapter.WcfAdapter
{
  public delegate void HandleMessage(Message.Message message, Message.CompletionDelegate completionDelegate);
  
  public interface IServiceTypeDelegates
  {    
    HandleMessage HandleMessageDelegate { get; set; }
  }
}
