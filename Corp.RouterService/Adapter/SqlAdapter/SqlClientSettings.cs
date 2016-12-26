
namespace Corp.RouterService.Adapter.SqlAdapter
{
    public class SqlClientSettings : SqlAdapterSettings
    {
        public override SqlAdapterMode AdapterMode() { return SqlAdapterMode.Client; }

    }
}
