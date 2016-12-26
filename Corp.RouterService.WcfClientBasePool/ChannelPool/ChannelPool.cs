using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel.Channels;
using System.Threading;
using ThreadState = System.Threading.ThreadState;
using Timer=System.Timers.Timer;

namespace System.ServiceModel.ChannelPool
{
    /// <summary>
    /// The ChannelPool implementation for a particular Channel/Service. Instantiated by the ChannelPoolFactory
    /// </summary>
    /// <typeparam name="TChannel"></typeparam>
    public class ChannelPool<TChannel> : IDisposable where TChannel : class
    {
        #region Private fields

        private List<ChannelContext<TChannel>> _channelList = new List<ChannelContext<TChannel>>(Config.PoolSize);

        private ChannelFactory<TChannel> _channelFactory;

        private static Timer _timer = null;
        private object _repopulateLock = new object();
        private Guid _poolId = Guid.NewGuid();

        private int _poolRefillTrigger = Config.PoolRefillTrigger;

        private bool _runRefillThread = true;
        private bool _runCleanupTask = true;
        private Thread _repopulateThread = null;
        private Thread _cleanupThread = null;

        #endregion

        #region Events

        /// <summary>
        /// The Pool Refilled event. When the pool is refilled asynchronously, this event is fired.
        /// </summary>
        public event PoolRefilledEvent PoolRefilled;

        public event ChannelRetrievedEvent ChannelRetrieved;
        public event ChannelPoolCollectedEvent ChannelPoolCollected;

        #endregion

        #region Constructor

        /// <summary>
        /// Duh. Default Constructor
        /// </summary>
        public ChannelPool()
        {
            _channelFactory = new ChannelFactory<TChannel>("*");

            RefillPool();
            StartRepopulateProxyPoolThread();
        }

        public ChannelPool(System.ServiceModel.Description.ClientCredentials credentialsToUse)
        {
            _channelFactory = new ChannelFactory<TChannel>("*");
            _channelFactory.Credentials.UserName.UserName = credentialsToUse.UserName.UserName;
            _channelFactory.Credentials.UserName.Password = credentialsToUse.UserName.Password;
            _channelFactory.Credentials.Windows.ClientCredential = credentialsToUse.Windows.ClientCredential;
            _channelFactory.Credentials.Windows.AllowNtlm = credentialsToUse.Windows.AllowNtlm;

            RefillPool();
            StartRepopulateProxyPoolThread();
        }

