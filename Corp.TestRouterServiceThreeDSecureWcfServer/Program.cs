using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corp.TestRouterServiceThreePleLayerSecurityWcfServer
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("(3)PleLayerSecurity?(3)");
      string data = Console.ReadLine();
      switch (data.ToLower())
      {
        case "h":
          HsmTest();
          break;
        case "o":
          OperationTest();
          break;
        case "i":
          InformationTest();
          break;
      }
    }
  }
}
