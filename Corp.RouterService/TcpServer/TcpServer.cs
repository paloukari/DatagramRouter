
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Corp.RouterService.Common;
using Corp.RouterService.Connection;
using Corp.RouterService.Message;
using Corp.RouterService.Services;



namespace Corp.RouterService.TcpServer
{

  internal class TcpServer : IDisposable
  {
    private static LoggingLibrary.Log4Net.ILog log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    #region Members

    private TcpServerMemoryManager _memoryManager;

    private TcpServerEventArgsPools _argumentPools;

    private Socket _tcpServerSocket;

    private TcpServerLiveStatistics _liveStatistics;

    private TcpServerSettings _settings;

    private TcpMessageDispatcher _DatagramProcessor;

    private ServiceManager _servicesManager;

    private object stateSyncObj = new object();

    #endregion

    #region Properties

    internal TcpServerSettings Settings
    {
      get { return _settings; }
      set
      {
        if (_settings != value)
        {
          if (_settings != null)
            _settings.UpdateSettings(value);
          else
            _settings = value;
        }
      }
    }

    private TcpServerState State
    {
      get
      {
        lock (stateSyncObj)
        {
          return _liveStatistics.State;
        }
      }
      set
      {
        lock (stateSyncObj)
        {
          if (log.IsInfoEnabled)
          {
            log.Info("State change from " + _liveStatistics.State + " to " + value + " " + _settings.ToString());
          }
          _liveStatistics.State = value;
        }
      }
    }

    #endregion

    #region ctor

    public TcpServer(TcpServerSettings settings)
    {
      if (log.IsInfoEnabled)
      {
        log.Info("Creating " + settings.ToString());
      }

      Settings = settings;

      _memoryManager = new TcpServerMemoryManager(settings);

      _liveStatistics = new TcpServerLiveStatistics(settings);

      _argumentPools = new TcpServerEventArgsPools(settings, ConnectionsEventFactory, MessagesEventFactory, _liveStatistics);

      _DatagramProcessor = new TcpMessageDispatcher(settings, _liveStatistics);

      _servicesManager = new ServiceManager(settings);

      State = TcpServerState.Uninitialized;

      if (log.IsInfoEnabled)
      {
        log.Info("Created " + settings.ToString());
      }
    }

    #endregion

    public void Initialize()
    {
      if (log.IsInfoEnabled)
      {
        log.Info("Initializing " + _settings.ToString());
      }
      State = TcpServerState.Initializing;

      _memoryManager.Initialize();

      _argumentPools.Initialize();

      _servicesManager.Initialize();

      State = TcpServerState.Initialized;

      if (log.IsInfoEnabled)
      {
        log.Info("Initialized " + _settings.ToString());
      }
    }

    public void Start()
    {
      if (log.IsInfoEnabled)
      {
        log.Info("Starting " + _settings.ToString());
      }

      State = TcpServerState.Starting;


      while (true)
      {
        try
        {
          if (State != TcpServerState.Starting)
            return;

          if (_tcpServerSocket == null)
            _tcpServerSocket = new Socket(_settings.LocalEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

          lock (_tcpServerSocket)
          {
            if (_tcpServerSocket.IsBound)
              break;

            _tcpServerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, _settings.IncomingDirectionSettings.NetworkBufferSize);
            _tcpServerSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);


            _tcpServerSocket.LingerState = new LingerOption(true, 0);

            _tcpServerSocket.Bind(_settings.LocalEndPoint);

            if (!_tcpServerSocket.IsBound)
            {
              if (log.IsErrorEnabled)
              {
                log.Error("Socket not bound!! " + _settings.ToString());
              }
              continue;
            }

            _tcpServerSocket.Listen(_settings.ConnectionsBacklog);

            State = TcpServerState.Started;

          }
        }
        catch (Exception ex)
        {

          if (log.IsErrorEnabled)
          {
            log.Error("Binding Error :" + ex.ToString() + _settings.ToString());
          }
          Thread.Sleep(3000);
          continue;
        }
        break;
      }



      State = TcpServerState.Started;

      Task.Factory.StartSafeNew(() => Start_Accept());

