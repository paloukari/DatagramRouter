
namespace Corp.RouterService.TcpServer
{
    internal class TcpServerEventArgsPools
    {
        private SocketAsyncEventArgsPool _tcpConnectionsPool;
        private SocketAsyncEventArgsPool _tcpMessagesPool;
        private SocketAsyncEventArgsPool _tcpSendMessagesPool;
        private TcpServerLiveStatistics _tcpServerLiveStatistics;

        private SocketAsyncEventArgsPool.EventArgsFactory _connectionEventArgsFactory;
        private SocketAsyncEventArgsPool.EventArgsFactory _messageEventArgsFactory;

        internal SocketAsyncEventArgsPool ConnectionsPool
        {
            get { return _tcpConnectionsPool; }
        }
        internal SocketAsyncEventArgsPool MessagesPool
        {
            get { return _tcpMessagesPool; }
        }


        //hide the default ctor
        private TcpServerEventArgsPools()
        {

        }

        internal TcpServerEventArgsPools(TcpServerSettings settings,
            SocketAsyncEventArgsPool.EventArgsFactory connectionEventArgsFactory,
            SocketAsyncEventArgsPool.EventArgsFactory messageEventArgsFactory,
            TcpServerLiveStatistics tcpServerLiveStatistics)
        {
            _tcpServerLiveStatistics = tcpServerLiveStatistics;

            _tcpConnectionsPool = new SocketAsyncEventArgsPool(_tcpServerLiveStatistics, 
                settings.PoolSize,
                connectionEventArgsFactory, 
                IO_Direction.Connect);
            _tcpMessagesPool = new SocketAsyncEventArgsPool(_tcpServerLiveStatistics, 
                settings.PoolSize,
                messageEventArgsFactory, 
                IO_Direction.Incoming);
            _tcpSendMessagesPool = new SocketAsyncEventArgsPool(_tcpServerLiveStatistics, 
                settings.PoolSize, 
                messageEventArgsFactory, 
                IO_Direction.Outgoing);
            
            _connectionEventArgsFactory = connectionEventArgsFactory;
            _messageEventArgsFactory = messageEventArgsFactory;
        }

        internal void Initialize()
        {
            _tcpConnectionsPool.InitializePool();
            _tcpMessagesPool.InitializePool();            
        }
    }
}
