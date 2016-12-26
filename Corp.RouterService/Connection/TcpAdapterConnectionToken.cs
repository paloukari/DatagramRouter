using System;
using System.Threading;

namespace Corp.RouterService.Connection
{
    class RouterServiceConnectionToken
    {
        private static Int32 _connectionID= 0;
        private Int32 _tokenID = Interlocked.Increment(ref _connectionID);

        internal Int32 TokenID
        {
            get { return _tokenID; }            
        }
    }
}
