using System;
using System.Diagnostics;
using System.Threading;
using Corp.RouterService.Common;

namespace Corp.RouterService.TcpServer
{
    class TcpServerLiveStatistics : ObservableProperty
    {
      private static LoggingLibrary.Log4Net.ILog log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        TcpServerSettings _settings;
        private TcpServerState _state;
        bool _countersCreated = false;
        private int _acceptedConnections = 0;
        private int _maxAcceptedConnections = 0;
        private Semaphore _connectionsThrottler;
        private PerformanceCounter _messagesPoolPerformanceCounter;
        private PerformanceCounter _connectionsPoolPerformanceCounter;

        private PerformanceCounter _receivedMessagesPerformanceCounter;
        private PerformanceCounter _sentMessagesPerformanceCounter;
        private PerformanceCounter _receivedMessagesThroughputPerformanceCounter;
        private PerformanceCounter _sentMessagesThroughputPerformanceCounter;
        private PerformanceCounter _activeConnectionsPerformanceCounter;

        private PerformanceCounter _sentMessagesBytesPerformanceCounter;
        private PerformanceCounter _sentMessagesBytesThroughputPerformanceCounter;

        private PerformanceCounter _receivedMessagesBytesPerformanceCounter;
        private PerformanceCounter _receivedMessagesBytesThroughputPerformanceCounter;

        private PerformanceCounter _dispatchedMessagesCountPerformanceCounter;


        internal Semaphore ConnectionsThrottler
        {
            get { return _connectionsThrottler; }
            set { _connectionsThrottler = value; }
        }

        private TcpServerLiveStatistics()
        {

        }
        internal TcpServerLiveStatistics(TcpServerSettings settings)
        {
            _settings = settings;

            _connectionsThrottler = new Semaphore(_settings.PoolSize, _settings.PoolSize);

            if (_settings.UsePerformanceCounters)
                CreatePerformanceCounters(_settings.PreformanceCountersIntanceName);
        }

        private void CreatePerformanceCounters(string InstanceName)
        {
          try
          {

            var countersCreationData = new CounterCreationDataCollection();
            var cdMessages = new CounterCreationData() { CounterName = "MessagesPoolCount", CounterHelp = "This is the incoming pool's SAEA objecs available", CounterType = PerformanceCounterType.NumberOfItems64 };
            //var cdOutgoing = new CounterCreationData() { CounterName = "OutgoingMessagesPoolCount", CounterHelp = "This is the outgoing pool's SAEA objecs available", CounterType = PerformanceCounterType.NumberOfItems64 };
            var cdConnections = new CounterCreationData() { CounterName = "ConnectionsMessagesPoolCount", CounterHelp = "This is the connections pool's SAEA objecs available", CounterType = PerformanceCounterType.NumberOfItems64 };
            var cdReceivedMessages = new CounterCreationData() { CounterName = "ReceivedMessagesCount", CounterHelp = "This is received messages count", CounterType = PerformanceCounterType.NumberOfItems64 };
            var cdSentMessages = new CounterCreationData() { CounterName = "SentMessagesCount", CounterHelp = "This is sent messages count", CounterType = PerformanceCounterType.NumberOfItems64 };
            var cdReceivedMessagesThroughput = new CounterCreationData() { CounterName = "ReceivedMessagesCountThroughput", CounterHelp = "This is received messages count", CounterType = PerformanceCounterType.RateOfCountsPerSecond64 };
            var cdSentMessagesThroughput = new CounterCreationData() { CounterName = "SentMessagesCountThroughput", CounterHelp = "This is sent messages count", CounterType = PerformanceCounterType.RateOfCountsPerSecond64 };

            var cdReceivedMessagesBytes = new CounterCreationData() { CounterName = "ReceivedMessagesBytesCount", CounterHelp = "This is received messages count", CounterType = PerformanceCounterType.NumberOfItems64 };
            var cdSentMessagesBytes = new CounterCreationData() { CounterName = "SentMessagesBytesCount", CounterHelp = "This is sent messages count", CounterType = PerformanceCounterType.NumberOfItems64 };
            var cdReceivedMessagesBytesThroughput = new CounterCreationData() { CounterName = "ReceivedMessagesBytesCountThroughput", CounterHelp = "This is received messages count", CounterType = PerformanceCounterType.RateOfCountsPerSecond64 };
            var cdSentMessagesBytesThroughput = new CounterCreationData() { CounterName = "SentMessagesBytesCountThroughput", CounterHelp = "This is the uptime sent messages count", CounterType = PerformanceCounterType.RateOfCountsPerSecond64 };

            var cdActiveConnections = new CounterCreationData() { CounterName = "ActiveConnections", CounterHelp = "This is the active connections count", CounterType = PerformanceCounterType.NumberOfItems64 };

            var cdDispatchedMessages = new CounterCreationData() { CounterName = "DispatchedMessages", CounterHelp = "This is the dispatched messages count", CounterType = PerformanceCounterType.NumberOfItems64 };

            countersCreationData.Add(cdMessages);
            //countersCreationData.Add(cdOutgoing);
            countersCreationData.Add(cdConnections);
            countersCreationData.Add(cdReceivedMessages);
            countersCreationData.Add(cdSentMessages);
            countersCreationData.Add(cdReceivedMessagesThroughput);
            countersCreationData.Add(cdSentMessagesThroughput);
            countersCreationData.Add(cdActiveConnections);

            countersCreationData.Add(cdReceivedMessagesBytes);
            countersCreationData.Add(cdSentMessagesBytes);
            countersCreationData.Add(cdReceivedMessagesBytesThroughput);
            countersCreationData.Add(cdSentMessagesBytesThroughput);
            countersCreationData.Add(cdDispatchedMessages);

            const string categoryName = "Corp Message Router";

            try
            {
              if (!PerformanceCounterCategory.Exists(categoryName))
                PerformanceCounterCategory.Create(categoryName,
                    "The performance counters of the local RouterService",
                    PerformanceCounterCategoryType.MultiInstance,
                    countersCreationData);
            }
            catch (Exception inEx)
            {
              if (log.IsErrorEnabled)
              {
                log.Error(inEx.ToString() + " at " + InstanceName);
              }
            }

            _messagesPoolPerformanceCounter = new PerformanceCounter(categoryName, cdMessages.CounterName, InstanceName, false) { RawValue = 0 };

            _connectionsPoolPerformanceCounter = new PerformanceCounter(categoryName, cdConnections.CounterName, InstanceName, false) { RawValue = 0 };
            _receivedMessagesPerformanceCounter = new PerformanceCounter(categoryName, cdReceivedMessages.CounterName, InstanceName, false) { RawValue = 0 };
            _sentMessagesPerformanceCounter = new PerformanceCounter(categoryName, cdSentMessages.CounterName, InstanceName, false) { RawValue = 0 };
            _receivedMessagesThroughputPerformanceCounter = new PerformanceCounter(categoryName, cdReceivedMessagesThroughput.CounterName, InstanceName, false) { RawValue = 0 };
            _sentMessagesThroughputPerformanceCounter = new PerformanceCounter(categoryName, cdSentMessagesThroughput.CounterName, InstanceName, false) { RawValue = 0 };
            _activeConnectionsPerformanceCounter = new PerformanceCounter(categoryName, cdActiveConnections.CounterName, InstanceName, false) { RawValue = 0 };

            _receivedMessagesBytesPerformanceCounter = new PerformanceCounter(categoryName, cdReceivedMessagesBytes.CounterName, InstanceName, false) { RawValue = 0 };
            _receivedMessagesBytesThroughputPerformanceCounter = new PerformanceCounter(categoryName, cdReceivedMessagesBytesThroughput.CounterName, InstanceName, false) { RawValue = 0 };

            _sentMessagesBytesPerformanceCounter = new PerformanceCounter(categoryName, cdSentMessagesBytes.CounterName, InstanceName, false) { RawValue = 0 };
            _sentMessagesBytesThroughputPerformanceCounter = new PerformanceCounter(categoryName, cdSentMessagesBytesThroughput.CounterName, InstanceName, false) { RawValue = 0 };

            _dispatchedMessagesCountPerformanceCounter = new PerformanceCounter(categoryName, cdDispatchedMessages.CounterName, InstanceName, false) { RawValue = 0 };

            _countersCreated = true;
          }
          catch (Exception ex)
          {
            if (log.IsErrorEnabled)
            {
              log.Error(ex.ToString() + " at " + InstanceName);
            }
          }
        }

