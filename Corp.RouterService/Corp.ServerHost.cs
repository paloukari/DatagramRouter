using System;
using System.Collections.Generic;
using System.Diagnostics;
using Corp.RouterService;
using Corp.RouterService.Adapter;
using Corp.RouterService.Adapter.MemoryAdapter;
using Corp.RouterService.Adapter.SqlAdapter;
using Corp.RouterService.Adapter.TcpAdapter;
using Corp.RouterService.Message;
using Corp.RouterService.TcpServer;
using Corp.RouterService.Adapter.WcfAdapter;


namespace Corp.ServerHost
{
  public static class ServerHost
  {

    readonly static string categoryName = "Corp Message Router";

    private static LoggingLibrary.Log4Net.ILog log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private static List<TcpServer> _tcpServers = new List<TcpServer>();
    private static List<IAdapter> _servers = new List<IAdapter>();
    private static List<IAdapter> _clients = new List<IAdapter>();

    static bool _shouldInitializeBeforeStarting = true;

    public static void StartServer()
    {
      try
      {
        if (log.IsDebugEnabled)
        {
          log.Debug("Starting Server Host..");
        }

        if (_shouldInitializeBeforeStarting)
          InitializeServer();

        foreach (var client in _clients)
        {
          client.Start();
        }

        foreach (var server in _servers)
        {
          server.Start();
        }

        foreach (var server in _tcpServers)
        {
          server.Start();
        }
        
        if (log.IsDebugEnabled)
        {
          log.Debug("Started Server Host.");
        }
      }
      catch (Exception ex)
      {
        if (log.IsErrorEnabled)
        {
          log.Error(ex.ToString());
        }
      }
    }

    public static void StopServer()
    {
      try
      {
        if (log.IsInfoEnabled)
        {
          log.Warn("Stopping Server");
        }

        _shouldInitializeBeforeStarting = true;
        
        foreach (var server in _tcpServers)
        {
          server.Stop();
          server.Dispose();
        }

        foreach (var server in _servers)
        {
          server.Stop();
          server.Dispose();
        }

        foreach (var client in _clients)
        {
          client.Stop();
          client.Dispose();
        }

        
        int timeToSleep = 5;
        if (log.IsInfoEnabled)
        {
          log.Info("Waiting " + timeToSleep + "s for all components to complete");
        }

        System.Threading.Thread.Sleep(timeToSleep * 1000);


        if (log.IsInfoEnabled)
        {
          log.Info(timeToSleep + "s completed. Closing server..");
        }

        _clients = new List<IAdapter>();
        _servers = new List<IAdapter>();
        _tcpServers = new List<TcpServer>();
        AdapterCache.Clear();
        GC.Collect(0, GCCollectionMode.Forced);

        System.Threading.Thread.Sleep(1000);
      }

      catch (Exception ex)
      {
        if (log.IsErrorEnabled)
        {
          log.Error(ex.ToString());
        }
      }
    }

    private static IAdapter CreateWcfServer(RouterService.Adapter.WcfAdapter.WcfServerAdapterSettings serverSettings)
    {
      var adapter = new WcfServerAdapter(serverSettings);
      var endp = new MessageEndpoint(new Uri(@"wcf://" + serverSettings.Name));
      AdapterCache.Add(endp, adapter);
      return adapter;
    }

    private static IAdapter CreateWcfClient(RouterService.Adapter.WcfAdapter.WcfClientAdapterSettings clientSettings)
    {
      var adapter = new WcfClientAdapter(clientSettings);
      var endp = new MessageEndpoint(new Uri(@"wcf://" + clientSettings.Name));
      AdapterCache.Add(endp, adapter);
      return adapter;
    }

    private static IAdapter CreateMemClient(RouterService.Adapter.MemoryAdapter.MemClientSettings clientSettings)
    {
      var adapter = new MemoryAdapter(clientSettings);

      var endp = new MessageEndpoint(new Uri(@"mem://" + clientSettings.Name));
      AdapterCache.Add(endp, adapter);
      return adapter;
    }

