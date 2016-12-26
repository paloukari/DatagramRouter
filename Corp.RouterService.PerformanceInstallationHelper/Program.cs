using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Configuration;

namespace Corp.RouterService.PerformanceInstallationHelper
{
  class Program
  {

    private static LoggingLibrary.Log4Net.ILog log = LoggingLibrary.LoggerManager.Log4NetConfigureAndGetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    static void Main(string[] args)
    {
      try
      {

        if (log.IsDebugEnabled)
          log.Debug("Starting..");
        string categoryName = "gxcvbgchxCorp HPC Router Server";

     

        var categoryNameSetting = ConfigurationManager.AppSettings["categoryName"];
        if (categoryNameSetting != null && !string.IsNullOrEmpty(categoryNameSetting))
        {
          categoryName = categoryNameSetting;
          if (log.IsDebugEnabled)
            log.Debug("Category loaded from config:" + categoryName);
        }

        if (args.Count() > 0)
        {
          if (args[0].ToLower() == "d")
          {
            //delete
            if (PerformanceCounterCategory.Exists(categoryName))
            {
              if (log.IsDebugEnabled)
                log.Debug(categoryName + " category does exist. Deleting..");
              PerformanceCounterCategory.Delete(categoryName);
            }
          }
          else if (args[0].ToLower() == "c")
          {
            //create
            if (!PerformanceCounterCategory.Exists(categoryName))
            {
              if (log.IsDebugEnabled)
                log.Debug(categoryName + " category does not exist. Creating..");
              CreatePerformanceCounters(categoryName);
            }
          }
        }      
      }
      catch (Exception ex)
      {
        if (log.IsErrorEnabled)
        {
          log.Error(ex.ToString());
        }
      }
      
      Console.WriteLine("Press any key to exit ...");
      Console.ReadKey();
      if (log.IsDebugEnabled)
        log.Debug("Exiting gracefully..");
    }

    static void CreatePerformanceCounters(string categoryName)
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


        if (!PerformanceCounterCategory.Exists(categoryName))
        {
          PerformanceCounterCategory.Create(categoryName,
              "The performance counters of the local RouterService",
              PerformanceCounterCategoryType.MultiInstance,
              countersCreationData);
          if (log.IsDebugEnabled)
            log.Debug("Counters created..");
        }
      }
      catch (Exception ex)
      {
        if (log.IsErrorEnabled)
        {
          log.Error(ex.ToString() + " at " + categoryName);
        }
      }
    }  
  }
}
