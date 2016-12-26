
namespace Corp.RouterService.Message
{
    public class MessageEndpoints
    {
        public MessageEndpoint LocalEndpoint { get; set; }
        public MessageEndpoint RemoteEndpoind { get; set; }

        public override string ToString()
        {
          return " From:" + (RemoteEndpoind != null ? RemoteEndpoind.ToString() : "NULL") +
            " To:" + (LocalEndpoint != null ? LocalEndpoint.ToString() : "NULL");
        }
    }
}