      if (log.IsInfoEnabled)
      {
        log.Info("Started " + _settings.ToString());
      }

    }

    #region Accept

    private void Start_Accept()
    {
      if (log.IsDebugEnabled)
      {
        log.Debug("Start_Accept " + _settings.ToString());
      }

      if (State != TcpServerState.Started)
      {
        if (log.IsWarnEnabled)
        {
          log.Warn("Server not started. Ignoring socket accept " + _settings.ToString());
        }
        return;
      }
      SocketAsyncEventArgs acceptEventArg = _argumentPools.ConnectionsPool.GetEventArg();

      _liveStatistics.ConnectionsThrottler.WaitOne();

      if (!_tcpServerSocket.AcceptAsync(acceptEventArg))
      {
        //ether way this callback will be called
        Accept_Completed(this, acceptEventArg);
      }
    }

    void Accept_Completed(object sender, SocketAsyncEventArgs e)
    {
      if (log.IsDebugEnabled)
      {
        log.Debug("Accept_Completed " + _settings.ToString());
      }
      //the order of these calls doesn't matter
      Task.Factory.StartSafeNew(() => Process_Accept(e));
      Task.Factory.StartSafeNew(() => Start_Accept());
    }

    private void Process_Accept(SocketAsyncEventArgs connectionEventArgs)
    {
      if (log.IsDebugEnabled)
      {
        log.Debug("Process_Accept " + _settings.ToString());
      }
      //new connection request            
      if (connectionEventArgs.SocketError != SocketError.Success)
      {
        HandleBadAccept(connectionEventArgs);
        return;
      }

      //Debug.WriteLine("Connected " + connectionEventArgs.GetConnectionToken().TokenID);

      _liveStatistics.IncrementActiveConnections();

      SocketAsyncEventArgs receiveMessageEventArgs = GenerateMessageEventArgs(connectionEventArgs);

      Task.Factory.StartSafeNew(() => StartReceive(receiveMessageEventArgs));
    }

    #endregion

    #region Receive

    private void StartReceive(SocketAsyncEventArgs receiveMessageEventArgs)
    {
      if (log.IsDebugEnabled)
      {
        log.Debug("StartReceive " + _settings.ToString());
      }
      try
      {
        if (receiveMessageEventArgs.AcceptSocket != null && !receiveMessageEventArgs.AcceptSocket.ReceiveAsync(receiveMessageEventArgs))
        {
          Task.Factory.StartSafeNew(() => ProcessReceive(receiveMessageEventArgs));
        }
      }
      catch
      {
        CloseClientSocket(receiveMessageEventArgs);
        //i've seen receiveMessageEventArgs.AcceptSocket == null in receiveMessageEventArgs.AcceptSocket.ReceiveAsync inside if
      }
    }

