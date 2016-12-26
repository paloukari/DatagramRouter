using System;
using System.Collections.Generic;
using DatabaseHelperLibrary;
using System.Configuration;
using System.Threading.Tasks;


namespace Corp.RouterService.Message.DatagramProcessor
{

  public class ItpDatagramProcessor : DatagramProcessor
  {
    public const string Plural = "Plural";
    public const string Antigonis = "Antigonis";
    public const string Wolf = "Wolf";

    public const string CSQL_POSMESSAGE_INSERT =
@"
INSERT INTO PosMessage
(
  ServiceName,
  RequestTime, 
  RequestPayload, 
  ResponseTime, 
  ResponsePayload 
)
VALUES
(
  @ServiceName,
  @RequestTime, 
  @RequestPayload, 
  @ResponseTime, 
  @ResponsePayload
)
";

    private static LoggingLibrary.Log4Net.ILog log = null;
    private static string _coralConnection = null;

    private static bool _wolfServiceActive = false;
    private static bool _pluralServiceActive = true;
    private static bool _AntigonisServiceActive = false;
    private static string _responseService = Plural;

    
    static ItpDatagramProcessor()
    {
      log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
          System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

      _coralConnection = ConfigurationManager.ConnectionStrings["CoralConnection"].ConnectionString;

      _AntigonisServiceActive = bool.Parse(ConfigurationManager.AppSettings["IsAntigonisServiceActive"]);
      _pluralServiceActive = bool.Parse(ConfigurationManager.AppSettings["IsPluralServiceActive"]);
      _wolfServiceActive = bool.Parse(ConfigurationManager.AppSettings["IsWolfServiceActive"]);

      switch (ConfigurationManager.AppSettings["ResponseService"])
      {
        case Plural: { _responseService = Plural; break; }
        case Antigonis: { _responseService = Antigonis; break; }
        case Wolf: { _responseService = Wolf; break; }
      }
    }
    
    
    public override void PreprocessMessage(ref Message inMessage)
    {      
      string unzippedPayload = Lzss.LZSS.C_Decompress(inMessage.Payload);
      ItpData itpdata = new ItpData(inMessage.HeaderText + unzippedPayload);
      inMessage.ProcessorData = itpdata;
    }

    
    public override Message ProcessMessage(Message inMessage)
    {
      string warning = "";
      if (log.IsWarnEnabled)
      {

        if (inMessage == null)
        {
          warning = "Received message to Process is NULL";
          log.Warn(warning);
          return LogAndReturn(_responseService, inMessage, null, warning);
        }

        if (inMessage.ProcessorData as ItpData == null)
        {
          warning = "Received message to Process with ItpData is NULL";
          log.Warn(warning);
          return LogAndReturn(_responseService, inMessage, null, warning);
        }

        if (inMessage.HeaderCopyData == null || inMessage.HeaderCopyData.Length != 4)
        {
          log.Warn("Received message to Process with HeaderCopyData is NULL or HeaderCopyData.Length != 8");
          log.Warn(warning);
          return LogAndReturn(_responseService, inMessage, null, warning);
        }
      }

      var payload = inMessage.ProcessorData as ItpData;

      switch (_responseService)
      {
        case Plural:
          {
            if (_AntigonisServiceActive)
            {
              Task.Factory.StartNew(() =>
              {
                CallAntigonisService(inMessage, payload);
              }).ContinueWith(t =>
              {
                var aggException = t.Exception.Flatten();
                foreach (var exception in aggException.InnerExceptions)
                  log.Error(exception);
              },
              TaskContinuationOptions.OnlyOnFaulted);
            }

            if (_wolfServiceActive)
            {
              Task.Factory.StartNew(() =>
              {
                CallWolfService(inMessage, payload);
              }).ContinueWith(t =>
              {
                var aggException = t.Exception.Flatten();
                foreach (var exception in aggException.InnerExceptions)
                  log.Error(exception);
              },
              TaskContinuationOptions.OnlyOnFaulted);
            }

            return CallPluralService(inMessage, payload);
          }

        case Antigonis:
          {
            if (_pluralServiceActive)
            {
              Task.Factory.StartNew(() =>
              {
                CallPluralService(inMessage, payload);
              }).ContinueWith(t =>
              {
                var aggException = t.Exception.Flatten();
                foreach (var exception in aggException.InnerExceptions)
                  log.Error(exception);
              },
              TaskContinuationOptions.OnlyOnFaulted);
            }
            if (_wolfServiceActive)
            {
              Task.Factory.StartNew(() =>
              {
                CallWolfService(inMessage, payload);
              }).ContinueWith(t =>
              {
                var aggException = t.Exception.Flatten();
                foreach (var exception in aggException.InnerExceptions)
                  log.Error(exception);
              },
              TaskContinuationOptions.OnlyOnFaulted);
            }

            return CallAntigonisService(inMessage, payload);
          }

        case Wolf:
          {
            if (_AntigonisServiceActive)
            {
              Task.Factory.StartNew(() =>
              {
                CallAntigonisService(inMessage, payload);
              }).ContinueWith(t =>
              {
                var aggException = t.Exception.Flatten();
                foreach (var exception in aggException.InnerExceptions)
                  log.Error(exception);
              },
              TaskContinuationOptions.OnlyOnFaulted);
            }
            if (_pluralServiceActive)
            {
              Task.Factory.StartNew(() =>
              {
                CallPluralService(inMessage, payload);
              }).ContinueWith(t =>
              {
                var aggException = t.Exception.Flatten();
                foreach (var exception in aggException.InnerExceptions)
                  log.Error(exception);
              },
              TaskContinuationOptions.OnlyOnFaulted);
            }

            return CallWolfService(inMessage, payload);
          }
        default:
          log.Error("Unexpected case value!!");
          return null;
      }
    }

    
    private Message CallPluralService(Message inMessage, ItpData payload)
    {
      string serviceName = Plural;
      string warning = "";

      PluralServices.CoralLoyaltyClient client = null;
      try
      {
        byte[] headerCopyBytes = new byte[4];
        Array.Copy(inMessage.HeaderCopyData, 0, headerCopyBytes, 2, 2);
        Array.Copy(inMessage.HeaderCopyData, 2, headerCopyBytes, 0, 2);

        PluralServices.MessageResponse response = null;

        if (payload.IsLogon || payload.IsLogoff)
        {
          return LogAndReturn(serviceName, inMessage, new Message(MessageType.Itp,
               inMessage.Payload,
               inMessage.Info.OutgoingEndpoints,
               inMessage.HeaderText,
               headerCopyBytes,
               inMessage.HeaderSuffix));
        }
        client = new PluralServices.CoralLoyaltyClient();
        response = client.RequestDispatcher(new PluralServices.MessageRequest() { Message = payload.ItpMsg.ToString() });
        client.Close();
        if (response != null && !string.IsNullOrEmpty(response.Message))
        {
          Message outMessage = new Message(MessageType.Itp,
              response.Message.Substring(inMessage.HeaderText.Length),
              inMessage.Info.OutgoingEndpoints,
              response.Message.Substring(0, inMessage.HeaderText.Length),
              headerCopyBytes);

          return LogAndReturn(serviceName, inMessage, outMessage);
        }
        else
        {
          if (log.IsWarnEnabled)
          {
            if (response == null)
            {
              warning = "Received message from Antigonis PosReceiverResponse: NULL";
              log.Warn(warning);
            }
            if (response != null && string.IsNullOrEmpty(response.Message))
            {
              warning = "Received message from Antigonis PosReceiverResponse.PosMessage: NULL";
              log.Warn(warning);
            }
          }
          return LogAndReturn(serviceName, inMessage, null);
        }
      }
      catch (Exception ex)
      {
        if (log.IsErrorEnabled)
        {
          if (inMessage == null)
          {
            log.Error(ex.ToString());
          }
        }
        if (client != null)
          client.Abort();
        return LogAndReturn(serviceName, inMessage, null, ex.Message);
      }
    }

    
    private Message CallAntigonisService(Message inMessage, ItpData payload)
    {
      string serviceName = Antigonis;
      string warning = "";
      AntigonisServices.CoralLoyaltyClient client = null;
      
      try
      {

        byte[] headerCopyBytes = new byte[4];
        Array.Copy(inMessage.HeaderCopyData, 0, headerCopyBytes, 2, 2);
        Array.Copy(inMessage.HeaderCopyData, 2, headerCopyBytes, 0, 2);


        AntigonisServices.MessageResponse response = null;

        if (payload.IsLogon || payload.IsLogoff)
        {
          return LogAndReturn(serviceName, inMessage, new Message(MessageType.Itp,
               inMessage.Payload,
               inMessage.Info.OutgoingEndpoints,
               inMessage.HeaderText,
               headerCopyBytes,
               inMessage.HeaderSuffix));

        }
        client = new AntigonisServices.CoralLoyaltyClient();
        response = client.RequestDispatcher(new AntigonisServices.MessageRequest() { Message = payload.ItpMsg.ToString() });
        client.Close();
        if (response != null && !string.IsNullOrEmpty(response.Message))
        {
          Message outMessage = new Message(MessageType.Itp,
              response.Message.Substring(inMessage.HeaderText.Length),
              inMessage.Info.OutgoingEndpoints,
              response.Message.Substring(0, inMessage.HeaderText.Length),
              headerCopyBytes);

          return LogAndReturn(serviceName, inMessage, outMessage);
        }
        else
        {
          if (log.IsWarnEnabled)
          {
            if (response == null)
            {
              warning = "Received message from Antigonis PosReceiverResponse :null";
              log.Warn(warning);
            }
            if (response != null && string.IsNullOrEmpty(response.Message))
            {
              warning = "Received message from Antigonis PosReceiverResponse.PosMessage :null";
              log.Warn(warning);
            }
          }
          return LogAndReturn(serviceName, inMessage, null);
        }
      }
      catch (Exception ex)
      {
        if (log.IsErrorEnabled)
        {
          if (inMessage == null)
          {
            log.Error(ex.ToString());
          }
        }
        if (client != null)
          client.Abort();
        return LogAndReturn(serviceName, inMessage, null, ex.Message);
      }
    }

    
    private Message CallWolfService(Message inMessage, ItpData payload)
    {
      string serviceName = Wolf;
      string warning = "";
      WolfServices.CoralParallelPortTypeClient client = null;
      try
      {

        byte[] headerCopyBytes = new byte[4];
        Array.Copy(inMessage.HeaderCopyData, 0, headerCopyBytes, 2, 2);
        Array.Copy(inMessage.HeaderCopyData, 2, headerCopyBytes, 0, 2);

        string response = null;

        if (payload.IsLogon || payload.IsLogoff)
        {
          return LogAndReturn(serviceName, inMessage, new Message(MessageType.Itp,
               inMessage.Payload,
               inMessage.Info.OutgoingEndpoints,
               inMessage.HeaderText,
               headerCopyBytes,
               inMessage.HeaderSuffix));

        }
        client = new WolfServices.CoralParallelPortTypeClient();
        response = client.CoralRequest(payload.ItpMsg.ToString());
        client.Close();
        if (response != null && !string.IsNullOrEmpty(response))
        {
          Message outMessage = new Message(MessageType.Itp,
              response.Substring(inMessage.HeaderText.Length),
              inMessage.Info.OutgoingEndpoints,
              response.Substring(0, inMessage.HeaderText.Length),
              headerCopyBytes);

          return LogAndReturn(serviceName, inMessage, outMessage);
        }
        else
        {
          if (log.IsWarnEnabled)
          {
            if (response == null)
            {
              warning = "Received message from Antigonis PosReceiverResponse: NULL";
              log.Warn(warning);
            }
            if (response != null && string.IsNullOrEmpty(response))
            {
              warning = "Received message from Antigonis PosReceiverResponse.PosMessage: NULL";
              log.Warn(warning);
            }
          }
          return LogAndReturn(serviceName, inMessage, null);
        }
      }
      catch (Exception ex)
      {
        if (log.IsErrorEnabled)
        {
          if (inMessage == null)
          {
            log.Error(ex.ToString());
          }
        }
        if (client != null)
          client.Abort();
        return LogAndReturn(serviceName, inMessage, null, ex.Message);
      }
    }


