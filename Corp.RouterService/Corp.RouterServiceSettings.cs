using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Corp.RouterService.Adapter.MemoryAdapter;
using Corp.RouterService.Adapter.SqlAdapter;
using Corp.RouterService.Adapter.TcpAdapter;
using Corp.RouterService.Message;
using Corp.RouterService.TcpServer;
using Corp.RouterService.Adapter.WcfAdapter;

namespace Corp.RouterService
{
  public class RouterServiceSettings
  {
    private MessageRoutingTable _predicates = new MessageRoutingTable();

    private Dictionary<string, Dictionary<string, string>> _adapterSettings = new Dictionary<string, Dictionary<string, string>>();


    private List<TcpServerSettings> _tcpServersSettings = new List<TcpServerSettings>();
    public List<TcpServerSettings> RouterServicesSettings
    {
      get { return _tcpServersSettings; }
    }


    private List<SqlServerSettings> _sqlServersSettings = new List<SqlServerSettings>();
    public List<SqlServerSettings> SqlServersSettings
    {
      get { return _sqlServersSettings; }
    }

    private List<TcpClientSettings> _tcpClientsSettings = new List<TcpClientSettings>();
    public List<TcpClientSettings> TcpClientsSettings
    {
      get { return _tcpClientsSettings; }
    }

    private List<SqlClientSettings> _sqlClientsSettings = new List<SqlClientSettings>();
    public List<SqlClientSettings> SqlClientsSettings
    {
      get { return _sqlClientsSettings; }
    }

    private List<MemClientSettings> _memClientsSettings = new List<MemClientSettings>();
    public List<MemClientSettings> MemClientsSettings
    {
      get { return _memClientsSettings; }
    }


    private List<WcfServerAdapterSettings> _wcfServersSettings = new List<WcfServerAdapterSettings>();
    public List<WcfServerAdapterSettings> WcfServersSettings
    {
      get { return _wcfServersSettings; }
    }

    private List<WcfClientAdapterSettings> _wcfClientsSettings = new List<WcfClientAdapterSettings>();
    public List<WcfClientAdapterSettings> WcfClientsSettings
    {
      get { return _wcfClientsSettings; }
    }



