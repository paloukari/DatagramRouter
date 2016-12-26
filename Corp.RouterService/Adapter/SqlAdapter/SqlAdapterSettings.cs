using Corp.RouterService.Message;

namespace Corp.RouterService.Adapter.SqlAdapter
{
    public class SqlAdapterSettings : AdapterSettings
    {
        

        public string ConnectionString { get; set; }
        public int PollingTimeout { get; set; }
        public string Guid { get; set; }

        public virtual SqlAdapterMode AdapterMode() { return SqlAdapterMode.Server; }

        //ISO, WEB
        public MessageType MessageType { get; set; }

        public override string ToString()
        {
          return "SqlAdapter" + AdapterMode().ToString() +":"+ Name;
        }

        public int Timeout { get; set; }
    }

    public enum SqlAdapterMode
    {
        Server, 
        Client
    }
}
