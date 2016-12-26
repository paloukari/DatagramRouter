
namespace System.ServiceModel.ChannelPool
{
    public delegate void ChannelRetrievedEvent(object sender, ChannelRetrievedEventArgs ea);

    public class ChannelRetrievedEventArgs : EventArgs
    {
        private int _remainingChannelsInPool = 0;

        public int RemainingChannelsInPool
        {
            get { return _remainingChannelsInPool; }
            set { _remainingChannelsInPool = value; }
        }

    }
}
