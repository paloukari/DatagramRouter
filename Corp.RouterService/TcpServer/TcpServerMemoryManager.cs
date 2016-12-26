using System;
using System.Net.Sockets;
using Corp.RouterService.Memory;

namespace Corp.RouterService.TcpServer
{
    class TcpServerMemoryManager
    {
        TcpServerSettings _settings;

        private MemoryManager _receiveMessageMemoryManager;
        private MemoryManager _sendMessageMemoryManager;


        private TcpServerMemoryManager()
        {

        }

        internal TcpServerMemoryManager(TcpServerSettings settings)
        {
            _settings = settings;
            _receiveMessageMemoryManager = new MemoryManager(settings.PoolSize,
                settings.IncomingDirectionSettings.NetworkBufferSize);
            _sendMessageMemoryManager = new MemoryManager(settings.PoolSize,
                settings.OutgoingDirectionSettings.NetworkBufferSize);

        }

        internal void Initialize()
        {
            _receiveMessageMemoryManager.InitializeBuffer();
            _sendMessageMemoryManager.InitializeBuffer();
        }

        internal bool SetBuffer(SocketAsyncEventArgs args, IO_Direction direction)
        {
            if (direction == IO_Direction.Incoming)
                return _receiveMessageMemoryManager.SetBuffer(args);
            else if (direction == IO_Direction.Outgoing)
                return _sendMessageMemoryManager.SetBuffer(args);
            else
                throw new ArgumentException("Direction can be ether In or Out", "direction");
        }
    }
}