    private void ProcessReceive(SocketAsyncEventArgs receiveMessageEventArgs)
    {
      if (log.IsDebugEnabled)
      {
        log.Debug("ProcessReceive " + _settings.ToString());
      }
      try
      {
        if (State != TcpServerState.Started)
          CloseClientSocket(receiveMessageEventArgs);

        if (receiveMessageEventArgs.AcceptSocket == null)
          return;
        TcpMessageToken messageToken = receiveMessageEventArgs.GetMessageToken();

        if (receiveMessageEventArgs.SocketError != SocketError.Success || receiveMessageEventArgs.BytesTransferred == 0)
        {
          //wait for the loop to complete first
          while (!messageToken.Lock())
          {
            Thread.Sleep(1);
            //Debug.WriteLine("In sleep..");
          }

          CloseClientSocket(receiveMessageEventArgs);

          messageToken.Unlock();
          return;
        }

        _liveStatistics.IncrementReceivedMessagesBytes(receiveMessageEventArgs.BytesTransferred);
        //empty the buffer and receive new data from socket
        messageToken.MessageHandler.AddData(receiveMessageEventArgs.Buffer, receiveMessageEventArgs.BytesTransferred);

        Task.Factory.StartSafeNew(() => StartReceive(receiveMessageEventArgs));

        //this is equal to ether the message total size, or the buffer size if the message is bigger
        if (messageToken.IsLocked)
          return;

        while (messageToken.MessageHandler.HasData && messageToken.Lock())
        {
          if (messageToken.MessageHandler.TryConstructTcpMessage())
          {
            //if we have not handled the header yet.Handle means read header bytes and determine the message size from the header
            if (!messageToken.MessageHandler.IsValid())
            {
              //couldn't understand the message.
              //bail out
              CloseClientSocket(receiveMessageEventArgs);

              return;
            }


            _liveStatistics.IncrementReceivedMessages();

            messageToken.ArmIncomingMessage(receiveMessageEventArgs.AcceptSocket.LocalEndPoint as IPEndPoint, receiveMessageEventArgs.AcceptSocket.RemoteEndPoint as IPEndPoint);

            var sendMessageEventArgs = messageToken.SendMessageEventArgs;// GenerateSendMessageEventArgs(receiveMessageEventArgs);

            TcpMessageToken sendMessageToken = sendMessageEventArgs.GetMessageToken();

            _DatagramProcessor.DispatchMessage(messageToken, sendMessageToken, new Task(() =>
            {
              //the dispatch is the most time consuming. if the connection has been closed, do nothing
              StartSend(sendMessageEventArgs);
            }));

            messageToken.DisarmIncomingMessage();
          }

          messageToken.Unlock();
        }
      }
      catch
      {
        CloseClientSocket(receiveMessageEventArgs);
      }
    }




    #endregion

    #region Send

    private void StartSend(SocketAsyncEventArgs sendMessageEventArgs)
    {
      if (log.IsDebugEnabled)
      {
        log.Debug("StartSend " + _settings.ToString());
      }
      try
      {
        TcpMessageToken messageToken = sendMessageEventArgs.GetMessageToken();

        //http://msdn.microsoft.com/en-us/library/system.net.sockets.socketasynceventargs.setbuffer.aspx

        if (messageToken.IsLocked)
          return;
        //DeconstructTcpMessage
        //copy block
        //send

        if (sendMessageEventArgs.AcceptSocket != null)//&& sendMessageEventArgs.AcceptSocket.Connected)
        {
          //try sending the message only in one send(!!)
          if (messageToken.MessageHandler.TryDeconstructTcpMessage() &&
              messageToken.MessageHandler.HasData &&
              messageToken.Lock())
          {

            //Copy the bytes to the buffer associated with this SAEA object.
            try
            {
              //this may fail 'cause the socket may close by receive process..
              lock (sendMessageEventArgs)
              {
                messageToken.LockForSend();

                int count = messageToken.MessageHandler.RemoveData(sendMessageEventArgs.Buffer);

                sendMessageEventArgs.SetBuffer(sendMessageEventArgs.Offset, count);

                if (!sendMessageEventArgs.AcceptSocket.SendAsync(sendMessageEventArgs))
                {
                  Task.Factory.StartSafeNew(() =>
                  {
                    ProcessSend(sendMessageEventArgs);
                  });
                }
              }
            }
            catch (Exception ex)
            {
              if (log.IsErrorEnabled)
                log.Error(ex.ToString());
              //Debug.Assert(!(sendMessageEventArgs.AcceptSocket != null && sendMessageEventArgs.AcceptSocket.Connected), "Check me!!!");
              return;
            }
          }
          else
          {
            //cannot send. Socket is closed..
            //log this and return.
            //don't close the socket. I will be by receive
            //CloseClientSocket(sendMessageEventArgs);
            return;

          }
        }
      }
      catch
      {
      }
    }

