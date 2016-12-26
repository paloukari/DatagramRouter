using System;
using System.Text;

namespace Corp.RouterService.Message
{
    class TcpMessageBodyDataHandler
    {
        private TcpTrafficSettings _settings;        
        
        private TcpMessageBodyDataHandler()
        {

        }
        internal TcpMessageBodyDataHandler(TcpTrafficSettings settings)
        {            
            _settings = settings;            
        }
     

        internal void PartiallyDeserializeBodyData(TcpMessageBody tcpServerMessageBody, TcpMessageBuffer buffer)
        {
            if (tcpServerMessageBody.MessageSettings.IsTokenBasedMessage)
            {
                tcpServerMessageBody.HandledData += buffer.Remove(ref tcpServerMessageBody.BodyData,
                            tcpServerMessageBody.HandledData, tcpServerMessageBody.MessageSettings.MessageSuffix);
            }
            else
            {
                int messageBodyBytesCount = Math.Min(buffer.Count, tcpServerMessageBody.UnhandledData);

                if (messageBodyBytesCount > 0)
                    tcpServerMessageBody.HandledData += buffer.Remove(tcpServerMessageBody.BodyData,
                        tcpServerMessageBody.HandledData, messageBodyBytesCount);
            }        
        }

        internal void PartiallySerializeBodyData(TcpMessageBody tcpServerMessageBody, TcpMessageBuffer buffer)
        {
            int messageBodyBytesCount = Math.Min(buffer.FreeCount, tcpServerMessageBody.UnhandledData);

            if (messageBodyBytesCount > 0)
                tcpServerMessageBody.HandledData += buffer.Add(tcpServerMessageBody.BodyData,
                    tcpServerMessageBody.HandledData, messageBodyBytesCount);
        }
    }
}
