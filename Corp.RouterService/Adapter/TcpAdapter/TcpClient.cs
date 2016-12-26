using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Corp.RouterService.Common;
using Corp.RouterService.Connection;
using Corp.RouterService.Message;
using Corp.RouterService.Services;
using Corp.RouterService.TcpServer;


namespace Corp.RouterService.Adapter.TcpAdapter
{


  public class TcpClient : IDisposable
  {
    private static LoggingLibrary.Log4Net.ILog log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    #region Members

    private TcpServerMemoryManager _memoryManager;

    private TcpServerEventArgsPools _argumentPools;

    private Socket _tcpClientSocket;

    private TcpServerLiveStatistics _liveStatistics;

    private TcpClientSettings _settings;

    private ServiceManager _servicesManager;

    private SocketAsyncEventArgs _messageEventArgs;

    private TcpMessageDispatcher _messageDispatcher;

    MessageCallbackRegistry _messageRegistry = null;
    
    private bool _shutingdown = false;
    #endregion

    #region Properties

    internal TcpClientSettings Settings
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
      get { return _liveStatistics.State; }
      set
      {
        _liveStatistics.State = value;
      }
    }

    #endregion

    #region ctor

    public TcpClient(TcpClientSettings settings)
    {
      if (log.IsInfoEnabled)
      {
        log.Info("Creating " + settings.ToString());
      }

      Settings = settings;

      _memoryManager = new TcpServerMemoryManager(settings);

      _liveStatistics = new TcpServerLiveStatistics(settings);

      _argumentPools = new TcpServerEventArgsPools(settings, ConnectionsEventFactory, MessagesEventFactory, _liveStatistics);
    

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

      _messageRegistry = new MessageCallbackRegistry(_settings.TimeoutInMilliseconds, _settings.MaxMessagesPerSecond); 

      _memoryManager.Initialize();

      _argumentPools.Initialize();

      _servicesManager.Initialize();

      _messageDispatcher = new TcpMessageDispatcher(_settings, _liveStatistics);

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
          if (_shutingdown == true)
            break;

          _tcpClientSocket = new Socket(_settings.RemoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
          _tcpClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveBuffer, _settings.IncomingDirectionSettings.NetworkBufferSize);
          _tcpClientSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);

          _tcpClientSocket.Bind(_settings.LocalEndPoint);

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

      //non blocking call
      Task.Factory.StartSafeNew(() => Connect());

