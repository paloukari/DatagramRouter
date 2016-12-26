using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Corp.RouterService.TcpServer
{
    internal sealed class SocketAsyncEventArgsPool
    {
        //from http://msdn.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.socketasynceventargs.aspx

        #region Members
        
        private EventArgsFactory _eventFactory;
        private Queue<SocketAsyncEventArgs> _pool;
        private int _poolSize;
        private IO_Direction _direction;
        private TcpServerLiveStatistics _tcpServerLiveStatistics;

        internal delegate SocketAsyncEventArgs EventArgsFactory(SocketAsyncEventArgsPool pool);
        #endregion

        #region Properties

        internal Int32 Count
        {
            get { return _pool.Count; }
        }

        #endregion


        internal SocketAsyncEventArgsPool(TcpServerLiveStatistics tcpServerLiveStatistics, Int32 poolSize, EventArgsFactory eventFactory, IO_Direction direction)
        {
            _tcpServerLiveStatistics = tcpServerLiveStatistics;
            _poolSize = poolSize;
            _eventFactory = eventFactory;
            _direction = direction;
        }

        internal void InitializePool()
        {
            _pool = new Queue<SocketAsyncEventArgs>(_poolSize);

            for (int i = 0; i < _poolSize; i++)
            {
                IncrementLiveData();
                _pool.Enqueue(_eventFactory(this));
            }
        }

        private void IncrementLiveData()
        {
            switch (_direction)
            {
                case IO_Direction.Incoming:
                    _tcpServerLiveStatistics.IncrementIncomingPoolSize();
                    break;                
                case IO_Direction.Connect:
                    _tcpServerLiveStatistics.IncrementConnectionsPoolSize();
                    break;
                default:
                    break;
            }             
        }
        private void DecrementLiveData()
        {
            switch (_direction)
            {
                case IO_Direction.Incoming:
                    _tcpServerLiveStatistics.DecrementIncomingPoolSize();
                    break;
                case IO_Direction.Connect:
                    _tcpServerLiveStatistics.DecrementConnectionsPoolSize();
                    break;
                default:
                    break;
            }   
        }

        private SocketAsyncEventArgs Pop()
        {
            lock (_pool)
            {
                DecrementLiveData();
                return _pool.Dequeue();
            }
        }

       

        internal void Push(SocketAsyncEventArgs item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");
            }
            lock (_pool)
            {
                IncrementLiveData();
                _pool.Enqueue(item);
            }
        }

        internal SocketAsyncEventArgs GetEventArg()
        {
            while (true)
            {
                SocketAsyncEventArgs eventArg = null;
                if (Count > 0)
                {
                    lock (this)
                    {
                        if (Count > 0)
                            eventArg = Pop();
                    }
                }
                if (eventArg != null)
                    return eventArg;
                Thread.Sleep(1);
            }
            //if (eventArg == null)
            //{
            //    Debug.Assert(false, "Throttling didn't work correctly!");
            //    eventArg = _eventFactory(this, _direction);
            //}
            //return eventArg;
        }
    }
}
