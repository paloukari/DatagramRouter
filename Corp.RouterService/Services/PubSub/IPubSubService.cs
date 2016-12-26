using System.ServiceModel;

namespace Corp.RouterService.Services.PubSub
{
    [ServiceContract(Namespace = "Corp.RouterService.Services", 
        SessionMode = SessionMode.Required)]
    interface IPubSubService<T> where T : class
    {
        [OperationContract(IsOneWay = true)]
        void Publish(T data);
        [OperationContract(IsOneWay = false, IsInitiating = true)]
        void Subscribe();
        [OperationContract(IsOneWay = false, IsTerminating = true)]
        void Unsubscribe();
    }
}
