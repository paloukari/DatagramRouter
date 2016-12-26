using System.ServiceModel.Channels;

namespace System.ServiceModel.ChannelPool
{
    public abstract class ClientBasePool<TChannel> : ClientBase<TChannel>, IDisposable where TChannel : class
    {
        #region Private Fields

        private ChannelContext<TChannel> _channelContext = null;
        private ChannelFactory<TChannel> _channelFactory;
        private ChannelPool<TChannel> _channelPool = null;

        private bool _isInitialised = false;

        #endregion

        #region Constructors

        protected ClientBasePool()
        {
            //InitialiseChannel();
        }

        protected ClientBasePool(InstanceContext callbackInstance) : base(callbackInstance)
        {
            //InitialiseChannel();
        }
        protected ClientBasePool(string endpointConfigurationName) : base(endpointConfigurationName)
        {
            //InitialiseChannel();
        }
        protected ClientBasePool(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
        {
            //InitialiseChannel();
        }

        protected ClientBasePool(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName)
        {
            //InitialiseChannel();
        }

        protected ClientBasePool(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
        {
            //InitialiseChannel();
        }

        protected ClientBasePool(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
        {
            //InitialiseChannel();
        }

        protected ClientBasePool(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress)
        {
            //InitialiseChannel();
        }

        protected ClientBasePool(InstanceContext callbackInstance, string endpointConfigurationName, EndpointAddress remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
            //InitialiseChannel();
        }

        protected ClientBasePool(InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : base(callbackInstance, endpointConfigurationName, remoteAddress)
        {
            //InitialiseChannel();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The Pool ID of the underlying Channel Pool
        /// </summary>
        public Guid ChannelPoolID
        {
            get
            {
                return _channelPool.PoolID;
            }
        }

        /// <summary>
        /// The communication channel
        /// </summary>
        protected new TChannel Channel 
        {
            get
            {
                return CreateChannel();
            }
        }
        
        /// <summary>
        /// The channel factory used to create the channel
        /// </summary>
        public new ChannelFactory<TChannel> ChannelFactory
        {
            get { return _channelFactory; }
        }

        /// <summary>
        /// The Channel Context that is associated with the Channel. Contains contextual properties
        /// related to the channel poool.
        /// </summary>
        public ChannelContext<TChannel> ChannelContext
        {
            get { return _channelContext;}
        }

        #endregion

        #region InitialiseChannel method

        private void InitialiseChannel()
        {
            if (!_isInitialised)
            {
                // Ensure out channel pool is initialised
                //_channelPool = ChannelPoolFactory<TChannel>.GetPool();
                _channelPool = ChannelPoolFactory<TChannel>.GetPool(this.ClientCredentials);
                _channelFactory = _channelPool.ChannelFactory;
                _channelContext = _channelPool.GetChannelContext();
                _isInitialised = true;
            }
        }

        #endregion

        #region CreateChannel overriden method

        protected override TChannel CreateChannel()
        {
            //Note: We do a late channel initialisation as the client using the proxy can sometimes create the proxy, then set credentials before
            // accessing the service. We need to make sure that we capture the credentials that have been set PRIOR to initialising the proxy/channel pool
            // otherwise auth failures occur.
            InitialiseChannel();
            return _channelContext.Channel;
        }

        #endregion

        #region Open and close methods

        public new void Open()
        {
            // Do a double check just in case.
            if (_channelPool.IsChannelExpired(_channelContext))
            {
                // Our channel has expired since we obtained it within the constructor
                // so grab another
                _channelContext = _channelPool.GetChannelContext();
            }
                
            //System.Diagnostics.Debug.WriteLine(_channelPool.);
        }

        public new void Close()
        {
            ((IChannel) _channelContext.Channel).Close();
        }

        #endregion
    }
}
