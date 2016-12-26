using Corp.RouterService.Message;

namespace Corp.RouterService.Adapter.SqlAdapter
{
    public interface IQueues
    {
        void InputQueueEnqueue(Message.Message message);
        Message.Message InputQueueDequeue();
        void OutputQueueEnqueue(Message.Message message);
        Message.Message OutputQueueDequeue();        
    }
}
