
namespace System.ServiceModel.ChannelPool
{
    /// <summary>
    /// This class acts as a container for the channel. It contains contextual information that is used to determine if the
    /// channel is valid and to prevent it from becoming 'faulted'.
    /// </summary>
    /// <typeparam name="TChannel">The communications channel.</typeparam>
    public class ChannelContext<TChannel> where TChannel : class
    {
        #region Private fields

        private DateTime _dateTimeOpened = DateTime.MinValue;
        private TChannel _channel = null;

        #endregion

        #region Public Fields

        public DateTime DateTimeOpened
        {
            get { return _dateTimeOpened; }
            set { _dateTimeOpened = value; }
        }

        public TChannel Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Nope, can't do this one
        /// </summary>
        private ChannelContext() {}

        /// <summary>
        /// The only way to create a ChannelContext is if we have a channel to go with it.
        /// </summary>
        /// <param name="channel"></param>
        public ChannelContext(TChannel channel)
        {
            _channel = channel;
            _dateTimeOpened = DateTime.Now;
        }

        #endregion
    }
}
