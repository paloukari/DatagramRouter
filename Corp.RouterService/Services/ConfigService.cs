using System.ServiceModel;
using Corp.RouterService.Services.PubSub;

namespace Corp.RouterService.Services
{

    [ServiceContract(CallbackContract = typeof(ISubscriberService<Config>))]
    internal class ConfigService : PubSubService<Config>
    {

    }
}