    private static IAdapter CreateSqlClient(RouterService.Adapter.SqlAdapter.SqlClientSettings clientSettings)
    {
      var adapter = new SqlAdapter(clientSettings);

      var endp = new MessageEndpoint(new Uri(@"sql://" + clientSettings.Name));
      AdapterCache.Add(endp, adapter);
      return adapter;
    }

    private static IAdapter CreateTcpClient(RouterService.Adapter.TcpAdapter.TcpClientSettings clientSettings)
    {
      var adapter = new TcpAdapter(clientSettings);

      var endp = new MessageEndpoint(new Uri(@"tcp://" + clientSettings.RemoteEndPoint.Address.ToString() + ":" + clientSettings.RemoteEndPoint.Port));
      AdapterCache.Add(endp, adapter);
      return adapter;
    }

    public static void InitializeServer()
    {
      if (log.IsInfoEnabled)
      {
        log.Info("Initializing Server Host..");
      }
      try
      {

        if (!PerformanceCounterCategory.Exists(categoryName))
        {
          if (log.IsWarnEnabled)
          {
            log.Warn(categoryName + " does not exist...");
          }
        }
      }
      catch (Exception ex)
      {
        if (log.IsErrorEnabled)
        {
          log.Error(ex.ToString() + " at " + categoryName);
        }
      }

      _shouldInitializeBeforeStarting = false;

      RouterServiceSettings settings = new RouterServiceSettings();

      foreach (var tcpServerSettings in settings.RouterServicesSettings)
      {
        var server = new TcpServer(tcpServerSettings);

        server.Initialize();

        _tcpServers.Add(server);
      }

      foreach (var sqlServerSettings in settings.SqlServersSettings)
      {
        var server = new Corp.RouterService.Adapter.SqlAdapter.SqlAdapter(sqlServerSettings);

        server.Init();

        _servers.Add(server);

      }

      foreach (var clientSettings in settings.TcpClientsSettings)
      {
        var client = CreateTcpClient(clientSettings);
        
        client.Init();

        _clients.Add(client);
      }

      foreach (var clientSettings in settings.SqlClientsSettings)
      {
        var client = CreateSqlClient(clientSettings);

        client.Init();

        _clients.Add(client);
      }

      foreach (var clientSettings in settings.MemClientsSettings)
      {
        var client = CreateMemClient(clientSettings);

        client.Init();

        _clients.Add(client);
      }

      foreach (var clientSettings in settings.WcfClientsSettings)
      {
        var client = CreateWcfClient(clientSettings);

        client.Init();

        _clients.Add(client);
      }

      foreach (var serverSettings in settings.WcfServersSettings)
      {
        var server = CreateWcfServer(serverSettings);

        server.Init();

        _servers.Add(server);
      }
    }
  
    public static void InstallPerformanceCounters()
    {

      if (log.IsDebugEnabled)
        log.Debug("installing performance counters category " + categoryName);
      try
      {
        if (!PerformanceCounterCategory.Exists(categoryName))
        {
          if (log.IsDebugEnabled)
            log.Debug(categoryName + " category does not exist. Creating..");
          CreatePerformanceCounters(categoryName);
        }
      }
      catch (Exception ex)
      {
        if (log.IsErrorEnabled)
        {
          log.Error(ex.ToString());
          return;
        }
      }
      if (log.IsDebugEnabled)
        log.Debug("Success!  Exiting gracefully..");
    }

    public static void UninstallPerformanceCounters()
    {

      if (log.IsDebugEnabled)
        log.Debug("uninstalling performance counters category " + categoryName);
      try
      {

        if (PerformanceCounterCategory.Exists(categoryName))
        {
          if (log.IsDebugEnabled)
            log.Debug(categoryName + " category does exist. Deleting..");
          PerformanceCounterCategory.Delete(categoryName);
        }
      }
      catch (Exception ex)
      {
        if (log.IsErrorEnabled)
        {
          log.Error(ex.ToString());
          return;
        }
      }
      if (log.IsDebugEnabled)
        log.Debug("Success!  Exiting gracefully..");
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
        throw;
      }
    }
  }
}