
namespace Corp.RouterService.Message
{
    class TcpMessage
    {

        private TcpTrafficSettings _settings;

        private TcpMessageSettings _messageSettings;
        internal TcpMessageSettings MessageSettings
        {
            get { return _messageSettings; }
            set { _messageSettings = value; }
        }

        private TcpMessageBody _body;
        private TcpMessageHeader _header;
        private TcpMessageEndpoint _localEndpoint;
        private TcpMessageEndpoint _remoteEndpoint;


        private MessageType _type;

        public MessageType Type
        {
            get { return _type; }
            set
            {
                //_messageSettings = _settings.GetMessageTypeSettings(value);
                _type = value;
            }
        }

        internal TcpMessageBody Body
        {
            get { return _body; }
            set { _body = value; }
        }

        internal TcpMessageHeader Header
        {
            get { return _header; }
            set { _header = value; }
        }

        internal TcpMessageEndpoint LocalEndpoint
        {
            get { return _localEndpoint; }
            set { _localEndpoint = value; }
        }

        internal TcpMessageEndpoint RemoteEndpoint
        {
            get { return _remoteEndpoint; }
            set { _remoteEndpoint = value; }
        }


        private TcpMessage()
        {

        }
        public TcpMessage(MessageType type, TcpTrafficSettings settings)
        {
            _settings = settings;
            Type = type;            
        }


        internal bool IsValid(bool serialize = false)
        {
            return _type != MessageType.Unknown && _header.IsValid(serialize) && _body.IsValid(serialize);
        }
    }
}