    private void ProcessSend(SocketAsyncEventArgs sendMessageEventArgs)
    {
      if (log.IsDebugEnabled)
      {
        log.Debug("ProcessSend " + _settings.ToString());
      }

      try
      {
        TcpMessageToken messageToken = sendMessageEventArgs.GetMessageToken();

        if (sendMessageEventArgs.SocketError != SocketError.Success)
        {
          messageToken.UnlockForSend();
          messageToken.Unlock();
          return;
        }
        else
        {
          _liveStatistics.IncrementSentMessagesBytes(sendMessageEventArgs.BytesTransferred);

          if (sendMessageEventArgs.BytesTransferred != 0)
            _liveStatistics.IncrementSentMessages();
          if (!messageToken.MessageHandler.HasData)
          {

            //prepare next message and start sending it
            //messageToken.MessageHandler.PrepareForSending();
          }
          messageToken.UnlockForSend();
          messageToken.Unlock();

          // So let's loop back to StartSend().
          Task.Factory.StartSafeNew(() => StartSend(sendMessageEventArgs));

        }
      }
      catch
      {
      }
    }

    #endregion

    #region Helpers

    private SocketAsyncEventArgs GenerateMessageEventArgs(SocketAsyncEventArgs connectionEventArgs)
    {
      //accepted a new connection. Let's get the incoming messages            
      //create the message event argument that will listen to incoming data
      lock (_argumentPools)
      {
        SocketAsyncEventArgs messageEventArgs = _argumentPools.MessagesPool.GetEventArg();

        messageEventArgs.AcceptSocket = connectionEventArgs.AcceptSocket;

        messageEventArgs.GetMessageToken().SendMessageEventArgs.AcceptSocket = connectionEventArgs.AcceptSocket;

        messageEventArgs.GetMessageToken().ConnectionTokenID = connectionEventArgs.GetConnectionToken().TokenID;

        messageEventArgs.GetMessageToken().SendMessageEventArgs.GetMessageToken().ConnectionTokenID = connectionEventArgs.GetConnectionToken().TokenID;

        connectionEventArgs.AcceptSocket = null;

        //reuse the connection event arg. We are done accepting this connection
        _argumentPools.ConnectionsPool.Push(connectionEventArgs);

        return messageEventArgs;
      }
    }



    private void CloseClientSocket(SocketAsyncEventArgs e)
    {
      try
      {
        if (log.IsInfoEnabled)
        {
          log.Info("CloseClientSocket " + _settings.ToString());
        }

        SocketAsyncEventArgs sendE = null;
        TcpMessageToken receiveMessageToken = e.GetMessageToken();
        sendE = receiveMessageToken.SendMessageEventArgs;

        // do a shutdown before you close the socket
        if (e.AcceptSocket != null)
        {
          try
          {
            e.AcceptSocket.Shutdown(SocketShutdown.Both);
          }
          // throws if socket was already closed
          catch (SocketException sex)
          {
            if (log.IsErrorEnabled)
              log.Error(sex.ToString());
            //Debug.Assert(sex.ErrorCode == 10054, "A socket exception other than connection closed occured during Socket Shutdown");
          }
          catch (Exception ex)
          {
            if (log.IsErrorEnabled)
              log.Error(ex.ToString());
            //Debug.Assert(false, "A exception other than SocketException occured during Socket Shutdown");
          }
          finally
          {
            //This method closes the socket and releases all resources, both
            //managed and unmanaged. It internally calls Dispose.
            try
            {
              e.AcceptSocket.Close();
            }
            catch (Exception)
            {
            }
            e.AcceptSocket = null;
            sendE.AcceptSocket = null;
          }
        }


        try
        {

          TcpMessageToken sendMessageToken = sendE.GetMessageToken();
          //e.SocketError = SocketError.Success;
          //sendE.SocketError = SocketError.Success;

          //Trace.WriteLine("Disconnected: " + messageToken.ConnectionTokenID);            
          //var sendArg = messageToken.SendMessageEventArgs;

          if (receiveMessageToken != null)
            receiveMessageToken.Reset();
          if (sendMessageToken != null)
            sendMessageToken.Reset();

        }
        catch
        {
        }

        // Put the SocketAsyncEventArg back into the pool,
        // to be used by another client. This             

        if (_argumentPools != null)
          lock (_argumentPools)
          {
            _argumentPools.MessagesPool.Push(e);
          }

        // decrement the counter keeping track of the total number of clients 
        //connected to the server, for testing
        _liveStatistics.DecrementActiveConnections();

        //Release Semaphore so that its connection counter will be decremented.
        //This must be done AFTER putting the SocketAsyncEventArg back into the pool,
        //or you can run into problems.
        _liveStatistics.ConnectionsThrottler.Release();


        //todo: there is the case where the server socket gets destroyed if the system looses it's IP (reset the NIC)
        //sould test this and restart

        if (_tcpServerSocket != null)
          lock (_tcpServerSocket)
          {
            if (State != TcpServerState.Stopping && State != TcpServerState.Stopped && (_tcpServerSocket == null || !_tcpServerSocket.IsBound))
            {
              Task.Factory.StartSafeNew(() => Start());
            }
          }
      }
      catch (Exception ex)
      {
        if (log.IsErrorEnabled)
        {
          log.Error(_settings.ToString()+" "+ex.ToString());
        }
      }
    }

