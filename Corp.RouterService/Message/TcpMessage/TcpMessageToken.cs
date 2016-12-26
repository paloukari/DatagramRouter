using System;
using System.Net.Sockets;
using System.Threading;
using Corp.RouterService.TcpServer;

namespace Corp.RouterService.Message
{
    internal class TcpMessageToken
    {
        private static Int32 _ID = 0;
        private Int32 _tokenID = Interlocked.Increment(ref _ID);
        private Int32 _connectionTokenID;
        private IO_Direction _direction;
        Semaphore _sendSemaphore;
        internal IO_Direction Direction
        {
            get { return _direction; }
        }

        TcpMessage _tcpMessage;
        TcpMessageHandler _messageHandler;
        
        internal TcpMessageHandler MessageHandler
        {
            get { return _messageHandler; }
        }

        private object _syncRoot = new object();
        private object _syncRoot2 = new object();
        private bool _sendStarted = false;
        private bool _isLocked = false;
        private bool _hasBeenReset = false;

        internal bool Lock()
        {
            if (!_isLocked)
            {
                lock (_syncRoot)
                {
                    if (!_isLocked)
                    {
                        if (_hasBeenReset)
                            return false;

                        _isLocked = true;
                        return true;
                    }
                }
            }
            return false;
        }
        internal bool IsLocked
        {
            get
            {
                lock (_syncRoot)
                {
                  return _hasBeenReset || _isLocked;
                }
            }
        }

        internal bool SendStarted
        {
            get
            {
                lock (_syncRoot2)
                {
                    return _sendStarted;
                }
            }
            set
            {
                lock (_syncRoot2)
                {
                    _sendStarted = value;
                }
            }
        }
        internal void Unlock() { _isLocked = false; }

        internal TcpMessage TcpMessage
        {
            get { return _tcpMessage; }
            set { _tcpMessage = value; }
        }

        internal Int32 TokenId
        {
            get { return _tokenID; }
        }

        internal Int32 ConnectionTokenID
        {
            get { return _connectionTokenID; }
            set
            {
                _connectionTokenID = value;
                _hasBeenReset = false;
            }
        }

        internal TcpMessageToken(SocketAsyncEventArgs e,
            Int32 receiveBufferOffset,
            IO_Direction direction,
            TcpTrafficSettings messagesSettings
            )
        {
            _direction = direction;

            _messageHandler = new TcpMessageHandler(messagesSettings,
                receiveBufferOffset,
                TokenId);
            _sendSemaphore = new Semaphore(1, 1);
        }

        internal void Reset()
        {

            _tcpMessage = null;
            _hasBeenReset = true;
          
            lock (_syncRoot)
            {              
              _isLocked = false;
            }
            _sendStarted = false;

            try
            {
              _sendSemaphore.Release();
            }
            catch { }
            _sendSemaphore.WaitOne();
            _messageHandler.Reset();
            _sendSemaphore.Release();

            
        }



        internal SocketAsyncEventArgs SendMessageEventArgs { get; set; }
        internal SocketAsyncEventArgs ReceiveMessageEventArgs { get; set; }




        internal void DisarmIncomingMessage()
        {
            //Debug.Assert((_tcpMessage == null || _tcpMessage.IsValid()) && _messageHandler.ArmedTcpMessage.IsValid());

            _messageHandler.DisarmIncomingMessage();

            _tcpMessage = null;
        }

        internal void ArmOutgoingMessage()
        {
            //Debug.Assert((_tcpMessage == null || _tcpMessage.IsValid()) && _messageHandler.ArmedTcpMessage.IsValid());

            _tcpMessage = _messageHandler.ArmedTcpMessage;
        }

        internal void DisarmGoingMessage()
        {
            //Debug.Assert((_tcpMessage == null || _tcpMessage.IsValid()) && _messageHandler.ArmedTcpMessage.IsValid());

            _messageHandler.DisarmIncomingMessage();

        }


        internal void LockForSend()
        {
            _sendSemaphore.WaitOne();
        }

        internal void UnlockForSend()
        {
            try
            {
                _sendSemaphore.Release();
            }
            catch { }
        }

        internal void LockForReceive()
        {
            _sendSemaphore.WaitOne();
        }

        internal void UnlockForReceive()
        {
            try
            {
                _sendSemaphore.Release();
            }
            catch { }
        }

        internal void ArmIncomingMessage(System.Net.IPEndPoint localEndPoint, System.Net.IPEndPoint remoteEndPoint)
        {
            _tcpMessage = _messageHandler.ArmIncomingMessage();

            _tcpMessage.LocalEndpoint = new TcpMessageEndpoint(localEndPoint);

            _tcpMessage.RemoteEndpoint = new TcpMessageEndpoint(remoteEndPoint);

            //Debug.Assert(_tcpMessage.IsValid());
        }
    }
}
