using System.ServiceModel.Description;

namespace System.ServiceModel.ChannelPool
{
    public static class ChannelPoolFactory<TChannel> where TChannel : class
    {
        private static ChannelPool<TChannel> _channelPool;

        static ChannelPoolFactory()
        {
            //CreateChannelPool();
        }

        public static ChannelPool<TChannel> GetPool()
        {
            CreateChannelPool();
            return _channelPool;
        }

        public static ChannelPool<TChannel> GetPool(System.ServiceModel.Description.ClientCredentials credentialsToUse)
        {
            CreateChannelPool(credentialsToUse);
            return _channelPool;
            //throw new NotImplementedException("Need to copy over the credentials to use to our channel factory.");
        }

        public static bool Initialise()
        {
            CreateChannelPool();
            if (_channelPool != null && _channelPool.PoolID != null)
                return true;
            else
                return false;
        }

        public static void Destroy()
        {
            _channelPool.Dispose();
            _channelPool = null;

        }

        private static void CreateChannelPool()
        {
            if (_channelPool == null)
                _channelPool = new ChannelPool<TChannel>();
        }

        private static void CreateChannelPool(ClientCredentials credentialsToUse)
        {
            if (_channelPool == null)
                _channelPool = new ChannelPool<TChannel>(credentialsToUse);
        }
    }
}