        public ChannelFactory<TChannel> ChannelFactory
        {
            get
            {
                if (_channelFactory == null || _channelFactory.State == CommunicationState.Closed || _channelFactory.State == CommunicationState.Faulted)
                    _channelFactory = new ChannelFactory<TChannel>("*");
                return _channelFactory;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// A general description text about the ChannelPool instance.
        /// </summary>
        public string Description
        {
            get
            {
                return string.Format("WCF Channel Pool: Id='{0}', Type='{1}', Capacity='{2}', Current Pool Size='{3}'",
                                     this._poolId,
                                     typeof (TChannel).ToString(),
                                     Config.PoolSize,
                                     _channelList.Count);
            }
        }

        public Guid PoolID
        {
            get { return _poolId; }
        }

        #endregion

        #region Get Channel from the Pool Methods.

        /// <summary>
        /// Grab a channel from the pool.
        /// </summary>
        /// <returns></returns>
        public TChannel GetChannel()
        {
            TChannel chann = null;
            ChannelContext<TChannel> ctxt = GetChannelContext();
            if (ctxt != null)
                chann = ctxt.Channel;

            return chann;
        }

        /// <summary>
        /// Returns the Channel Context object which encapsulates the channel itself.
        /// </summary>
        /// <returns></returns>
        public ChannelContext<TChannel> GetChannelContext()
        {
            ChannelContext<TChannel> channCtxt = null;
            if (Monitor.TryEnter(_repopulateLock, Constants.LOCK_ATTEMPT_PERIOD))
            {
                bool channelRetrieved = false;
                try
                {
                    if (_channelList.Count == 0)
                        RefillPool(false);

                    while (channCtxt == null)
                    {
                        //chan = _channelList[0];
                        ChannelContext<TChannel> tmpCtxt = _channelList[0];
                        // Double check the lease time.
                        if (!IsChannelExpired(tmpCtxt))
                        {
                            channCtxt = tmpCtxt;
                            channelRetrieved = true;
                        }

                        // whether expired or not, The channel must be removed from the pool
                        _channelList.RemoveAt(0);
                    }

                    // If all the channels have expired, in this tight loop we may need to refill the pool.
                    if (_channelList.Count == 0)
                        RefillPool(false);

                    if (channCtxt == null)
                        throw new Exception(Constants.EXCEPTION_CANNOT_OBTAIN_CHANNEL_FROM_POOL);
                }
                finally
                {
                    Monitor.Exit(_repopulateLock);
                    if (channelRetrieved)
                    {
                        ChannelRetrievedEventArgs cea = new ChannelRetrievedEventArgs();
                        cea.RemainingChannelsInPool = _channelList.Count;
                        OnChannelRetrieved(cea);
                    }
                }
                return channCtxt;
            }
            else
                throw new Exception(Constants.EXCEPTION_CANNOT_OBTAIN_LOCK_GETCHANNEL);
        }

        #endregion

        #region IsChannelExpired

        /// <summary>
        /// Is the channel supplied still valid and not past its end-of-life according to
        /// the configured interval time.
        /// </summary>
        /// <param name="ctxt"></param>
        /// <returns></returns>
        public bool IsChannelExpired(ChannelContext<TChannel> ctxt)
        {
            bool channelValid = false;
            DateTime expiryTime = DateTime.Now.AddSeconds(Config.ChannelLifetime*-1);
            if (ctxt.DateTimeOpened > expiryTime)
                channelValid = true;

            return !channelValid;
        }

        #endregion

        #region Async methods to refill the proxy pool

        /// <summary>
        /// Kicks of the asynchronous process to refill the proxy pool with the given amount of proxies.
        /// </summary>
        /// <remarks>This does not use a Timer object to periodically kick of the process as the Timer object
        /// used Threads from the ThreadPool. We spawn dedicate threads so that the threadpool is not left with
        /// 2 less threads as a result of using this class.</remarks>
        private void StartRepopulateProxyPoolThread()
        {
            ThreadStart ts = new ThreadStart(InitiateRepopulatePoolProcess);
            _repopulateThread = new Thread(ts);
            _repopulateThread.Name = "Channel Pool Repopulation Thread";
            _repopulateThread.Priority = ThreadPriority.BelowNormal;
            _repopulateThread.IsBackground = true;

            // Kick off our background thread to do the Proxy pool repopulation
            if (_runRefillThread)
                _repopulateThread.Start();

            // Also kick of the cleanup timer
            ThreadStart chkTs = new ThreadStart(PoolCleanup);
            _cleanupThread = new Thread(chkTs);
            _cleanupThread.Name = "ChannelPool Cleanup Thread";
            _cleanupThread.IsBackground = true;
            _cleanupThread.Priority = ThreadPriority.Lowest;

            if (_runCleanupTask)
                _cleanupThread.Start();
        }


        /// <summary>
        /// Default, Non paramterised method to initiate the "repopulate channel pool" process.
        /// </summary>
        private void InitiateRepopulatePoolProcess()
        {
            RepopulateProxyPoolAction(null);
        }

        /// <summary>
        /// The method that is called asynchronously to refill the proxy pool.
        /// </summary>
        /// <param name="state">Optional state to pass onto the repopulation method. Currently unused - reserved for future use.</param>
        private void RepopulateProxyPoolAction(object state)
        {
            int oldPoolCount = _channelList.Count;
            int oldProxiesToAddCount = 0;
            bool poolFilled = false;

            while (_runRefillThread)
            {
                if (Config.PoolSize - _channelList.Count >= _poolRefillTrigger)
                {
                    oldProxiesToAddCount = Config.PoolSize - _channelList.Count;

                    RefillPool();
                    poolFilled = true;
                }

                if (poolFilled)
                {
                    // Fire the event
                    PoolRefilledEventArgs pea = new PoolRefilledEventArgs();
                    pea.PoolCountBeforeAdd = oldPoolCount;
                    pea.ProxiesAddedToPool = oldProxiesToAddCount;
                    OnPoolRefilled(pea);
                    poolFilled = false;
                }

                oldPoolCount = _channelList.Count;
            }

            DebugMessage("...exiting refill thread loop.");
        }

        public void RefillPool()
        {
            RefillPool(true);
        }

        /// <summary>
        /// The simple routine to refill the channel pool with opened channels.
        /// </summary>
        private void RefillPool(bool withLock)
        {
            DebugMessage("In RefillPool action");

            bool inSync = false;
            ChannelContext<TChannel> chanContext = null;

            try
            {
                int limit = Config.PoolSize - _channelList.Count;

                // If the pool is empty, lets obtain a lock and do a full refill
                if (_channelList.Count == 0)
                {
                    if (withLock)
                    {
                        if (!Monitor.TryEnter(_repopulateLock, Constants.LOCK_ATTEMPT_PERIOD))
                            throw new Exception(Constants.EXCEPTION_CANNOT_OBTAIN_LOCK_REFILL);
                        inSync = true;
                    }
                    limit = Config.PoolSize - _channelList.Count;
                    
                }

                DebugMessage(string.Format("Number of Channels to RefillPool with: {0}",limit));

                for (int loop = 0; loop < limit; loop++)
                {
                    if (_channelList.Count <= Config.PoolSize)  // This is just a safeguard.
                    {
                        //TChannel chann = _channelFactory.CreateChannel();
                        TChannel chann = ChannelFactory.CreateChannel();
                        if (chann == null)
                            throw new Exception(Constants.EXCEPTION_CANNOT_CREATE_CHANNEL);

                        chanContext = new ChannelContext<TChannel>(chann);
                        try
                        {
                            ((ICommunicationObject) chann).Faulted += new EventHandler(ChannelPool_Faulted);
                            ((IChannel) chann).Open();
                            _channelList.Add(chanContext);
                        }
                        catch (CommunicationException ce)
                        {
                            // Typically, the initiation of the secure conversation has failed at this point as the server cannot process any more pending
                            // Secure conversations requirests.
                            string msg =
                                string.Format("Error opening a channel to add to the pool. [{0}\n{1}]", ce.Message,
                                              ce.InnerException != null ? ce.InnerException.Message : "");
                            DebugMessage(msg);
                            throw;
                            //break;
                        }
                    }


                    // If we didn't get a lock in the first place when adding in some proxies, and suddenly load increased and exhausted our pool
                    // then we check here
                    if (_channelList.Count == 0)
                    {
                        DebugMessage("Attempting secondary lock in refill...");
                        if (withLock)
                        {
                            if (!Monitor.TryEnter(_repopulateLock, Constants.LOCK_ATTEMPT_PERIOD))
                                throw new Exception("Canot get lock for refill!");
                            inSync = true;
                        }
                    }
                }
            }
            finally
            {
                if (inSync && withLock)
                    Monitor.Exit(_repopulateLock);
            }
        }

        void ChannelPool_Faulted(object sender, EventArgs e)
        {
            DebugMessage("A Channel was Faulted!!!!");
        }

        #endregion

        #region DebugMessage

        private void DebugMessage(string msg)
        {
            string debugMsg =
                string.Format("[{0} {1}] - {2}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), msg);
            Debug.WriteLine(msg);
        }

        #endregion

        #region PoolCleanup

        /// <summary>
        /// The callback method that scans the pool for any end-of-life channels and purges them
        /// from the pool. This routine does not hold any locks so has to be pretty resilient about
        /// accessing and disposing of the channels which may well be used or removed at the time the
        /// cleanup occurs.
        /// </summary>
        //private void PoolCleanup(object sender, ElapsedEventArgs e)
        private void PoolCleanup()
        {
            Debug.WriteLine("in PoolCleanup event....");

            while (_runCleanupTask)
            {
                Thread.Sleep(Config.CleanupInterval * 1000);

                int currentChannelCount = 0;
                lock (_repopulateThread)
                {
                    DebugMessage("Cleanup process is checking pool status");

                    if (_channelList.Count > 0)
                    {
                        int latestCount = _channelList.Count - 1;

                        for (int cnt = latestCount; cnt >= 0; cnt--)
                        {
                            try
                            {
                                ChannelContext<TChannel> ch = _channelList[cnt];
                                if (IsChannelExpired(ch))
                                {
                                    DebugMessage(
                                        string.Format(
                                            "channel found that needs to be removed. ChannelOpened:{0}, Current Time:{1}",
                                            ch.DateTimeOpened.ToShortTimeString(), DateTime.Now.ToShortTimeString()));

                                    // needs to be purged.
                                    try
                                    {
                                        _channelList.RemoveAt(cnt);
                                        currentChannelCount++;
                                        if (((ICommunicationObject) ch.Channel).State == CommunicationState.Opened || ((ICommunicationObject) ch.Channel).State == CommunicationState.Created)
                                            ((ICommunicationObject) ch.Channel).Close();
                                    }
                                    catch (CommunicationException ce)
                                    {
                                        // do nothing here as well, the channel was already in a faulted state.
                                        DebugMessage("Channel to be removed was already in a faulted state");
                                    }
                                    catch (Exception ex)
                                    {
                                        // To get here means the number of channels in the pool was exhaused in between accessing the
                                        // initial element, so we leave things as they are and let the asynch process refill the pool.
                                        DebugMessage("Unable to remove channel. [" + ex.Message + "]");
                                    }
                                }
                            }
                            catch
                            {
                                // and again, we dont care if we are out of range
                                continue;
                            }
                        }
                    }
                }

                // Fire the ChannelPoolCollected event.
                ChannelPoolCollectedEventArgs cpea = new ChannelPoolCollectedEventArgs(currentChannelCount);
                OnChannelPoolCollected(cpea);
            }
        }

        #endregion

        #region Event Helper method to fire events.

        /// <summary>
        /// Fire the Pool Refilled event if assigned
        /// </summary>
        /// <param name="pea"></param>
        protected void OnPoolRefilled(PoolRefilledEventArgs pea)
        {
            if (PoolRefilled != null)
                PoolRefilled(this, pea);
        }

        /// <summary>
        /// Fire the Channel retrieved event when a channel is removed from the pool
        /// </summary>
        /// <param name="cea"></param>
        protected void OnChannelRetrieved(ChannelRetrievedEventArgs cea)
        {
            if (ChannelRetrieved != null)
                ChannelRetrieved(this, cea);
        }

        /// <summary>
        /// Fires the Channel Pool Collected event after the timer fires and a pool cleanup/collection
        /// has occured.
        /// </summary>
        /// <param name="cpea"></param>
        protected void OnChannelPoolCollected(ChannelPoolCollectedEventArgs cpea)
        {
            if (ChannelPoolCollected != null)
                ChannelPoolCollected(this, cpea);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _runRefillThread = false;
            _runCleanupTask = false;

            Thread.Sleep(200);
            if (_cleanupThread != null)
            {
                if (_cleanupThread.ThreadState == ThreadState.Running && _cleanupThread.IsAlive)
                    _cleanupThread.Abort();
            }
            if (_repopulateThread != null)
            {
                if (_repopulateThread.ThreadState == ThreadState.Running && _repopulateThread.IsAlive)
                    _repopulateThread.Abort();
            }

            _channelList.ForEach(delegate(ChannelContext<TChannel> c)
                                     {
                                         IChannel ic = c.Channel as IChannel;
                                         if (ic != null)
                                         {
                                             if (ic.State == CommunicationState.Opened)
                                                 ic.Close();
                                         }
                                     });

            _channelList.Clear();

            _channelFactory.Close();
        }

        #endregion
    }
}