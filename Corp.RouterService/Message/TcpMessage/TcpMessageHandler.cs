using System;
using System.Collections.Generic;
using System.Threading;

namespace Corp.RouterService.Message
{
    class TcpMessageHandler
    {

        bool _hasError = false;
        private TcpMessageBodyDataHandler _bodyHandler;
        private TcpMessageHeaderDataHandler _headerHandler;
        private TcpMessageBuffer _buffer;
        private TcpMessage _underConstructionMessage;

        private TcpMessage _underDeconstructionMessage;
        private Stack<TcpMessage> _underDeconstructionMessages;
        Semaphore _decontructionSemaphore;
        private TcpMessage _armedTcpMessage;

        internal TcpMessage ArmedTcpMessage
        {
            get { return _armedTcpMessage; }
        }
        private int _bufferSize;
        private int _bufferOffset;        
        private int _tokenId;
        private TcpTrafficSettings _messagesSettings;
        private TcpMessageHandler()
        {

        }

        internal TcpMessageHandler(TcpTrafficSettings messagesSettings,
            int bufferOffset,
            int tokenId)
        {
            _messagesSettings = messagesSettings;
            _bufferOffset = bufferOffset;
            _tokenId = tokenId;
            _underDeconstructionMessages = new Stack<TcpMessage>();
            _decontructionSemaphore = new Semaphore(1, 1);
            _bufferSize = messagesSettings.NetworkBufferSize;            
            _buffer = new TcpMessageBuffer(messagesSettings.CircularBufferSize);            
            //we will know the message type when the header is valid
            _bodyHandler = new TcpMessageBodyDataHandler(messagesSettings);
            _headerHandler = new TcpMessageHeaderDataHandler(messagesSettings);
        }

        internal void AddData(Byte[] buffer, int count, bool log = false)
        {
            _buffer.Add(buffer, _bufferOffset, count, log);
        }

        internal int RemoveData(Byte[] buffer, bool log=false)
        {
          return _buffer.Remove(buffer, _bufferOffset, _bufferSize, log);
        }

        internal bool HasData { get { return _buffer.Count > 0; } }

        internal bool IsValid()
        {
            return !_hasError;
        }

        internal bool TryConstructTcpMessage()
        {
            try
            {
                if (_underConstructionMessage == null)
                {
                    _underConstructionMessage = new TcpMessage( MessageType.Unknown, _messagesSettings);
                    _underConstructionMessage.Header = new TcpMessageHeader(MessageType.Unknown, _messagesSettings);
                }

                if (!_underConstructionMessage.Header.IsValid())
                {
                    _headerHandler.PartiallyDeserializeHeaderData(_underConstructionMessage.Header, _buffer);
                }

                if (_underConstructionMessage.Header.IsValid())
                {                    
                    if (_underConstructionMessage.Body == null)
                    {
                        //we got the message type!!
                        _underConstructionMessage.Type = _underConstructionMessage.Header.Type;
                        _underConstructionMessage.MessageSettings = _underConstructionMessage.Header.MessageSettings;
                        _underConstructionMessage.Body = new TcpMessageBody(_underConstructionMessage.Header.Type, _underConstructionMessage.Header.BodyLength, _underConstructionMessage.Header.MessageSettings);
                    }
                    if (!_underConstructionMessage.Body.IsValid())
                    {
                        _bodyHandler.PartiallyDeserializeBodyData(_underConstructionMessage.Body, _buffer);
                    }
                }

                return _underConstructionMessage.IsValid();
            }
            catch (Exception)
            {
                _hasError = true;
                return true;
            }
        }

        internal bool TryDeconstructTcpMessage()
        {
            try
            {
                _decontructionSemaphore.WaitOne();
                {
                    //only for the first message
                  if (_underDeconstructionMessage == null || (_underDeconstructionMessage.IsValid(true) && !HasData))
                        PrepareForSending();

                    //no message to send
                    if (_underDeconstructionMessage == null)
                        return false;

                    if (!_underDeconstructionMessage.IsValid(true))
                    {
                        if (_underDeconstructionMessage == null)
                        {
                            throw new Exception("_underDeconstructionMessage is null");
                        }

                        if (!_underDeconstructionMessage.Header.IsValid(true))
                        {
                            _headerHandler.PartiallySerializeHeaderData(_underDeconstructionMessage.Header, _buffer);
                        }

                        if (_underDeconstructionMessage.Header.IsValid(true))
                        {
                          if (!_underDeconstructionMessage.Body.IsValid(true))
                            {
                                _bodyHandler.PartiallySerializeBodyData(_underDeconstructionMessage.Body, _buffer);
                            }
                        }
                    }

                    return _underDeconstructionMessage.IsValid(true) || _buffer.FreeCount == 0;
                }
            }
            catch (Exception)
            {
                _hasError = true;
                return false;
            }
            finally
            {
                _decontructionSemaphore.Release();
            }
        }

        internal void AddOutgoingTcpMessage(TcpMessage message)
        {
            try
            {
                _decontructionSemaphore.WaitOne();
                //lock (_underDeconstructionMessages)
                {
                    _underDeconstructionMessages.Push(message);
                }
            }
            finally { _decontructionSemaphore.Release(); }
        }

        internal void PrepareForSending()
        {
            try
            {
                //_decontructionSemaphore.WaitOne();

                lock (_underDeconstructionMessages)
                {
                    //Debug.Assert(!HasData && (_underDeconstructionMessage == null || _underDeconstructionMessage.IsValid()));

                    if (_underDeconstructionMessages.Count > 0)
                        _underDeconstructionMessage = _underDeconstructionMessages.Pop();
                }
            }
            finally
            { //_decontructionSemaphore.Release(); 
            }
        }

        internal void DisarmIncomingMessage()
        {
            //Debug.Assert(_armedTcpMessage != null && _armedTcpMessage.IsValid());
            _armedTcpMessage = null;
            _underConstructionMessage = null;
        }

        internal TcpMessage ArmIncomingMessage()
        {
            //Debug.Assert(_underConstructionMessage != null && _underConstructionMessage.IsValid());
            _armedTcpMessage = _underConstructionMessage;
            return _underConstructionMessage;
        }

        internal void Reset()
        {

          try
          {
            _decontructionSemaphore.Release();
          }
          catch { }
            _decontructionSemaphore.WaitOne();

            _underDeconstructionMessage = null;
            _underDeconstructionMessages.Clear();
            _buffer.Reset();

            _decontructionSemaphore.Release();

            
            _hasError = false;

            _underConstructionMessage = null;


            //Debug.Assert(_underDeconstructionMessages.Count == 0);

            _armedTcpMessage = null;
        }
    }
}
