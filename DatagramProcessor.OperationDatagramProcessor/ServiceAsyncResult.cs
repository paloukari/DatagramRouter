using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AntigonisTypes.Operation;

namespace DatagramProcessor.OperationDatagramProcessor
{
  public class ServiceAsyncResult : IAsyncResult
  {
    private object _state;
    private OperationMessageRequest _operationMessageRequest;        

    public ServiceAsyncResult(object state, OperationMessageRequest operationMessageRequest)
    {
      // TODO: Complete member initialization
      _state = state;
      _operationMessageRequest = operationMessageRequest;
    }


    public bool IsCompleted { get; set; }

    public object AsyncState
    {
      get { throw new NotImplementedException(); }
    }

    public System.Threading.WaitHandle AsyncWaitHandle
    {
      get { throw new NotImplementedException(); }
    }

    public bool CompletedSynchronously
    {
      get { throw new NotImplementedException(); }
    }
  }
}