    public RouterServiceSettings()
    {
      var section = System.Configuration.ConfigurationManager.GetSection("serverConfiguration")
       as Corp.RouterService.Configuration.ServerConfigurationSection;

      int messageAuditLength = section.MessageAuditLength;

      if (section == null || section.RoutingElements == null || section.TcpServerElements == null || section.ClientElements == null)
        throw new ConfigurationErrorsException("Incomplete configuration found.");
      
      foreach (var item in section.RoutingElements)
      {
        var routingElement = item as Corp.RouterService.Configuration.ServerConfigurationSection.RoutingElement;
        if (_predicates != null && routingElement != null && routingElement.IsEnabled)
          _predicates.Add(new RouterServicePredicate((MessageType)Enum.Parse(typeof(MessageType), routingElement.MessageType), routingElement.OriginLocalAddress, routingElement.OriginRemoteAddress, routingElement.DestinationAddress, routingElement.IncludeDiagnostics));
      }

      foreach (var item in section.AdapterSettingElements)
      {
        var settingsElement = item as Corp.RouterService.Configuration.ServerConfigurationSection.AdapterSettingsElement;
        if (_adapterSettings != null && settingsElement != null && settingsElement.IsEnabled)
        {
          if (!_adapterSettings.ContainsKey(settingsElement.SettingsGroup))
            _adapterSettings[settingsElement.SettingsGroup] = new Dictionary<string, string>();

          _adapterSettings[settingsElement.SettingsGroup].Add(settingsElement.SettingsKey, settingsElement.SettingsValue);
        }
      }



      //tcp servers
      foreach (var item in section.TcpServerElements)
      {
        var tcpServerElement = item as Corp.RouterService.Configuration.ServerConfigurationSection.TcpServerElement;

        if (_tcpServersSettings != null && tcpServerElement != null && tcpServerElement.IsEnabled)
        {

          List<TcpMessageSettings> receiveTcpMessagesSettings = ReadTcpMessagesSettings(tcpServerElement.IncomingTrafficSettings.TcpMessagesSetttingsElements);
          List<TcpMessageSettings> sendTcpMessagesSettings = ReadTcpMessagesSettings(tcpServerElement.OutgoingTrafficSettings.TcpMessagesSetttingsElements);

          _tcpServersSettings.Add(new TcpServerSettings()
          {            
            MessageAuditLength=messageAuditLength,
            Name = tcpServerElement.Name,
            LocalEndPoint = new System.Net.IPEndPoint(IPAddress.Parse(tcpServerElement.LocalEndpoint.IP), tcpServerElement.LocalEndpoint.Port),
            PoolSize = tcpServerElement.PoolSize,
            ConnectionsBacklog = tcpServerElement.ConnectionsBacklog,
            IncomingDirectionSettings = new TcpTrafficSettings(tcpServerElement.IncomingTrafficSettings.NetworkBufferSize,
              receiveTcpMessagesSettings),

            OutgoingDirectionSettings = new TcpTrafficSettings(tcpServerElement.OutgoingTrafficSettings.NetworkBufferSize,
              sendTcpMessagesSettings),

            RoutingTable = _predicates,
            UsePerformanceCounters = tcpServerElement.UsePerformanceCounters,
            AppSettings = _adapterSettings
          });
        }
      }


      //sql servers
      foreach (var item in section.SqlServerElements)
      {
        var sqlServerElement = item as Corp.RouterService.Configuration.ServerConfigurationSection.SqlServerElement;
        if (_sqlServersSettings != null && sqlServerElement != null && sqlServerElement.IsEnabled)
        {
          string connectionString = null;
          if (ConfigurationManager.ConnectionStrings[sqlServerElement.ConnectionString] != null)
            connectionString = ConfigurationManager.ConnectionStrings[sqlServerElement.ConnectionString].ConnectionString;
          else
            throw new Exception("Cannot find connection string " + sqlServerElement.ConnectionString);

          _sqlServersSettings.Add(new SqlServerSettings()
          {
            MessageAuditLength = messageAuditLength,
            Name = sqlServerElement.Name,
            ConnectionString = connectionString,
            PollingTimeout = sqlServerElement.PollingPeriodInMilliseconds,
            RoutingTable = _predicates,
            Guid = sqlServerElement.Guid,
            MessageType = (MessageType)Enum.Parse(typeof(MessageType), sqlServerElement.MessageType),
            UsePerformanceCounters = sqlServerElement.UsePerformanceCounters,
            AppSettings = _adapterSettings
          });
        }
      }

      //wcf servers
      foreach (var item in section.WcfServerElements)
      {
        var wcfServerElement = item as Corp.RouterService.Configuration.ServerConfigurationSection.WcfServerElement;
        if (_wcfServersSettings != null && wcfServerElement != null && wcfServerElement.IsEnabled)
        {                    

          _wcfServersSettings.Add(new WcfServerAdapterSettings()
          {
            MessageAuditLength = messageAuditLength,
            Name = wcfServerElement.Name,            
            TimeoutPeriodInMilliseconds= wcfServerElement.TimeoutPeriodInMilliseconds,
            RoutingTable = _predicates,
            Guid = wcfServerElement.Guid,
            ServerURI = wcfServerElement.ServerURI,
            MessageType = (MessageType)Enum.Parse(typeof(MessageType), wcfServerElement.MessageType),
            UsePerformanceCounters = wcfServerElement.UsePerformanceCounters,
            AppSettings = _adapterSettings
          });
        }
      }


      //client 
      foreach (var item in section.ClientElements)
      {
        var clientElement = item as Corp.RouterService.Configuration.ServerConfigurationSection.ClientElement;

        switch (clientElement.Type)
        {
          case "tcp":
            {

              var tcpClientElement = clientElement.TcpClient;

              if (_tcpClientsSettings != null && tcpClientElement != null && tcpClientElement.IsEnabled)
              {
                List<TcpMessageSettings> receiveTcpMessagesSettings = ReadTcpMessagesSettings(tcpClientElement.IncomingTrafficSettings.TcpMessagesSetttingsElements);
                List<TcpMessageSettings> sendTcpMessagesSettings = ReadTcpMessagesSettings(tcpClientElement.OutgoingTrafficSettings.TcpMessagesSetttingsElements);

                _tcpClientsSettings.Add(new TcpClientSettings
                {
                  Name = tcpClientElement.Name,
                  LocalEndPoint = new System.Net.IPEndPoint(IPAddress.Parse(tcpClientElement.LocalEndpoint.IP), tcpClientElement.LocalEndpoint.Port),
                  RemoteEndPoint = new System.Net.IPEndPoint(IPAddress.Parse(tcpClientElement.RemoteEndpoint.IP), tcpClientElement.RemoteEndpoint.Port),
                  MaxMessagesPerSecond = tcpClientElement.MaxMessagesPerSecond,
                  PoolSize = tcpClientElement.PoolSize,
                  TimeoutInMilliseconds = tcpClientElement.TimeoutPeriodInMilliseconds,
                  ConnectionsBacklog = tcpClientElement.ConnectionsBacklog,

                  IncomingDirectionSettings = new TcpTrafficSettings(tcpClientElement.IncomingTrafficSettings.NetworkBufferSize,
                      receiveTcpMessagesSettings),

                  OutgoingDirectionSettings = new TcpTrafficSettings(tcpClientElement.OutgoingTrafficSettings.NetworkBufferSize,
                    sendTcpMessagesSettings),

                  RoutingTable = _predicates,
                  UsePerformanceCounters = tcpClientElement.UsePerformanceCounters,
                  AppSettings = _adapterSettings
                });
              }
            } break;


          case "sql":
            {
              var sqlClientElement = clientElement.SqlClient;
              if (_sqlClientsSettings != null && sqlClientElement != null && sqlClientElement.IsEnabled)
              {
                string connectionString = null;
                if (ConfigurationManager.ConnectionStrings[sqlClientElement.ConnectionString] != null)
                  connectionString = ConfigurationManager.ConnectionStrings[sqlClientElement.ConnectionString].ConnectionString;
                else
                  throw new Exception("Cannot find connection string " + sqlClientElement.ConnectionString);

                _sqlClientsSettings.Add(new SqlClientSettings()
                {
                  MessageAuditLength = messageAuditLength,
                  Name = sqlClientElement.Name,
                  ConnectionString = connectionString,
                  PollingTimeout = sqlClientElement.PollingPeriodInMilliseconds,
                  RoutingTable = _predicates,
                  Guid = sqlClientElement.Guid,
                  MessageType = (MessageType)Enum.Parse(typeof(MessageType), sqlClientElement.MessageType),
                  UsePerformanceCounters = sqlClientElement.UsePerformanceCounters,
                  AppSettings = _adapterSettings
                });
              }
            } break;


          case "wcf":
            {
              var wcfClientElement = clientElement.WcfClient;
              if (_wcfClientsSettings != null && wcfClientElement != null && wcfClientElement.IsEnabled)
              {
                _wcfClientsSettings.Add(new WcfClientAdapterSettings()
                {
                  MessageAuditLength = messageAuditLength,
                  Name = wcfClientElement.Name,
                  EndpointConfigurationName = wcfClientElement.EndpointConfigurationName,
                  TimeoutPeriodInMilliseconds = wcfClientElement.TimeoutPeriodInMilliseconds,
                  RoutingTable = _predicates,
                  Guid = wcfClientElement.Guid,
                  MessageType = (MessageType)Enum.Parse(typeof(MessageType), wcfClientElement.MessageType),
                  UsePerformanceCounters = wcfClientElement.UsePerformanceCounters,
                  AppSettings = _adapterSettings
                });
              }
            } break;

          case "mem":
            {
              var memClientElement = clientElement.MemClient;
              if (_memClientsSettings != null && memClientElement != null && memClientElement.IsEnabled)
                _memClientsSettings.Add(new MemClientSettings
                {
                  MessageAuditLength = messageAuditLength,
                  Name = memClientElement.Name,
                  UsePerformanceCounters = memClientElement.UsePerformanceCounters,
                  AppSettings = _adapterSettings
                });
            } break;
          default:
            break;
        }



      }

    }

