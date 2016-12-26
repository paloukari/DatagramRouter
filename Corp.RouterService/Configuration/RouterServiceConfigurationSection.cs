using System;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Corp.RouterService.Configuration
{
  public class ServerConfigurationSection : System.Configuration.ConfigurationSection
  {

    public ServerConfigurationSection()
    {
    }
    
    [ConfigurationProperty("messageAuditLength", DefaultValue = "-1", IsRequired = false)]
    public int MessageAuditLength
    {
      get
      {
        int result = -1;
        Int32.TryParse(this["messageAuditLength"]!=null?this["messageAuditLength"].ToString():"-1", out result);
        return result;
      }
      set
      {
        this["messageAuditLength"] = value;
      }
    }




    [ConfigurationProperty("adapterSettings", IsRequired = true, IsKey = false, IsDefaultCollection = true)]
    public AdapterSettingsElementCollection AdapterSettingElements
    {
      get { return ((AdapterSettingsElementCollection)(base["adapterSettings"])); }
      set { base["adapterSettings"] = value; }
    }

    [ConfigurationProperty("tcpServers", IsRequired = true, IsKey = false, IsDefaultCollection = true)]
    public TcpServerElementCollection TcpServerElements
    {
      get { return ((TcpServerElementCollection)(base["tcpServers"])); }
      set { base["tcpServers"] = value; }
    }

    [ConfigurationProperty("sqlServers", IsRequired = true, IsKey = false, IsDefaultCollection = true)]
    public SqlServerElementCollection SqlServerElements
    {
      get { return ((SqlServerElementCollection)(base["sqlServers"])); }
      set { base["sqlServers"] = value; }
    }

    [ConfigurationProperty("wcfServers", IsRequired = true, IsKey = false, IsDefaultCollection = true)]
    public WcfServerElementCollection WcfServerElements
    {
      get { return ((WcfServerElementCollection)(base["wcfServers"])); }
      set { base["wcfServers"] = value; }
    }

    [ConfigurationProperty("clients", IsRequired = true, IsKey = false, IsDefaultCollection = true)]
    public ClientElementCollection ClientElements
    {
      get { return ((ClientElementCollection)(base["clients"])); }
      set { base["clients"] = value; }
    }


    [ConfigurationProperty("routingPredicates", IsRequired = true, IsKey = false, IsDefaultCollection = true)]
    public RoutingElementCollection RoutingElements
    {
      get { return ((RoutingElementCollection)(base["routingPredicates"])); }
      set { base["routingPredicates"] = value; }
    }



    [ConfigurationCollection(typeof(TcpServerElement), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
    public class TcpServerElementCollection : ConfigurationElementCollection
    {

      internal const string ItemPropertyName = "tcpServer";

      public TcpServerElementCollection()
      {

      }
      public override ConfigurationElementCollectionType CollectionType
      {
        get { return ConfigurationElementCollectionType.BasicMapAlternate; }
      }

      protected override string ElementName
      {
        get { return ItemPropertyName; }
      }

      protected override bool IsElementName(string elementName)
      {
        return (elementName == ItemPropertyName);
      }

      protected override object GetElementKey(ConfigurationElement element)
      {
        string key = "";
        if (((TcpServerElement)element).LocalEndpoint != null)
          key += ((TcpServerElement)element).LocalEndpoint.ToString();
        return key;
      }

      protected override ConfigurationElement CreateNewElement()
      {
        return new TcpServerElement();
      }

      public override bool IsReadOnly()
      {
        return false;
      }

    }

    [ConfigurationCollection(typeof(SqlServerElement), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
    public class SqlServerElementCollection : ConfigurationElementCollection
    {

      internal const string ItemPropertyName = "sqlServer";

      public SqlServerElementCollection()
      {

      }
      public override ConfigurationElementCollectionType CollectionType
      {
        get { return ConfigurationElementCollectionType.BasicMapAlternate; }
      }

      protected override string ElementName
      {
        get { return ItemPropertyName; }
      }

      protected override bool IsElementName(string elementName)
      {
        return (elementName == ItemPropertyName);
      }

      protected override object GetElementKey(ConfigurationElement element)
      {
        string key = "";
        if (((SqlServerElement)element).Name != null)
          key += ((SqlServerElement)element).Name.ToString();
        return key;
      }

      protected override ConfigurationElement CreateNewElement()
      {
        return new SqlServerElement();
      }

      public override bool IsReadOnly()
      {
        return false;
      }

    }
    [ConfigurationCollection(typeof(WcfServerElement), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
    public class WcfServerElementCollection : ConfigurationElementCollection
    {

      internal const string ItemPropertyName = "wcfServer";

      public WcfServerElementCollection()
      {

      }
      public override ConfigurationElementCollectionType CollectionType
      {
        get { return ConfigurationElementCollectionType.BasicMapAlternate; }
      }

      protected override string ElementName
      {
        get { return ItemPropertyName; }
      }

      protected override bool IsElementName(string elementName)
      {
        return (elementName == ItemPropertyName);
      }

      protected override object GetElementKey(ConfigurationElement element)
      {
        string key = "";
        if (((WcfServerElement)element).Name != null)
          key += ((WcfServerElement)element).Name.ToString();
        return key;
      }

      protected override ConfigurationElement CreateNewElement()
      {
        return new WcfServerElement();
      }

      public override bool IsReadOnly()
      {
        return false;
      }

    }

    [ConfigurationCollection(typeof(ClientElement), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
    public class ClientElementCollection : ConfigurationElementCollection
    {
      internal const string ItemPropertyName = "client";

      public ClientElementCollection()
      {

      }
      public override ConfigurationElementCollectionType CollectionType
      {
        get { return ConfigurationElementCollectionType.BasicMapAlternate; }
      }

      protected override string ElementName
      {
        get { return ItemPropertyName; }
      }

      protected override bool IsElementName(string elementName)
      {
        return (elementName == ItemPropertyName);
      }

      protected override object GetElementKey(ConfigurationElement element)
      {
        string key = "";
        ClientElement client = element as ClientElement;
        if (client != null)
        {
          if (client.Type == "tcp")
          {
            if (client.TcpClient != null)
              key += client.TcpClient.LocalEndpoint.ToString();
          }
          else if (client.Type == "sql")
          {
            if (client.SqlClient != null)
              key += client.SqlClient.Name;
          }
          else if (client.Type == "wcf")
          {
            if (client.MemClient != null)
              key += client.WcfClient.Name;
          }
          else if (client.Type == "mem")
          {
            if (client.MemClient != null)
              key += client.MemClient.Name;
          }

        }
        return key;
      }

      protected override ConfigurationElement CreateNewElement()
      {
        return new ClientElement();
      }

      public override bool IsReadOnly()
      {
        return false;
      }

    }


    public class SqlServerElement : AdapterElement
    {
      public SqlServerElement()
      {

      }

      [ConfigurationProperty("connectionString", IsRequired = true, IsKey = false)]
      public string ConnectionString
      {
        get { return base["connectionString"] as string; }
        set { base["connectionString"] = value; }
      }


      [ConfigurationProperty("messageType", DefaultValue = "Iso8583", IsRequired = true)]
      [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
      public String MessageType
      {
        get
        {
          return (String)this["messageType"];
        }
        set
        {
          this["messageType"] = value;
        }
      }

      [ConfigurationProperty("pollingPeriodInMilliseconds", IsRequired = true, IsKey = false)]
      public int PollingPeriodInMilliseconds
      {
        get
        {
          return (int)base["pollingPeriodInMilliseconds"];
        }
        set { base["pollingPeriodInMilliseconds"] = value; }
      }

      [ConfigurationProperty("guid", IsRequired = true, IsKey = false)]
      public string Guid
      {
        get { return base["guid"] as string; }
        set { base["guid"] = value; }
      }



    }

    public class SqlClientElement : SqlServerElement { }

    public class TcpServerElement : AdapterElement
    {
      public TcpServerElement()
      {

      }
      [ConfigurationProperty("localEndpoint", IsRequired = true, IsKey = false)]
      public ServerEndpointElement LocalEndpoint
      {
        get { return ((ServerEndpointElement)(base["localEndpoint"])); }
        set { base["localEndpoint"] = value; }
      }


      [ConfigurationProperty("incomingTrafficSettings", IsRequired = true, IsKey = false)]
      public TcpTrafficSettingsElement IncomingTrafficSettings
      {
        get { return ((TcpTrafficSettingsElement)(base["incomingTrafficSettings"])); }
        set { base["incomingTrafficSettings"] = value; }
      }

      [ConfigurationProperty("outgoingTrafficSettings", IsRequired = true, IsKey = false)]
      public TcpTrafficSettingsElement OutgoingTrafficSettings
      {
        get { return ((TcpTrafficSettingsElement)(base["outgoingTrafficSettings"])); }
        set { base["outgoingTrafficSettings"] = value; }
      }




      [ConfigurationProperty("poolSize", IsRequired = true)]
      //[IntegerValidator(MinValue = 1, MaxValue = 1000)]
      public int PoolSize
      {
        get
        {
          return (int)this["poolSize"];
        }
        set
        {
          this["poolSize"] = value;
        }
      }

      [ConfigurationProperty("connectionsBacklog", IsRequired = true)]
      //[IntegerValidator(MinValue = 1, MaxValue = 1000)]
      public int ConnectionsBacklog
      {
        get
        {
          return (int)this["connectionsBacklog"];
        }
        set
        {
          this["connectionsBacklog"] = value;
        }
      }
    }

    public class WcfServerElement : AdapterElement
    {
      public WcfServerElement()
      {

      }

      
      [ConfigurationProperty("messageType", DefaultValue = "Iso8583", IsRequired = true)]
      [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
      public String MessageType
      {
        get
        {
          return (String)this["messageType"];
        }
        set
        {
          this["messageType"] = value;
        }
      }

      [ConfigurationProperty("serverURI", IsRequired = true, IsKey = false)]
      public string ServerURI
      {
        get
        {
          return base["serverURI"].ToString();
        }
        set { base["serverURI"] = value; }
      }

      [ConfigurationProperty("timeoutPeriodInMilliseconds", IsRequired = true, IsKey = false)]
      public int TimeoutPeriodInMilliseconds
      {
        get
        {
          return (int)base["timeoutPeriodInMilliseconds"];
        }
        set { base["timeoutPeriodInMilliseconds"] = value; }
      }

      [ConfigurationProperty("guid", IsRequired = true, IsKey = false)]
      public string Guid
      {
        get { return base["guid"] as string; }
        set { base["guid"] = value; }
      }



    }

    public class WcfClientElement : AdapterElement
    {
      public WcfClientElement()
      {

      }      
      [ConfigurationProperty("messageType", DefaultValue = "Iso8583", IsRequired = true)]
      [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
      public String MessageType
      {
        get
        {
          return (String)this["messageType"];
        }
        set
        {
          this["messageType"] = value;
        }
      }

      [ConfigurationProperty("endpointConfigurationName", IsRequired = true, IsKey = false)]
      public string EndpointConfigurationName
      {
        get
        {
          return base["endpointConfigurationName"].ToString();
        }
        set { base["endpointConfigurationName"] = value; }
      }


      [ConfigurationProperty("timeoutPeriodInMilliseconds", IsRequired = true, IsKey = false)]
      public int TimeoutPeriodInMilliseconds
      {
        get
        {
          return (int)base["timeoutPeriodInMilliseconds"];
        }
        set { base["timeoutPeriodInMilliseconds"] = value; }
      }

      [ConfigurationProperty("guid", IsRequired = true, IsKey = false)]
      public string Guid
      {
        get { return base["guid"] as string; }
        set { base["guid"] = value; }
      }
    }

    public class ClientElement : SwitchedConfigurationElement
    {
      public ClientElement()
      {

      }
      [ConfigurationProperty("type", IsRequired = true)]
      //[StringValidator(MaxLength = 3, MinLength = 3)]
      public string Type
      {
        get
        {
          return (string)this["type"];
        }
        set
        {
          this["type"] = value;
        }
      }


      [ConfigurationProperty("tcpClient", IsRequired = false, IsKey = false, IsDefaultCollection = true)]
      public TcpClientElement TcpClient
      {
        get { return ((TcpClientElement)(base["tcpClient"])); }
        set { base["tcpClient"] = value; }
      }

      [ConfigurationProperty("sqlClient", IsRequired = false, IsKey = false, IsDefaultCollection = true)]
      public SqlClientElement SqlClient
      {
        get { return ((SqlClientElement)(base["sqlClient"])); }
        set { base["sqlClient"] = value; }
      }

      [ConfigurationProperty("wcfClient", IsRequired = false, IsKey = false, IsDefaultCollection = true)]
      public WcfClientElement WcfClient
      {
        get { return ((WcfClientElement)(base["wcfClient"])); }
        set { base["wcfClient"] = value; }
      }

      [ConfigurationProperty("memClient", IsRequired = false, IsKey = false, IsDefaultCollection = true)]
      public MemClientElement MemClient
      {
        get { return ((MemClientElement)(base["memClient"])); }
        set { base["memClient"] = value; }
      }


    }

    public class MemClientElement : AdapterElement
    {
      public MemClientElement()
      {

      }      
    }
    //for the time being, every client is a tcp client
    public class TcpClientElement : TcpServerElement
    {
      public TcpClientElement()
      {

      }
      [ConfigurationProperty("remoteEndpoint", IsRequired = true, IsKey = false)]
      public ServerEndpointElement RemoteEndpoint
      {
        get { return ((ServerEndpointElement)(base["remoteEndpoint"])); }
        set { base["remoteEndpoint"] = value; }
      }  

      [ConfigurationProperty("timeoutPeriodInMilliseconds", IsRequired = true, IsKey = false)]
      public int TimeoutPeriodInMilliseconds
      {
        get
        {
          return (int)base["timeoutPeriodInMilliseconds"];
        }
        set { base["timeoutPeriodInMilliseconds"] = value; }
      }
      
    }

    public class TcpTrafficSettingsElement : SwitchedConfigurationElement
    {
      public TcpTrafficSettingsElement()
      {

      }

      [ConfigurationProperty("networkBufferSize", IsRequired = true)]
      public int NetworkBufferSize
      {
        get
        {
          return (int)this["networkBufferSize"];
        }
        set
        {
          this["networkBufferSize"] = value;
        }
      }

      [ConfigurationProperty("tcpMessagesSettings", IsRequired = true, IsKey = false, IsDefaultCollection = true)]
      public TcpMessagesSettingsElementCollection TcpMessagesSetttingsElements
      {
        get { return ((TcpMessagesSettingsElementCollection)(base["tcpMessagesSettings"])); }
        set { base["tcpMessagesSettings"] = value; }
      }
    }

    [ConfigurationCollection(typeof(TcpMessageSettingsElement), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
    public class TcpMessagesSettingsElementCollection : ConfigurationElementCollection
    {
      internal const string ItemPropertyName = "tcpMessageSettings";

      public TcpMessagesSettingsElementCollection()
      {

      }
      public override ConfigurationElementCollectionType CollectionType
      {
        get { return ConfigurationElementCollectionType.BasicMapAlternate; }
      }

      protected override string ElementName
      {
        get { return ItemPropertyName; }
      }

      protected override bool IsElementName(string elementName)
      {
        return (elementName == ItemPropertyName);
      }

      protected override object GetElementKey(ConfigurationElement element)
      {
        string key = "";
        if (((TcpMessageSettingsElement)element).MessageType != null)
          key += ((TcpMessageSettingsElement)element).MessageType;
        if (((TcpMessageSettingsElement)element).HeaderTemplate != null)
          key += ((TcpMessageSettingsElement)element).HeaderTemplate;
        if (((TcpMessageSettingsElement)element).HeaderPrefix != null)
          key += ((TcpMessageSettingsElement)element).HeaderPrefix;
        if (((TcpMessageSettingsElement)element).HeaderSuffix != null)
          key += ((TcpMessageSettingsElement)element).HeaderSuffix;
        if (((TcpMessageSettingsElement)element).Header != null)
          key += ((TcpMessageSettingsElement)element).Header;
        if (((TcpMessageSettingsElement)element).LengthFormat != null)
          key += ((TcpMessageSettingsElement)element).LengthFormat;
        if (((TcpMessageSettingsElement)element).LengthType != null)
          key += ((TcpMessageSettingsElement)element).LengthType;

        if (((TcpMessageSettingsElement)element).MessageSuffix != null)
          key += ((TcpMessageSettingsElement)element).MessageSuffix;
        

        return key;
      }

      protected override ConfigurationElement CreateNewElement()
      {
        return new TcpMessageSettingsElement();
      }

      public override bool IsReadOnly()
      {
        return false;
      }

    }

    public class TcpMessageSettingsElement : SwitchedConfigurationElement
    {
      [ConfigurationProperty("messageType", DefaultValue = "Iso8583", IsRequired = true)]
      [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
      public String MessageType
      {
        get
        {
          return (String)this["messageType"];
        }
        set
        {
          this["messageType"] = value;
        }
      }
      //example
      //"PPLL**HHHHHHSS"
      //2 bytes prefix, 2 bytes length, 2 bytes ignored, 6 bytes header, 2 bytes suffix
      //the * are ingnored for incoming messages and copied from message.VariableHeader for outgoing
      [ConfigurationProperty("headerTemplate", DefaultValue = "PL*HS", IsRequired = true)]
      [StringValidator(InvalidCharacters = "qwertyuiopasdfghjklzxcvbnm QWERTYUIOADFGJKZXVBNM?~!@#$%^&()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
      public string HeaderTemplate
      {
        get
        {
          return this["headerTemplate"].ToString();
        }
        set
        {
          this["headerTemplate"] = value;
        }     
      }

      [ConfigurationProperty("headerPrefix", IsRequired = true)]
      public string HeaderPrefix
      {
        get
        {
          var bytesInString = this["headerPrefix"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
          if (bytesInString.Count() > 0)
          {
            var bytes = bytesInString.Select(s => Convert.ToByte(s.Trim(), 16));
            if (bytes.Count() > 0)
              return Encoding.GetEncoding(1253).GetString(bytes.ToArray());
          }
          return string.Empty;
        }
        set
        {
          this["headerPrefix"] = value;
        }
      }

      [ConfigurationProperty("header", IsRequired = true)]
      public string Header
      {
        get
        {
          return this["header"].ToString();
        }
        set
        {
          this["header"] = value;
        }
      }

      [ConfigurationProperty("headerSuffix", IsRequired = true)]
      public string HeaderSuffix
      {
        get
        {

          var bytesInString = this["headerSuffix"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
          if (bytesInString.Count() > 0)
          {
            var bytes = bytesInString.Select(s => Convert.ToByte(s.Trim(), 16));
            if (bytes.Count() > 0)
              return Encoding.GetEncoding(1253).GetString(bytes.ToArray());
          }
          return string.Empty;
        }
        set
        {

          this["headerSuffix"] = value;
        }
      }

      [ConfigurationProperty("messageSuffix", IsRequired = true)]
      public string MessageSuffix
      {
        get
        {

          var bytesInString = this["messageSuffix"].ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
          if (bytesInString.Count() > 0)
          {
            var bytes = bytesInString.Select(s => Convert.ToByte(s.Trim(), 16));
            if (bytes.Count() > 0)
              return Encoding.GetEncoding(1253).GetString(bytes.ToArray());
          }
          return string.Empty;
        }
        set
        {

          this["messageSuffix"] = value;
        }
      }

      [ConfigurationProperty("lengthFormat", IsRequired = true)]
      public string LengthFormat
      {
        get
        {
          return this["lengthFormat"].ToString();
        }
        set
        {
          this["lengthFormat"] = value;
        }
      }

      [ConfigurationProperty("lengthType", IsRequired = true)]
      public string LengthType
      {
        get
        {
          return this["lengthType"].ToString();
        }
        set
        {
          this["lengthType"] = value;
        }
      }

      [ConfigurationProperty("bodyFixedLength", IsRequired = false)]
      public int BodyFixedLength
      {
          get
          {
              int result = -1;
              if (this["bodyFixedLength"] != null)
                  Int32.TryParse(this["bodyFixedLength"].ToString(), out result);
              return result;
          }
          set
          {
              this["bodyFixedLength"] = value;
          }
      }
    }

    public class ServerEndpointElement : SwitchedConfigurationElement
    {

      public ServerEndpointElement()
      {

      }

      public override string ToString()
      {
        return this.IP + ":" + this.Port;
      }
      [ConfigurationProperty("IP", IsRequired = true)]
      //[StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
      public String IP
      {
        get
        {
          return (String)this["IP"];
        }
        set
        {
          this["IP"] = value;
        }
      }

      [ConfigurationProperty("Port", IsRequired = true)]
      //[IntegerValidator(MinValue = 1, MaxValue = 67000)]
      public int Port
      {
        get
        {
          return (int)this["Port"];
        }
        set
        {
          this["Port"] = value;
        }
      }
    }

    [ConfigurationCollection(typeof(AdapterSettingsElement), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
    public class AdapterSettingsElementCollection : ConfigurationElementCollection
    {
      internal const string ItemPropertyName = "adapterSetting";

      public AdapterSettingsElementCollection()
      {

      }
      public override ConfigurationElementCollectionType CollectionType
      {
        get { return ConfigurationElementCollectionType.BasicMapAlternate; }
      }

      protected override string ElementName
      {
        get { return ItemPropertyName; }
      }

      protected override bool IsElementName(string elementName)
      {
        return (elementName == ItemPropertyName);
      }

      protected override object GetElementKey(ConfigurationElement element)
      {
        string key = "";
        if (((AdapterSettingsElement)element).SettingsGroup!= null)
          key += ((AdapterSettingsElement)element).SettingsGroup;
        if (((AdapterSettingsElement)element).SettingsKey != null)
          key += ((AdapterSettingsElement)element).SettingsKey;
        if (((AdapterSettingsElement)element).SettingsValue!= null)
          key += ((AdapterSettingsElement)element).SettingsValue;        
        return key;
      }

      protected override ConfigurationElement CreateNewElement()
      {
        return new AdapterSettingsElement();
      }

      public override bool IsReadOnly()
      {
        return false;
      }

    }

    [ConfigurationCollection(typeof(RoutingElement), CollectionType = ConfigurationElementCollectionType.BasicMapAlternate)]
    public class RoutingElementCollection : ConfigurationElementCollection
    {
      internal const string ItemPropertyName = "routingPredicate";

      public RoutingElementCollection()
      {

      }
      public override ConfigurationElementCollectionType CollectionType
      {
        get { return ConfigurationElementCollectionType.BasicMapAlternate; }
      }

      protected override string ElementName
      {
        get { return ItemPropertyName; }
      }

      protected override bool IsElementName(string elementName)
      {
        return (elementName == ItemPropertyName);
      }

      protected override object GetElementKey(ConfigurationElement element)
      {
        string key = "";
        if (((RoutingElement)element).MessageType != null)
          key += ((RoutingElement)element).MessageType;
        if (((RoutingElement)element).OriginLocalAddress != null)
          key += ((RoutingElement)element).OriginLocalAddress;
        if (((RoutingElement)element).OriginRemoteAddress != null)
          key += ((RoutingElement)element).OriginRemoteAddress;
        if (((RoutingElement)element).DestinationAddress != null)
          key += ((RoutingElement)element).DestinationAddress;
        return key;
      }

      protected override ConfigurationElement CreateNewElement()
      {
        return new RoutingElement();
      }

      public override bool IsReadOnly()
      {
        return false;
      }

    }

    public class AdapterSettingsElement : SwitchedConfigurationElement
    {
      public AdapterSettingsElement()
      {

      }

      [ConfigurationProperty("group", IsRequired = true)]      
      public String SettingsGroup
      {
        get
        {
          return (String)this["group"];
        }
        set
        {
          this["group"] = value;
        }
      }

      [ConfigurationProperty("key", IsRequired = true)]      
      public String SettingsKey
      {
        get
        {
          return (String)this["key"];
        }
        set
        {
          this["key"] = value;
        }
      }

      [ConfigurationProperty("value", IsRequired = true)]      
      public String SettingsValue
      {
        get
        {
          return (String)this["value"];
        }
        set
        {
          this["value"] = value;
        }
      }
    }

    public class RoutingElement : SwitchedConfigurationElement
    {
      public RoutingElement()
      {

      }
      [ConfigurationProperty("messageType", DefaultValue = "Iso8583", IsRequired = true)]
      [StringValidator(InvalidCharacters = "~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
      public String MessageType
      {
        get
        {
          return (String)this["messageType"];
        }
        set
        {
          this["messageType"] = value;
        }
      }

      [ConfigurationProperty("destinationAddress", IsRequired = true)]
      [StringValidator(MaxLength = 100)]
      public string DestinationAddress
      {
        get
        { return this["destinationAddress"].ToString(); }
        set
        { this["destinationAddress"] = value; }
      }

      [ConfigurationProperty("originLocalAddress", IsRequired = false)]
      [StringValidator(MaxLength = 100)]
      public string OriginLocalAddress
      {
        get
        { return this["originLocalAddress"].ToString(); }
        set
        { this["originLocalAddress"] = value; }
      }

      [ConfigurationProperty("originRemoteAddress", IsRequired = false)]
      [StringValidator(MaxLength = 100)]
      public string OriginRemoteAddress
      {
        get
        { return this["originRemoteAddress"].ToString(); }
        set
        { this["originRemoteAddress"] = value; }
      }

      [ConfigurationProperty("includeDiagnostics", IsRequired = false)]
      public bool IncludeDiagnostics
      {
        get
        {
          bool result = false;
          if (this["includeDiagnostics"] != null)
            bool.TryParse(this["includeDiagnostics"].ToString(), out result);
          return result;
        }
        set
        { this["includeDiagnostics"] = value; }
      }


    }
  }


  public class SwitchedConfigurationElement : ConfigurationElement
  {
    [ConfigurationProperty("isEnabled", IsRequired = false)]
    public bool IsEnabled
    {
      get
      {
        bool result = true;
        if (this["isEnabled"] != null)
          bool.TryParse(this["isEnabled"].ToString(), out result);
        return result;
      }
      set
      {
        this["isEnabled"] = value;
      }
    }
  }

  public class AdapterElement : SwitchedConfigurationElement
  {

    [ConfigurationProperty("name", IsRequired = true)]
    public string Name
    {
      get
      {
        return base["name"].ToString();
      }
      set
      {
        base["name"] = value;
      }
    }

    [ConfigurationProperty("usePerformanceCounters", IsRequired = false)]
    public bool UsePerformanceCounters
    {
      get
      {
        bool result = true;
        if (this["usePerformanceCounters"] != null)
          bool.TryParse(this["usePerformanceCounters"].ToString(), out result);
        return result;
      }
      set
      {
        this["usePerformanceCounters"] = value;
      }
    }

    [ConfigurationProperty("maxMessagesPerSecond", IsRequired = false)]
    public int MaxMessagesPerSecond
    {
      get
      {
        int result = 0;
        if (this["maxMessagesPerSecond"] != null)
          Int32.TryParse(this["maxMessagesPerSecond"].ToString(), out result);
        return result;
      }
      set
      {
        this["maxMessagesPerSecond"] = value;
      }
    }

    
  }

}
