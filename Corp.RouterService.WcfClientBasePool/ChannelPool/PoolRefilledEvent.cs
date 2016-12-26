
namespace System.ServiceModel.ChannelPool
{
    public delegate void PoolRefilledEvent(object sender, PoolRefilledEventArgs ea);

    public class PoolRefilledEventArgs : EventArgs
    {
        private int _proxiesAddedToPool = 0;
        private int _poolCountBeforeAdd = 0;

        public int ProxiesAddedToPool
        {
            get { return _proxiesAddedToPool; }
            set { _proxiesAddedToPool = value; }
        }

        public int PoolCountBeforeAdd
        {
            get { return _poolCountBeforeAdd; }
            set { _poolCountBeforeAdd = value; }
        }
    }
}