    private List<TcpMessageSettings> ReadTcpMessagesSettings(Corp.RouterService.Configuration.ServerConfigurationSection.TcpMessagesSettingsElementCollection tcpMessagesSettingsElementCollection)
    {
      List<TcpMessageSettings> settings = new List<TcpMessageSettings>();

      foreach (var msgSettings in tcpMessagesSettingsElementCollection)
      {

        var tcpMessageSettings = msgSettings as Corp.RouterService.Configuration.ServerConfigurationSection.TcpMessageSettingsElement;
        if (tcpMessageSettings != null && tcpMessageSettings.IsEnabled)
        {
          string[] lengthTypes = tcpMessageSettings.LengthType.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
          int lenghtTypesAggregated = 0;
          foreach (var item in lengthTypes)
          {
            lenghtTypesAggregated += (int)Enum.Parse(typeof(TcpMessageParts), item);
          }

          var tcpSettings = new TcpMessageSettings((MessageType)Enum.Parse(typeof(MessageType), tcpMessageSettings.MessageType),
          tcpMessageSettings.HeaderTemplate,
          tcpMessageSettings.HeaderPrefix,
          tcpMessageSettings.Header,
          tcpMessageSettings.HeaderSuffix,
          tcpMessageSettings.MessageSuffix,
          (TcpMessageHeaderLengthIndicatorFormat)Enum.Parse(typeof(TcpMessageHeaderLengthIndicatorFormat), tcpMessageSettings.LengthFormat),
          lenghtTypesAggregated,
          tcpMessageSettings.BodyFixedLength);
          if (!tcpSettings.IsValid())
            throw new Exception("TcpMessageSettings invalid");

          settings.Add(tcpSettings);
        }
      }
      return settings;
    }
  }
}
