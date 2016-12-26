using System.Linq;
using System;
using Corp.RouterService.Common;

namespace Corp.RouterService.Message
{
    class TcpMessageHeader
    {
        private TcpTrafficSettings _settings;

        private TcpMessageSettings _messageSettings;
        internal TcpMessageSettings MessageSettings
        {
            get { return _messageSettings; }
        }

        internal int BodyLength { get; set; }

        private int _handledData;
        private int _headerLength;
        private byte[] _headerData;
        MessageType _type;

        public MessageType Type
        {
            get { return _type; }
            set
            {
                if (value == MessageType.Unknown)
                {
                    if (_settings.TcpMessagesSettings.Count == 1)
                    {
                        //small optimization here
                        _messageSettings = _settings.TcpMessagesSettings[0];
                        _type = _messageSettings.MessageType;

                        _headerLength = _messageSettings.HeaderLength;
                        _headerData = new byte[_headerLength];
                        _handledData = 0;
                    }
                    else
                    {
                        _type = value;
                        _messageSettings = null;

                        _headerLength = 0;
                        _headerData = new byte[_headerLength];
                        _handledData = 0;
                    }
                }
                else
                {
                    _type = value;
                    _messageSettings = _settings.GetMessageTypeSettings(value);
                }
            }
        }


        internal byte[] HeaderData
        {
            get { return _headerData; }
        }

        internal int UnhandledData
        {
            get
            {
                if (_type == MessageType.Unknown && _headerLength == _handledData)
                {
                    var messageSettings = _settings.TcpMessagesSettings.Where(c => c.HeaderLength == _handledData);
                    if (messageSettings.Count() > 0)
                    {
                        //determine if we can read this header
                        messageSettings = _settings.TcpMessagesSettings.Where(c => c.ValidateHeader(_headerData));

                        if (messageSettings.Count() > 1)
                          throw new Exception("More than one message settings validated a header" + System.Text.Encoding.GetEncoding(1253).GetString(_headerData));

                        if (messageSettings.Count() == 1)
                        {
                            _messageSettings=messageSettings.First();
                            _type = _messageSettings.MessageType;
                        }                        
                    }

                    if(_type == MessageType.Unknown)
                    {
                        //find the next larger header
                        messageSettings = _settings.TcpMessagesSettings.Where(c => c.HeaderLength > _handledData).OrderBy(x => x.HeaderLength);

                        if (messageSettings.Count() == 0)
                        {
                            throw new Exception("message header cound not be read according to the settings");
                        }
                        _headerLength = messageSettings.First().HeaderLength;
                        byte[] tmp = new byte[_headerLength];
                        if (_headerData.Length > 0)
                            Buffer.BlockCopy(_headerData, 0, tmp, 0, _headerData.Length);
                        _headerData = tmp;
                        

                    }                    
                }
                return _headerLength - _handledData;
            }
        }

        internal int HandledData
        {
            get { return _handledData; }
            set { _handledData = value; }
        }

        internal TcpMessageHeader(MessageType type, TcpTrafficSettings settings)
        {
            _settings = settings;
            Type = type;            
        }

        internal TcpMessageHeader(MessageType type, byte[] headerData, int bodyLenght, TcpTrafficSettings settings)
        {
            _settings = settings;
            Type = type;

            _headerLength = headerData.Count();
            _headerData = headerData;
            _handledData = 0;

            BodyLength = bodyLenght;
        }

        internal bool IsValid(bool serialize = false)
        {
            return _type != MessageType.Unknown && UnhandledData == 0;
        }
    }
}