      if (log.IsInfoEnabled)
      {
        log.Info("Started " + _settings.ToString());
      }
    }

    #region Connect

    private void Connect()
    {

      if (log.IsInfoEnabled)
      {
        log.Info("Connecting " + _settings.ToString());
      }

      _liveStatistics.ConnectionsThrottler.WaitOne();

      SocketAsyncEventArgs acceptEventArg = _argumentPools.ConnectionsPool.GetEventArg();

      while (true)
      {
        try
        {
          if (_shutingdown == true)
            break;
          _tcpClientSocket.Connect(acceptEventArg.RemoteEndPoint);

        }
        catch
        {
          Thread.Sleep(3000);
          continue;
        }
        break;
      }
      {
        //ether way this callback will be called
        //Connect_Completed(this, acceptEventArg);
      }

      //Debug.WriteLine("Connected " + connectionEventArgs.GetConnectionToken().TokenID);

      _liveStatistics.IncrementActiveConnections();

      _messageEventArgs = GenerateMessageEventArgs(acceptEventArg, _tcpClientSocket);

      State = TcpServerState.Started;

      Task.Factory.StartSafeNew(() => StartReceive(_messageEventArgs));


      if (log.IsInfoEnabled)
      {
        log.Info("Connected " + _settings.ToString());
      }
    }

    void Connect_Completed(object sender, SocketAsyncEventArgs e)
    {
      //the order of these calls doesn't matter
      Task.Factory.StartSafeNew(() => Process_Connect(e));
      Task.Factory.StartSafeNew(() => Connect());
    }

    private void Process_Connect(SocketAsyncEventArgs connectionEventArgs)
    {
      ////connected
      //if (connectionEventArgs.SocketError != SocketError.Success)
      //{
      //    HandleBadAccept(connectionEventArgs);
      //    return;
      //}

      ////Debug.WriteLine("Connected " + connectionEventArgs.GetConnectionToken().TokenID);

      //_liveData.IncrementActiveConnections();

      //_messageEventArgs = GenerateMessageEventArgs(connectionEventArgs);

      //State = RouterServiceState.Started;

      //Task.Factory.StartSafeNew(() => StartReceive(_messageEventArgs));
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
        TcpMessageToken messageToken = receiveMessageEventArgs.GetMessageToken();

        if (receiveMessageEventArgs.SocketError != SocketError.Success || receiveMessageEventArgs.BytesTransferred == 0 || State != TcpServerState.Started)
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

            messageToken.ArmIncomingMessage(receiveMessageEventArgs.AcceptSocket.RemoteEndPoint as IPEndPoint, receiveMessageEventArgs.AcceptSocket.RemoteEndPoint as IPEndPoint);

            //var sendMessageEventArgs = messageToken.SendMessageEventArgs;// GenerateSendMessageEventArgs(receiveMessageEventArgs);
            

            AddIncomingMessage(messageToken.MessageHandler.ArmedTcpMessage);

            //_DatagramProcessor.DispatchMessage(messageToken, sendMessageToken, new Task(() => StartSend(sendMessageEventArgs)));

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

    private void AddIncomingMessage(TcpMessage tcpServerMessage)
    {
      if (log.IsDebugEnabled)
      {
        log.Debug("AddIncomingMessage " + _settings.ToString());
      }

      //create the dto
      Message.Message message = tcpServerMessage.ToMessage();

      if(!_messageRegistry.Notify(message, message))      
        {
          //we got a request from the connected server. this should be an 800 message
          _messageDispatcher.DispatchMessage(message, new CompletionDelegate(Send));
          if (log.IsWarnEnabled)
          {
              log.Warn("Received Message with ID " + message.ID + " with no recipient!");
          }
        }
    }

    #endregion

    #region Send

    public void Send(Message.Message message)
    {
      AddOutgoingMessage(message, null);
    }

    public void Send(Message.Message message, CompletionDelegate completionDelegate)
    {
      AddOutgoingMessage(message, completionDelegate);
    }

    private void AddOutgoingMessage(Message.Message message, CompletionDelegate completionDelegate)
    {
      if (log.IsDebugEnabled)
      {
        log.Debug("AddOutgoingMessage " + _settings.ToString());
      }

      //one way message?
      if (message == null)
        return;

      if (this.State != TcpServerState.Started)
      {
        //disconnected
        //burn the message
        if (log.IsInfoEnabled)
        {
            log.Info("Disconnected. Ignoring the message" + _settings.ToString());
        }
        return;
      }
      _messageRegistry.Register(message, completionDelegate, e => {

        if (e == null)
        {
          if (log.IsWarnEnabled)
          {
            log.Warn("Timeout in " + _settings + " for message " + message.ID);
          }
          return;
        }
        var args = _messageEventArgs.GetMessageToken().SendMessageEventArgs;

        args.GetMessageToken().MessageHandler.AddOutgoingTcpMessage(e.ToTcpMessage(_settings.OutgoingDirectionSettings));

        Task.Factory.StartSafeNew(() => StartSend(args));
      });

     
    }


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

        //try sending the message only in one send(!!)
        if (messageToken.MessageHandler.TryDeconstructTcpMessage() &&
            messageToken.MessageHandler.HasData &&
            messageToken.Lock())
        {

          //Copy the bytes to the buffer associated with this SAEA object.


          if (sendMessageEventArgs.AcceptSocket != null)
          {
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

        if (sendMessageEventArgs.SocketError != SocketError.Success || _shutingdown == true)
        {
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
          //Task.Factory.StartSafeNew(() => StartReceive(sendMessageEventArgs.GetMessageToken().SendMessageEventArgs));


        }
      }
      catch
      {
      }
    }

    #endregion

    #region Helpers

    private SocketAsyncEventArgs GenerateMessageEventArgs(SocketAsyncEventArgs connectionEventArgs, Socket connectionSocket)
    {
      //accepted a new connection. Let's get the incoming messages            
      //create the message event argument that will listen to incoming data
      lock (_argumentPools)
      {
        SocketAsyncEventArgs messageEventArgs = _argumentPools.MessagesPool.GetEventArg();

        messageEventArgs.AcceptSocket = connectionSocket;

        messageEventArgs.GetMessageToken().SendMessageEventArgs.AcceptSocket = connectionSocket;

        messageEventArgs.GetMessageToken().ConnectionTokenID = connectionEventArgs.GetConnectionToken().TokenID;

        messageEventArgs.GetMessageToken().SendMessageEventArgs.GetMessageToken().ConnectionTokenID = connectionEventArgs.GetConnectionToken().TokenID;

        //connectionEventArgs.AcceptSocket = null;

        //reuse the connection event arg. We are done acception this connection
        _argumentPools.ConnectionsPool.Push(connectionEventArgs);

        return messageEventArgs;
      }
    }

    private void CloseClientSocket(SocketAsyncEventArgs e)
    {
      if (log.IsInfoEnabled)
      {
        log.Info("CloseClientSocket " + _settings.ToString());
      }

      State = TcpServerState.Stopping;

      TcpMessageToken receiveMessageToken = e.GetMessageToken();

      var sendE = receiveMessageToken.SendMessageEventArgs;

      TcpMessageToken sendMessageToken = sendE.GetMessageToken();

      //Trace.WriteLine("Disconnected: " + messageToken.ConnectionTokenID);            
      //var sendArg = messageToken.SendMessageEventArgs;
      receiveMessageToken.Reset();
      sendMessageToken.Reset();

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

      // Put the SocketAsyncEventArg back into the pool,
      // to be used by another client. This             

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

      _messageRegistry.Clear();

      State = TcpServerState.Stopped;

      //this is the client mode: reconnect, unless we are stopping 
      if (_shutingdown == false)
        Task.Factory.StartSafeNew(() => Start());

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

      connectionEventArg.RemoteEndPoint = (EndPoint)_settings.RemoteEndPoint;

      connectionEventArg.Completed += Connect_Completed;

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
      sendMessageToken.ReceiveMessageEventArgs = receiveEventArg;


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
      try
      {
        _shutingdown = true;
      }
      catch (Exception ex)
      {
        if (log.IsErrorEnabled)
        {
          log.Error(ex.ToString());
        }
      }
    }

    public void Dispose()
    {
      try
      {
        if (_tcpClientSocket != null)
          _tcpClientSocket.Close();
      }
      catch { }
    }
  }
}
