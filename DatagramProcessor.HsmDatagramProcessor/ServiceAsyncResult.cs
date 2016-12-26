using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatagramProcessor.HsmDatagramProcessor
{
  public class ServiceAsyncResult : IAsyncResult
  {
    private object state;        
    private bool _isCompleted;
    private System.Threading.AutoResetEvent _waitHandle = new System.Threading.AutoResetEvent(true);    
    private AntigonisTypes.HsmMessageRequest hsmRequest;


    public ServiceAsyncResult(object state)
    {
      _isCompleted = false;
      this.state = state;
    }        

    

    public ServiceAsyncResult(object state, AntigonisTypes.HsmMessageRequest hsmRequest)
    {      
      this.state = state;
      this.hsmRequest = hsmRequest;
    }
    public object AsyncState
    {
      get { return state; }
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
      set { _isCompleted = value;}
    }
  }
}
