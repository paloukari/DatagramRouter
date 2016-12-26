using System.ServiceModel;

namespace Corp.RouterService.Services.PubSub
{
    internal abstract class PubSubService<T> : IPubSubService<T> where T : class
    {
        T _data = null;
        ISubscriberService<T> _callback = null;


        internal static event DataEventHandler dataChangeEvent;
        internal delegate void DataEventHandler(object sender, T e);


        public void Publish(T data)
        {
            _data = data;

            PublishHandler(this, data);            
        }

        public void Subscribe()
        {
            _callback = OperationContext.Current.GetCallbackChannel<ISubscriberService<T>>();

            if (dataChangeEvent == null)
                dataChangeEvent = new DataEventHandler(PublishHandler);
            dataChangeEvent += PublishHandler;           
        }

        public void Unsubscribe()
        {
            dataChangeEvent += PublishHandler;
        }

        private void PublishHandler(object sender, T e)
        {
            _callback.OnPublish(_data);
        }
    }
}
