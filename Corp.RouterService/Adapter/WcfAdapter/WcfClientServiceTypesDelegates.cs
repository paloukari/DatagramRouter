using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corp.RouterService.Adapter.WcfAdapter
{
  public class WcfClientServiceTypesDelegates : IServiceTypeDelegates
  {
    HandleMessage _delegates = null;
    public HandleMessage HandleMessageDelegate
    {
      get
      {
        return _delegates;
      }
      set
      {
        _delegates = value;
      }
    }
  }
}
