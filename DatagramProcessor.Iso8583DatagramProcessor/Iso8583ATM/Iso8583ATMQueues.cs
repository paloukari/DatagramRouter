using System;
using System.Collections.Generic;
using Corp.RouterService.Message;
using Corp.RouterService.Message.DatagramProcessor;
using Corp.Antigonis.SMSCommon;
using Iso8583Library;
using Iso8583Library.Formatters;


namespace Corp.RouterService.Adapter.SqlAdapter
{
  public class Iso8583ATMQueues : IQueues
  {
    private static readonly LoggingLibrary.Log4Net.ILog log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
        System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


    private string _serverGuid;
    private string _connectionString;
    private string _serverName;
    private Queue<Message.Message> _memoryQueue = null;
    private Iso8583ATMDatagramProcessor _DatagramProcessor = null;
    private IDictionary<string, string> _feedSourceIdDictionary;


    public Iso8583ATMQueues(
        string serverGuid,
        string connectionString,
        string serverName,
        Dictionary<string, Dictionary<string, string>> appSettings)
    {
      _serverGuid = serverGuid;
      _connectionString = connectionString;
      _serverName = serverName;

      _memoryQueue = new Queue<Message.Message>();

      _DatagramProcessor = new Iso8583ATMDatagramProcessor();

      if (appSettings.ContainsKey("feedSourceId") == true)
      {
        _feedSourceIdDictionary = appSettings["feedSourceId"];
      }
      else
      {
        _feedSourceIdDictionary = new Dictionary<string, string>();
      }
    }


    //this is incomplete
    //need to find a way to get the response out of sql

    
    //server mode
    public void InputQueueEnqueue(Message.Message message)
    {
      Message.Message response = null;

      var isoData = message.ProcessorData as Iso8583ATMData;

      if (isoData == null || isoData.Iso8583Msg == null)
      {
        response = _DatagramProcessor.ProcessMessage(message);
      }
      else
      {
        switch (isoData.Iso8583Msg.MessageTypeIdentifier)
        {
          case 220:
          case 221:
            response = RecordAndReplyToInternalAdviceMessages(
                230,
                new int[] { 3, 4, 6, 7, 11, 32, 35, 37, 39, 41, 49, 51 },
                message);
            break;
          case 420:
          case 421:
            response = RecordAndReplyToInternalAdviceMessages(
                430,
                new int[] { 3, 4, 6, 7, 11, 32, 35, 37, 39, 41, 49, 51, 90, 95 },
                message);
            break;
          default:
            {
              response = _DatagramProcessor.ProcessMessage(message);
              break;
            }
        }
      }
      if (response != null)
      {
        lock (_memoryQueue)
        {
          _memoryQueue.Enqueue(response);
        }
      }
      else
      {
        if (log.IsWarnEnabled)
        {
          log.Warn("Got request message " + message.ToString() + "message and failed to create response");
        }
      }
    }


    //client mode
    public Message.Message InputQueueDequeue()
    {
      throw new NotImplementedException();
    }


    //server mode
    public void OutputQueueEnqueue(Message.Message message)
    {
      throw new NotImplementedException();
    }


    //client mode
    public Message.Message OutputQueueDequeue()
    {
      lock (_memoryQueue)
      {
        if (_memoryQueue.Count > 0)
          return _memoryQueue.Dequeue();
        return null;
      }
    }


    #region Helpers


    private Message.Message RecordAndReplyToInternalAdviceMessages(
        int mti, int[] fieldsToCopy, Message.Message inMessage)
    {
      string result = default(string);

      RecordTheMessage(inMessage);

      Iso8583Msg message = new Iso8583Msg(mti);
      Iso8583MsgFormatter formatter = Iso8583MsgFormatterFactory.CreateIso8583MsgFormatter(
          Iso8583Library.Formatters.Iso8583MsgFormatterType.8583AtmVersion2012R01);

      Iso8583ATMData data = inMessage.ProcessorData as Iso8583ATMData;

      foreach (int field in fieldsToCopy)
      {
        var fieldData = data.Iso8583Msg[field];
        if (fieldData != null)
          message[field] = fieldData;
        else
        {
          if (log.IsDebugEnabled)
          {
            log.Debug("field:" + field + " is null in MTI:" + mti);
          }
        }
      }

      message.STAN = data.Iso8583Msg.STAN;

      result = message.ToAscii(formatter);

      Iso8583ATMData isodata = new Iso8583ATMData(result);

      Message.Message response = new Message.Message(
          MessageType.Iso8583ATM,
          result,
          inMessage.Info.OutgoingEndpoints,
          inMessage.HeaderText,
          inMessage.HeaderCopyData);
      _DatagramProcessor.PreprocessMessage(ref response);

      return response;
    }


