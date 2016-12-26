using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HsmLibrary
{
  internal static class HeaderGenerator
  {
    private static int _counter = 0;
    internal static string GenerateHeader()
    {
      return Interlocked.Increment(ref _counter).ToString("D" + HeaderLength);
    }

    internal static int HeaderLength { get { return 12; } }
  }
}
