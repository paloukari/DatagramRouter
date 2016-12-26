
namespace System.ServiceModel.ChannelPool
{
    public delegate void ChannelPoolCollectedEvent(object sender, ChannelPoolCollectedEventArgs cpea);
    
    public class ChannelPoolCollectedEventArgs : EventArgs
    {
        #region Private Fields
        
        private int _itemsCollected = 0;
        
        #endregion

        #region Constructors
        
        private ChannelPoolCollectedEventArgs() {}

        public ChannelPoolCollectedEventArgs(int numberOfItemsCollected) : this()
        {
            _itemsCollected = numberOfItemsCollected;
        }

        #endregion

        #region Public Properties

        public int NumberOfItemsCollected
        {
            get { return _itemsCollected; }
        }

        #endregion
    }
}
