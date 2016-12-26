
namespace Corp.RouterService.Adapter.SqlAdapter
{
    public class SqlServerSettings : SqlAdapterSettings
    {
        public override SqlAdapterMode AdapterMode() { return SqlAdapterMode.Server; }
    }
}
