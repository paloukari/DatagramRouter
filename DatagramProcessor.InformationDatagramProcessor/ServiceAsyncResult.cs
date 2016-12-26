using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AntigonisTypes.Information;

namespace DatagramProcessor.InformationDatagramProcessor
{
  public class ServiceAsyncResult : IAsyncResult
  {
    private object _state;
    private bool _isCompleted;
    private InformationMessageRequest _InformationMessageRequest;
    private System.Threading.AutoResetEvent _waitHandle = new System.Threading.AutoResetEvent(true);

    

    public ServiceAsyncResult(object state, InformationMessageRequest InformationMessageRequest)
    {
      // TODO: Complete member initialization
      _state = state;
      _InformationMessageRequest = InformationMessageRequest;
    }


    public object AsyncState
    {
      get { return _state; }
    }

    public System.Threading.WaitHandle AsyncWaitHandle
    {
      get { return _waitHandle; }
    }

    public bool CompletedSynchronously
    {
      get { return false; }
    }

    public bool IsCompleted
    {
      get { return _isCompleted; }
      set { _isCompleted = value; }
    }
  }
}
