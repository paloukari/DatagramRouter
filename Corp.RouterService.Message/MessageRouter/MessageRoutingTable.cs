using System;
using System.Collections.Generic;
using System.Linq;

namespace Corp.RouterService.Message
{
    public class MessageRoutingTable
    {            
        List<RouterServicePredicate> _predicates = new List<RouterServicePredicate>();

        public void Add(RouterServicePredicate predicate)
        {
            _predicates.Add(predicate);
        }

        public Uri Route(Message message)
        {
            var match= _predicates.FirstOrDefault(p => p.CanHandleMessage(message));
            return match.Destination;
        }
    }
}