        internal void IncrementAcceptedConnections()
        {
            if (!_countersCreated)
                return;
            _maxAcceptedConnections = Math.Max(_maxAcceptedConnections, Interlocked.Increment(ref _acceptedConnections));
        }

        internal void IncrementDispatchedMessages()
        {
            if (!_countersCreated)
                return;
            _dispatchedMessagesCountPerformanceCounter.Increment();
        }

        internal void DecrementAcceptedConnections()
        {
            if (!_countersCreated)
                return;
            Interlocked.Decrement(ref _acceptedConnections);
        }

        internal TcpServerState State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    NotifyPropertyChanged("State");
                }
            }
        }

        internal void IncrementActiveConnections()
        {
            if (!_countersCreated)
                return;
            _activeConnectionsPerformanceCounter.Increment();
        }
        internal void IncrementReceivedMessages()
        {
            if (!_countersCreated)
                return;
            _receivedMessagesPerformanceCounter.Increment();
            _receivedMessagesThroughputPerformanceCounter.Increment();
        }

        internal void IncrementReceivedMessagesBytes(int bytes)
        {
            if (!_countersCreated)
                return;
            _receivedMessagesBytesPerformanceCounter.IncrementBy(bytes);
            _receivedMessagesBytesThroughputPerformanceCounter.IncrementBy(bytes);
        }

        internal void IncrementSentMessages()
        {
            if (!_countersCreated)
                return;
            _sentMessagesPerformanceCounter.Increment();
            _sentMessagesThroughputPerformanceCounter.Increment();
        }

        internal void IncrementSentMessagesBytes(int bytes)
        {
            if (!_countersCreated)
                return;
            _sentMessagesBytesPerformanceCounter.IncrementBy(bytes);
            _sentMessagesBytesThroughputPerformanceCounter.IncrementBy(bytes);
        }

        internal void IncrementIncomingPoolSize()
        {
            if (!_countersCreated)
                return;
            _messagesPoolPerformanceCounter.Increment();
        }


        internal void IncrementConnectionsPoolSize()
        {
            if (!_countersCreated)
                return;
            _connectionsPoolPerformanceCounter.Increment();
        }

        internal void DecrementActiveConnections()
        {
            if (!_countersCreated)
                return;
            _activeConnectionsPerformanceCounter.Decrement();
        }

        internal void DecrementIncomingPoolSize()
        {
            if (!_countersCreated)
                return;
            _messagesPoolPerformanceCounter.Decrement();
        }



        internal void DecrementConnectionsPoolSize()
        {
            if (!_countersCreated)
                return;
            _connectionsPoolPerformanceCounter.Decrement();
        }
    }
}
