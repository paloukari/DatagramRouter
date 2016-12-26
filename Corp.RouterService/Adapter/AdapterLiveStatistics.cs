using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Corp.RouterService.Adapter
{
  internal class AdapterLiveStatistics
  {

    private static LoggingLibrary.Log4Net.ILog log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
          System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    AdapterSettings _settings;

    private PerformanceCounter _receivedMessagesPerformanceCounter;
    private PerformanceCounter _sentMessagesPerformanceCounter;
    private PerformanceCounter _receivedMessagesThroughputPerformanceCounter;
    private PerformanceCounter _sentMessagesThroughputPerformanceCounter;
    private bool _countersCreated = false;

    private AdapterLiveStatistics()
    {

    }
    public AdapterLiveStatistics(AdapterSettings settings)
    {
      _settings = settings;

      CreatePerformanceCounters(_settings.PreformanceCountersIntanceName);
    }

    private void CreatePerformanceCounters(string InstanceName)
    {
      try
      {
        var countersCreationData = new CounterCreationDataCollection();

        var cdReceivedMessages = new CounterCreationData() { CounterName = "ReceivedMessagesCount", CounterHelp = "This is received messages count", CounterType = PerformanceCounterType.NumberOfItems64 };
        var cdSentMessages = new CounterCreationData() { CounterName = "SentMessagesCount", CounterHelp = "This is sent messages count", CounterType = PerformanceCounterType.NumberOfItems64 };
        var cdReceivedMessagesThroughput = new CounterCreationData() { CounterName = "ReceivedMessagesCountThroughput", CounterHelp = "This is received messages count", CounterType = PerformanceCounterType.RateOfCountsPerSecond64 };
        var cdSentMessagesThroughput = new CounterCreationData() { CounterName = "SentMessagesCountThroughput", CounterHelp = "This is sent messages count", CounterType = PerformanceCounterType.RateOfCountsPerSecond64 };


        countersCreationData.Add(cdReceivedMessages);
        countersCreationData.Add(cdSentMessages);
        countersCreationData.Add(cdReceivedMessagesThroughput);
        countersCreationData.Add(cdSentMessagesThroughput);

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



        _receivedMessagesPerformanceCounter = new PerformanceCounter(categoryName, cdReceivedMessages.CounterName, InstanceName, false) { RawValue = 0 };
        _sentMessagesPerformanceCounter = new PerformanceCounter(categoryName, cdSentMessages.CounterName, InstanceName, false) { RawValue = 0 };
        _receivedMessagesThroughputPerformanceCounter = new PerformanceCounter(categoryName, cdReceivedMessagesThroughput.CounterName, InstanceName, false) { RawValue = 0 };
        _sentMessagesThroughputPerformanceCounter = new PerformanceCounter(categoryName, cdSentMessagesThroughput.CounterName, InstanceName, false) { RawValue = 0 };

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



    internal void IncrementReceivedMessages()
    {
      if(_countersCreated)
      {
        _receivedMessagesPerformanceCounter.Increment();
        _receivedMessagesThroughputPerformanceCounter.Increment();
      }      
    }


    internal void IncrementSentMessages()
    {
      if(_countersCreated)
      {
        _sentMessagesPerformanceCounter.Increment();
        _sentMessagesThroughputPerformanceCounter.Increment();
      }      
    }



    
  }
}