    private void HandleBadAccept(SocketAsyncEventArgs acceptEventArgs)
    {
      if (log.IsWarnEnabled)
      {
        log.Warn("HandleBadAccept " + _settings.ToString());
      }

      RouterServiceConnectionToken acceptOpToken = acceptEventArgs.GetConnectionToken();

      //This method closes the socket and releases all resources, both
      //managed and unmanaged. It internally calls Dispose.           
      acceptEventArgs.AcceptSocket.Close();

      //Put the SAEA back in the pool.
      _argumentPools.ConnectionsPool.Push(acceptEventArgs);
    }


    internal SocketAsyncEventArgs ConnectionsEventFactory(SocketAsyncEventArgsPool pool)
    {
      SocketAsyncEventArgs connectionEventArg = new SocketAsyncEventArgs();

      connectionEventArg.Completed += Accept_Completed;

      connectionEventArg.UserToken = new RouterServiceConnectionToken();

      return connectionEventArg;
    }

    internal SocketAsyncEventArgs MessagesEventFactory(SocketAsyncEventArgsPool pool)
    {
      SocketAsyncEventArgs receiveEventArg = new SocketAsyncEventArgs();

      SocketAsyncEventArgs sendEventArg = new SocketAsyncEventArgs();

      _memoryManager.SetBuffer(receiveEventArg, IO_Direction.Incoming);

      _memoryManager.SetBuffer(sendEventArg, IO_Direction.Outgoing);

      receiveEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(MessageIO_Completed);

      sendEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(MessageIO_Completed);

      TcpMessageToken receiveMessageToken;
      TcpMessageToken sendMessageToken;


      receiveMessageToken = new TcpMessageToken(receiveEventArg,
              receiveEventArg.Offset,
              IO_Direction.Incoming,
              _settings.IncomingDirectionSettings);

      sendMessageToken = new TcpMessageToken(sendEventArg,
              sendEventArg.Offset,
              IO_Direction.Outgoing,
              _settings.OutgoingDirectionSettings);

      receiveMessageToken.SendMessageEventArgs = sendEventArg;


      receiveEventArg.UserToken = receiveMessageToken;
      sendEventArg.UserToken = sendMessageToken;

      return receiveEventArg;

    }

    void MessageIO_Completed(object sender, SocketAsyncEventArgs e)
    {
      if (log.IsDebugEnabled)
      {
        log.Debug("MessageIO_Completed:" + e.LastOperation.ToString() + " " + _settings.ToString());
      }

      TcpMessageToken receiveSendToken = e.GetMessageToken();

      switch (e.LastOperation)
      {
        case SocketAsyncOperation.Receive:
          ProcessReceive(e);
          break;

        case SocketAsyncOperation.Send:
          ProcessSend(e);
          break;

        default:
          throw new ArgumentException("No receive or send operation");
      }
    }

    #endregion

    internal void Stop()
    {
      State = TcpServerState.Stopping;
      if (log.IsInfoEnabled)
      {
        log.Info("Stopping RouterService " + _settings.ToString());
      }
      State = TcpServerState.Stopped;
    }

    public void Dispose()
    {
      if (_tcpServerSocket != null)
      {
        try
        {
          _tcpServerSocket.Close();
          _tcpServerSocket = null;
        }
        catch { }
      }

    }
  }
}
