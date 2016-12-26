using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corp.RouterService.Adapter.WcfAdapter
{
  public interface IServiceType
  {   
    Type GetServiceType();
    IServiceTypeDelegates ServiceTypeDelegates { get; set; }
  }
}
