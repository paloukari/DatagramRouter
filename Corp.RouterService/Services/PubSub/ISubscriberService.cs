using System.ServiceModel;

namespace Corp.RouterService.Services.PubSub
{
    internal interface ISubscriberService<T> where T : class
    {
        [OperationContract(IsOneWay = true)]
        void OnPublish(T data);
    }
}
