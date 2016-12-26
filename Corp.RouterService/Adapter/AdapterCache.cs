using System.Collections.Generic;
using Corp.RouterService.Message;

namespace Corp.RouterService.Adapter
{
  internal static class AdapterCache
  {
    private static Dictionary<MessageEndpoint, IAdapter> _cache = new Dictionary<MessageEndpoint, IAdapter>();

    internal static bool Contains(MessageEndpoint endpoint, ref IAdapter adapter)
    {
      if (_cache.ContainsKey(endpoint))
      {
        adapter = _cache[endpoint];
        return true;
      }
      return false;
    }

    internal static void Add(MessageEndpoint endpoint, IAdapter adapter)
    {
      _cache.Add(endpoint, adapter);
    }

    internal static void Clear()
    {
      _cache.Clear();
    }
  }
}
