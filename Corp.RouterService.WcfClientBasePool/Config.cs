
namespace System.ServiceModel.ChannelPool
{
    /// <summary>
    /// As the name implies, the configuration setting class for the channel pool.
    /// </summary>
    public static class Config
    {
        #region Constants/defaults

        private const int DEFAULT_POOL_SIZE = 50 ;// max is 127 when using wsHttp (max number of pending Secure conversations) - but only if no method call is performed. If a method call is
                                                  // performed then it seems the SCT is ok and we can exceed this limit.
        private const int DEFAULT_CHANNEL_LIFETIME = 90;  // Defaults to 5 minutes approx before a channel is deemed end-of-life and needs to be re-initialised
        private const int DEFAULT_CLEANUP_INTERVAL = (DEFAULT_CHANNEL_LIFETIME / 2) - 2;  // Defaults to a little less than half the Channel lifetime.

        #endregion

        #region Private fields

        private static int _poolSize = DEFAULT_POOL_SIZE;
        private static int _cleanupInterval = DEFAULT_CLEANUP_INTERVAL;
        private static int _channelLifetime = DEFAULT_CHANNEL_LIFETIME;
        private static int _poolRefillTrigger = DEFAULT_POOL_SIZE/2;

        #endregion

        #region Public Properties

        /// <summary>
        /// The size of the channel pool.
        /// </summary>
        /// <remarks>Default is 50 and is generally optimal.</remarks>
        public static int PoolSize
        {
            get { return _poolSize; }
            set 
            {
                if (value > 0 && value < 127)
                {
                    _poolSize = value;
                    _poolRefillTrigger = _poolSize/2;
                }
                else
                    throw new Exception(Constants.EXCEPTION_EXCEEDED_MAXIMUM_POOL_SIZE);
            }
        }

        /// <summary>
        /// The threshold or trigger value used to determine when the channel pool should begin to be refilled.
        /// Defaults to half of the pool size.
        /// </summary>
        public static int PoolRefillTrigger
        {
            get { return _poolRefillTrigger; }
            set
            {
                if (value > 0 && value < _poolSize)
                    _poolRefillTrigger = value;
            }
        }



        /// <summary>
        /// The interval that determines how often a thread runs to check the validity of channels within the pool and
        /// to perform an cleanup actions.
        /// </summary>
        public static int CleanupInterval
        {
            get { return _cleanupInterval; }
            set { _cleanupInterval = value; }
        }

        /// <summary>
        /// The length of time (in seconds) before a channel stored in the pool is deemed at end-of-life and
        /// needs to be re-initialised.
        /// </summary>
        public static int ChannelLifetime
        {
            get { return _channelLifetime; }
            set 
            { 
                _channelLifetime = value;
                ResetCleanupInterval();
            }
        }

        #endregion

        #region ResetCleanupInterval

        private static void ResetCleanupInterval()
        {
            if (_channelLifetime > 0)
                _cleanupInterval = _channelLifetime/2;

            if (_cleanupInterval > 2)
                _cleanupInterval -= 2;
        }

        #endregion

    }
}
