using System;
using System.Linq;
using System.Text;

namespace Corp.RouterService.Message
{
    class TcpMessageBody
    {
        MessageType _type;

        public MessageType Type
        {
            get { return _type; }
            set { _type = value; }
        }


        private TcpMessageSettings _messageSettings;
      internal TcpMessageSettings MessageSettings
      {
        get { return _messageSettings; }
        set { _messageSettings = value; }
      }



        internal byte[] BodyData;
        private int _bodyLength;
        private int _handledData;


        internal int HandledData
        {
            get { return _handledData; }
            set { _handledData = value; }
        }
        internal int UnhandledData
        {
            get { return _bodyLength - _handledData; }
        }

        private TcpMessageBody()
        {

        }

        public TcpMessageBody(MessageType type, TcpMessageSettings messagesSettings)
        {
            _type = type;
            _messageSettings = messagesSettings;
        }

        internal TcpMessageBody(MessageType type, int bodyLength, TcpMessageSettings messagesSettings)
        {
            _type = type;
            _messageSettings = messagesSettings;
            _bodyLength = bodyLength;
            BodyData = new byte[_bodyLength];
            _handledData = 0;
        }

        internal TcpMessageBody(MessageType type, byte[] body, TcpMessageSettings messagesSettings)
        {
            _type = type;
            _messageSettings = messagesSettings;
            _bodyLength = body.Count();
            BodyData = body;
            _handledData = 0;
        }

        internal bool IsValid(bool serialize = false)
        {
          if(serialize)
            return UnhandledData == 0;

          if (_messageSettings.IsTokenBasedMessage)
          {
              return IsValid(_messageSettings.MessageSuffix);         
          }
         
           return UnhandledData == 0;
                    

          //switch (_messageSettings.MessageType)
          //{
          //  case MessageType.Iso8583:
          //  case MessageType.Web:
          //    {
          //      return UnhandledData == 0;
          //    }
          //  case MessageType.IsoInternal:
          //    return IsValid(_messageSettings.HeaderSuffix);
          //  case MessageType.Unknown:
          //  default:
          //    throw new Exception("Unknown Message Type");
              
          //}
          
        }

        internal bool IsValid(byte[] suffix)
        {
          if (BodyData.Length < suffix.Length)
            return false;

          for(int i=0;i<suffix.Length;i++)
          {
            if (BodyData[BodyData.Length - suffix.Length + i] != suffix[i])
              return false;
          }
          return true;
        }
        
    }
}
