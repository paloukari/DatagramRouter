using System.ServiceModel;
using Corp.RouterService.Services.PubSub;

namespace Corp.RouterService.Services
{
    //no templates in attributes
    [ServiceContract(CallbackContract = typeof(ISubscriberService<LiveData>))]
    internal class LiveDataService : PubSubService<LiveData>
    {

    }
}
