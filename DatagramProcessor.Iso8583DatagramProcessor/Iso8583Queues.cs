using System;
using Iso8583HostTcpCommunicationLibrary.Queue;
using Corp.RouterService.Message;
using System.Transactions;
using Corp.RouterService.Message.DatagramProcessor;
using System.Collections.Generic;

namespace Corp.RouterService.Adapter.SqlAdapter
{
  public class Iso8583Queues : IQueues
  {
    private InputQueue _inputQueue;
    private OutputQueue _outputQueue;
    private string _serverGuid;
    private string _connectionString;
    private string _serverName;
    TransactionOptions _options;
    private Dictionary<string, Dictionary<string, string>> _appSettings;

    Iso8583DatagramProcessor _DatagramProcessor = null;

    public Iso8583Queues(string serverGuid, string connectionString, string serverName, Dictionary<string, Dictionary<string, string>> appSettings)
    {
      _serverGuid = serverGuid;
      _connectionString = connectionString;
      _serverName = serverName;
      //init the DAL
      _inputQueue = InputQueue.Queue;
      _outputQueue = OutputQueue.Queue;

      _options = new TransactionOptions();
      _options.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
      _options.Timeout = new TimeSpan(0, 1, 0);

      _DatagramProcessor = new Iso8583DatagramProcessor();

      _appSettings = appSettings;
    }

    public void InputQueueEnqueue(Message.Message message)
    {
      throw new NotImplementedException();
    }

    public Message.Message InputQueueDequeue()
    {
      //use a local transaction in case of network problem to rollback the queue
      Message.Message message = null;
      using (TransactionScope tx = new TransactionScope(TransactionScopeOption.RequiresNew, _options))
      {
        string data = _inputQueue.Dequeue(_serverGuid);
        if (string.IsNullOrEmpty(data))
          return null;
        message = new Message.Message(MessageType.Iso8583, data, new MessageEndpoints() { LocalEndpoint = new MessageEndpoint(new Uri("sql://" + _serverName)) });

        _DatagramProcessor.PreprocessMessage(ref message);

        tx.Complete();
      }
      return message;
    }

    public void OutputQueueEnqueue(Message.Message message)
    {
      int queueID = _outputQueue.Enqueue(
          _serverGuid,
          Guid.NewGuid().ToString(),
          message.Payload,
          message.ProcessorData.TransactionType,
          message.ID);
    }

    public Message.Message OutputQueueDequeue()
    {
      throw new NotImplementedException();
    }

  }
}