    private Message LogAndReturn(string serviceName, Message inMessage, Message outMessage, string warning = null)
    {
      string inPayload = "ERROR:EMPTY";
      DateTime inTime = DateTime.Now;


      string outPayload = "ERROR:EMPTY";
      DateTime outTime = DateTime.Now;

      if (inMessage != null && (inMessage.ProcessorData as ItpData) != null && (inMessage.ProcessorData as ItpData).ItpMsg != null)
      {
        inPayload = (inMessage.ProcessorData as ItpData).ItpMsg.ToString();
        inTime = inMessage.Info.CreationTime;
      }
      if (outMessage != null && outMessage.Payload != null)
      {
        outPayload = outMessage.Payload;
        outTime = outMessage.Info.CreationTime;
      }
      if (warning != null)
        outPayload = "ERROR:" + warning;

      AuditServiceCommunication(serviceName,
            inPayload,
            inTime,
            outPayload,
            outTime);

      return outMessage;
    }

    
    private int AuditServiceCommunication(string serviceName, string requestPayload, DateTime requestTime, string responsePayload, DateTime responseTime)
    {
      int result = -1;

      try
      {
        // Create the parameter dictionary
        Dictionary<string, object> parameterNameValues = new Dictionary<string, object>();


        parameterNameValues["ServiceName"] = serviceName;
        parameterNameValues["RequestTime"] = requestTime;
        parameterNameValues["RequestPayload"] = requestPayload;
        parameterNameValues["ResponseTime"] = responseTime;
        parameterNameValues["ResponsePayload"] = responsePayload;

        using (DatabaseHelper databaseHelper = new DatabaseHelper(_coralConnection, true))
        {
          // Insert the Item
          result = databaseHelper.ExecuteNonQuery(
              CSQL_POSMESSAGE_INSERT,
              parameterNameValues);

          databaseHelper.Dispose();
        }

      }
      catch (Exception ex)
      {
        log.Error("Error inserting data in ItpDatagramProcessor.AuditServiceCommunication", ex);
      }

      return result;
    }


  }
}