    private void RecordTheMessage(Message.Message inMessage)
    {
      try
      {
        SmsFeedData smsFeedData = new SmsFeedData();
        Iso8583ATMData data = inMessage.ProcessorData as Iso8583ATMData;
        decimal tempAmount = 0;
        int tempCurrencyNumber = 0;
        string tempText;

        //  - 30/11/2012
        try
        {
          smsFeedData.sysEndPoint = string.Format("{0}:{1}",
              inMessage.Info.IncomingEndpoints.LocalEndpoint.Address,
              inMessage.Info.IncomingEndpoints.LocalEndpoint.Port);
        }
        catch
        {
          smsFeedData.sysEndPoint = "UNKNOWN";
        }

        //  - 09/01/2013
        if (_feedSourceIdDictionary.ContainsKey(smsFeedData.sysEndPoint) == true)
        {
          smsFeedData.SmsFeedSourceId =
              Convert.ToInt32(_feedSourceIdDictionary[smsFeedData.sysEndPoint]);
        }
        else
        {
          smsFeedData.SmsFeedSourceId = 1;
        }

        smsFeedData.SmsFeedText = inMessage != null ? inMessage.ToString() : null;
        smsFeedData.MessageTypeIndicator = data.TransactionType;
        smsFeedData.ResponseCode = data.Iso8583Msg[39] != null ? data.Iso8583Msg[39].ToString() : null;
        smsFeedData.ProcessingCode = data.Iso8583Msg[3] != null ? data.Iso8583Msg[3].ToString() : null;
        smsFeedData.CardNumber = data.Iso8583Msg[35] != null ? (data.Iso8583Msg[35].ToString().Split('=')[0]) : null;

        //  - 06/02/2014
        if ((data.Iso8583Msg[49] != null) &&
            Int32.TryParse(data.Iso8583Msg[49].ToString(), out tempCurrencyNumber) &&
            (data.Iso8583Msg[4] != null) &&
            Decimal.TryParse(data.Iso8583Msg[4].ToString(), out tempAmount))
        {
          smsFeedData.MerchantCurrency = data.Iso8583Msg[49] != null ? data.Iso8583Msg[49].ToString() : null;
          smsFeedData.MerchantRequestAmount = tempAmount / CurrencyHelper.GetCurrencyDivisor(tempCurrencyNumber);
        }
        else
        {
          smsFeedData.MerchantRequestAmount = -1;
        }
        
        smsFeedData.MerchantRequestDate = data.Iso8583Msg[13] != null ? data.Iso8583Msg[13].ToString() : null;
        smsFeedData.MerchantRequestTime = data.Iso8583Msg[12] != null ? data.Iso8583Msg[12].ToString() : null;
        smsFeedData.MerchantName = getSafeSubstring(data.Iso8583Msg[43], 0, 22);
        smsFeedData.MerchantCity = getSafeSubstring(data.Iso8583Msg[43], 22, 13);
        smsFeedData.MerchantCountry = getSafeSubstring(data.Iso8583Msg[43], 38, 2);

        smsFeedData.TerminalId = data.Iso8583Msg[41] != null ? data.Iso8583Msg[41].ToString() : null;
        smsFeedData.TerminalOwner = getSafeSubstring(data.Iso8583Msg[60], 0, 4);
        smsFeedData.CardIssuer = getSafeSubstring(data.Iso8583Msg[61], 0, 4);
        smsFeedData.Stan = data.Iso8583Msg[11] != null ? data.Iso8583Msg[11].ToString() : null;
        smsFeedData.AcquirerCode = data.Iso8583Msg[32] != null ? data.Iso8583Msg[32].ToString() : null; //N11
        smsFeedData.IssuerCode = data.Iso8583Msg[100] != null ? data.Iso8583Msg[100].ToString() : null; //N11
        smsFeedData.AuthorizationId = data.Iso8583Msg[38] != null ? data.Iso8583Msg[38].ToString() : null;
        smsFeedData.RetrievalReferenceNumber = data.Iso8583Msg.RRN;

        //  - 06/02/2014
        if ((data.Iso8583Msg[51] != null) &&
            Int32.TryParse(data.Iso8583Msg[51].ToString(), out tempCurrencyNumber) &&
            (data.Iso8583Msg[6] != null) &&
            Decimal.TryParse(data.Iso8583Msg[6].ToString(), out tempAmount))
        {
          smsFeedData.ProductCurrency = data.Iso8583Msg[51].ToString();
          smsFeedData.ProductAmount = tempAmount / CurrencyHelper.GetCurrencyDivisor(tempCurrencyNumber);
        }
        else
        {
          smsFeedData.ProductCurrency = smsFeedData.MerchantCurrency;
          smsFeedData.ProductAmount = smsFeedData.MerchantRequestAmount;
        }

        //  - 06/02/2014
        tempText = getSafeSubstring(data.Iso8583Msg[61], 8, 1).Trim();
        smsFeedData.RecurringIndicator = string.IsNullOrWhiteSpace(tempText) ? null : tempText;

        smsFeedData.sysStatusCode = (int)SmsStatusCode.Prepare;
        smsFeedData.sysStatusCount = 0;

        // Ignore these values for now...
        smsFeedData.sysInsertDate = DateTime.MinValue;
        smsFeedData.sysStatusDate = DateTime.MinValue;
        smsFeedData.sysNextUpdateDate = DateTime.MinValue;

        if (log.IsDebugEnabled)
        {
          log.DebugFormat("$3: RECORDING smsFeedData: {0}", smsFeedData.ToDebugString());
        }

        using (SmsFeedDataQueue smsFeedDataQueue = new SmsFeedDataQueue(_connectionString))
        {
          smsFeedDataQueue.InsertItem(smsFeedData);
        }
      }
      catch (Exception ex)
      {
        if (log.IsErrorEnabled)
        {
          log.Error("Exception Parsing and Recording 8583 Advice Message", ex);
        }
        throw;
      }
    }


    private string getSafeSubstring(object data, int start, int length)
    {
      string dataString = data as string;
      string result = default(string);

      if (string.IsNullOrWhiteSpace(dataString) == false)
      {
        int actualLength = Math.Min(length, dataString.Length - start);

        if (actualLength > 0)
        {
          result = dataString.Substring(start, actualLength);
        }
      }

      return result;
    }


    #endregion
  }
}
