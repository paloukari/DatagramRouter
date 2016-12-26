using System;
using OperationTcpCommunicationLibrary.Queues;
using Corp.RouterService.Message;
using System.Transactions;
using Corp.RouterService.Message.DatagramProcessor;
using System.Collections.Generic;


namespace Corp.RouterService.Adapter.SqlAdapter
{
    public class OperationQueues : IQueues
    {
        private InputQueue _inputQueue;
        private OutputQueue _outputQueue;
        private string _serverGuid;
        private string _connectionString;
        private string _serverName;
        TransactionOptions _options;

        OperationDatagramProcessor _DatagramProcessor = null;
        private Dictionary<string, Dictionary<string, string>> _appSettings;

        public OperationQueues(string serverGuid, string connectionString, string serverName, Dictionary<string, Dictionary<string, string>> appSettings)
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

            _DatagramProcessor = new OperationDatagramProcessor();

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
                message = new Message.Message(MessageType.Operation, data, new MessageEndpoints() { LocalEndpoint = new MessageEndpoint(new Uri("sql://" + _serverName)) });
                _DatagramProcessor.PreprocessMessage(ref message);
                tx.Complete();
            }
            return message;
        }

        public void OutputQueueEnqueue(Message.Message message)
        {
          if (message != null)
          {
            if (!string.IsNullOrEmpty(message.HeaderSuffix))
            {
              int queueID = _outputQueue.Enqueue(
                  _serverGuid,
                  message.ID,
                  message.HeaderSuffix + message.Payload,
                  message.ProcessorData.TransactionType,
                  message.ProcessorData.MessageID);
            }
            else
            {
              int queueID = _outputQueue.Enqueue(
                  _serverGuid,
                  message.ID,
                  message.Payload,
                  message.ProcessorData.TransactionType,
                  message.ProcessorData.MessageID);
            }
          }
        }

        public Message.Message OutputQueueDequeue()
        {
            throw new NotImplementedException();
        }
    }
}
