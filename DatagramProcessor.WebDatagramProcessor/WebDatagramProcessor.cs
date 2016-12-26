
namespace Corp.RouterService.Message.DatagramProcessor
{
    public class WebDatagramProcessor : DatagramProcessor
    {
        public override void PreprocessMessage(ref Message inMessage)
        {
            var webdata = new WebData(inMessage.Payload);
            inMessage.ProcessorData = webdata;
        }

        public override Message ProcessMessage(Message inMessage)
        {
            Message response = new Message(inMessage.Type, 
              inMessage.Payload, 
              inMessage.Info.OutgoingEndpoints, 
              inMessage.HeaderText,
              inMessage.HeaderCopyData,
              inMessage.HeaderSuffix);
            PreprocessMessage(ref response);
            return response;
        }
    }
}
