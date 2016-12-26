using System;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Corp.RouterService.Connection;
using Corp.RouterService.Message;



namespace Corp.RouterService.Common
{
  internal static class ExtensionMethods
  {

    private static LoggingLibrary.Log4Net.ILog log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    #region StartSafeNew
    public static void LogExceptions(this Task task)
    {
      task.ContinueWith(t =>
      {
        var aggException = t.Exception.Flatten();
        foreach (var exception in aggException.InnerExceptions)
          LogException(exception);
      },
      TaskContinuationOptions.OnlyOnFaulted);
    }

    private static void LogException(Exception exception)
    {
      if (log.IsErrorEnabled)
      {
        log.Error("Error in Task:" + exception.ToString());
      }
    }

    internal static void StartSafeNew(this TaskFactory factory, Action action)
    {
      factory.StartNew(action).LogExceptions();
    }


    internal static void StartSafeNew(this TaskFactory factory, Action action, TaskCreationOptions creationOptions)
    {
      factory.StartNew(action, creationOptions).LogExceptions();
    }

    #endregion


    #region SocketAsyncEventArgs extensions
    //no extension properties allowed, just methods
    internal static TcpMessageToken GetMessageToken(this SocketAsyncEventArgs args)
    {
      return (TcpMessageToken)args.UserToken as TcpMessageToken;
    }

    internal static RouterServiceConnectionToken GetConnectionToken(this SocketAsyncEventArgs args)
    {
      return (RouterServiceConnectionToken)args.UserToken as RouterServiceConnectionToken;
    }

    #endregion


    internal static int CalculateBodyLengthFromHeader(this TcpMessageSettings settings, byte[] _headerData)
    {
      if (settings.BodyFixedLength > 0)
        return settings.BodyFixedLength;
      //this is token based message
      if (settings.LenghtIndicatorLength == 0)
        return 0;

      byte[] lengthIndicatorBytes = new byte[settings.LenghtIndicatorLength];

      ReadMaskedHeaderPart(settings.HeaderTemplateBytes, _headerData, ref lengthIndicatorBytes, TcpMessageTemplateMasks.HeaderLengthIndicator);

      int lengthIndicatorValue = 0;
      switch (settings.LengthIndicatorFormat)
      {
        case TcpMessageHeaderLengthIndicatorFormat.Text:
          if (!Int32.TryParse(Encoding.GetEncoding(1253).GetString(lengthIndicatorBytes), out lengthIndicatorValue))
            throw new Exception("Could not find length indicator");
          break;
        case TcpMessageHeaderLengthIndicatorFormat.Binary:
          //patience please..
          for (int i = lengthIndicatorBytes.Length - 1; i >= 0; i--)
          {
            lengthIndicatorValue += lengthIndicatorBytes[i] * (int)Math.Pow(2, (8 * (lengthIndicatorBytes.Length - (i + 1))));
          }
          break;
        default:
          throw new Exception("Unknown TcpMessageHeaderLengthIndicatorFormat" + settings.LengthIndicatorFormat);
      }

      if ((settings.LengthIndicatorType & (int)TcpMessageParts.Header) != 0)
      {
        lengthIndicatorValue -= settings.HeaderTextLength;
      }
      if ((settings.LengthIndicatorType & (int)TcpMessageParts.HeaderPrefix) != 0)
      {
        lengthIndicatorValue -= settings.HeaderPrefixLength;
      }
      if ((settings.LengthIndicatorType & (int)TcpMessageParts.HeaderSuffix) != 0)
      {
        lengthIndicatorValue -= settings.HeaderSuffixLength;
      }
      if ((settings.LengthIndicatorType & (int)TcpMessageParts.Length) != 0)
      {
        lengthIndicatorValue -= settings.LenghtIndicatorLength;
      }
      if ((settings.LengthIndicatorType & (int)TcpMessageParts.HeaderWildcards) != 0)
      {
        lengthIndicatorValue -= settings.HeaderWildcardLength;
      }
      if ((settings.LengthIndicatorType & (int)TcpMessageParts.HeaderCopyBytes) != 0)
      {
        lengthIndicatorValue -= settings.HeaderCopyBytesLength;
      }
      //add the suffix
      if ((settings.LengthIndicatorType & (int)TcpMessageParts.MessageSuffix) == 0)
      {
        lengthIndicatorValue += settings.MessageSuffixLength;
      }
      return lengthIndicatorValue;
    }

    internal static bool ValidateHeader(this TcpMessageSettings settings, byte[] _headerData)
    {
      if (settings.HeaderLength != _headerData.Length)
        return false;

      if (settings.HeaderPrefixLength > 0)
      {
        if (!CheckMaskedHeaderPart(settings.HeaderTemplateBytes, _headerData, settings.HeaderPrefix, TcpMessageTemplateMasks.HeaderPrefix))
          return false;
      }
      if (settings.HeaderTextLength > 0)
      {
        if (!CheckMaskedHeaderPart(settings.HeaderTemplateBytes, _headerData, settings.HeaderTextBytes, TcpMessageTemplateMasks.HeaderText))
          return false;
      }
      if (settings.HeaderSuffixLength > 0)
      {
        if (!CheckMaskedHeaderPart(settings.HeaderTemplateBytes, _headerData, settings.HeaderSuffix, TcpMessageTemplateMasks.HeaderSuffix))
          return false;
      }

      //spyrosg: match with with just the length is a good match 
      //if (match == false)
      //    throw new Exception("No match and no difference in header validation!");
      return true;
    }


    private static bool CheckMaskedHeaderPart(byte[] headerTemplate, byte[] header, byte[] data, TcpMessageTemplateMasks tcpMessageTemplateMasks)
    {
      int iterationDataCounter = 0;
      for (int i = 0; i < headerTemplate.Length; i++)
      {
        if (headerTemplate[i] == (byte)tcpMessageTemplateMasks)
        {
          if (header[i] != data[iterationDataCounter++])
            return false;
        }
      }
      return true;
    }

    #region Message extensions

    internal static TcpMessage ToTcpMessage(this Message.Message message, TcpTrafficSettings settings)
    {
      var type = message.Type;

      TcpMessage tcpMessage = new TcpMessage(type, settings);

      tcpMessage.MessageSettings = settings.GetMessageTypeSettings(type);

      TcpMessageSettings tcpMessageSettings = tcpMessage.MessageSettings;

      byte[] headerBytes = GenerateHeader(message, tcpMessageSettings);

      byte[] bodyBytes = GenerateBody(message, tcpMessageSettings);

      tcpMessage.Header = new TcpMessageHeader(type, headerBytes, bodyBytes.Length, settings);

      tcpMessage.Body = new TcpMessageBody(type, bodyBytes, tcpMessage.Header.MessageSettings);

      return tcpMessage;
    }

    private static byte[] GenerateBody(Message.Message message, TcpMessageSettings settings)
    {
      byte[] body = new byte[message.Payload.Length + settings.MessageSuffixLength];
      System.Buffer.BlockCopy(Encoding.GetEncoding(1253).GetBytes(message.Payload), 0, body, 0, message.Payload.Length);
      if (settings.MessageSuffixLength > 0)
        System.Buffer.BlockCopy(settings.MessageSuffix, 0, body, message.Payload.Length, settings.MessageSuffixLength);

      return body;
    }

    private static byte[] GenerateHeader(Message.Message message, TcpMessageSettings settings)
    {
      MessageType type = message.Type;

      //first calculate the Length Indicator
      byte[] lengthIndicator = CalculateLengthIndicator(message, settings);

      byte[] headerTemplate = new byte[settings.HeaderLength];
      byte[] header = new byte[settings.HeaderLength];
      Buffer.BlockCopy(settings.HeaderTemplateBytes, 0, headerTemplate, 0, settings.HeaderLength);
      Buffer.BlockCopy(settings.HeaderTemplateBytes, 0, header, 0, settings.HeaderLength);


      if (message.HeaderCopyData.Length != settings.HeaderCopyBytesLength)
        throw new Exception("Wildcards are not equal to HeaderCopyData");

      if (settings.BodyFixedLength > 0 && message.Payload.Length != settings.BodyFixedLength)
          throw new Exception("settings.BodyFixedLength are not equal to actual body size");

      //for future dev: copy message.HeaderText to settings.HeaderText
      //if (!string.IsNullOrEmpty(message.HeaderText) && message.HeaderText.Length < settings.HeaderTextLength)
      //    throw new Exception("HeaderText are not equal with settings");

      CopyMaskedHeaderPart(headerTemplate, ref header, settings.HeaderPrefix, TcpMessageTemplateMasks.HeaderPrefix);
      CopyMaskedHeaderPart(headerTemplate, ref header, settings.HeaderTextBytes, TcpMessageTemplateMasks.HeaderText);
      CopyMaskedHeaderPart(headerTemplate, ref header, settings.HeaderSuffix, TcpMessageTemplateMasks.HeaderSuffix);
      CopyMaskedHeaderPart(headerTemplate, ref header, lengthIndicator, TcpMessageTemplateMasks.HeaderLengthIndicator);
      CopyMaskedHeaderPart(headerTemplate, ref header, message.HeaderCopyData, TcpMessageTemplateMasks.HeaderCopyBytes);

      if (settings.HeaderSuffix != null && settings.HeaderSuffix.Length > 0)
      {
        message.HeaderSuffix = Encoding.GetEncoding(1253).GetString(settings.HeaderSuffix);
      }
      return header;
      /*
      switch (type)
      {
          case MessageType.IsoInternal:
              {
                  int messageLength = message.Header.Length + message.Payload.Length + settings.HeaderSuffixLength + settings.HeaderPrefixLength;
                  byte msb = Convert.ToByte(messageLength / 256);
                  byte lsb = Convert.ToByte(messageLength % 256);

                  byte[] lengthBytes = new byte[] { msb, lsb };
                  byte[] headerBytes = Encoding.GetEncoding(1253).GetBytes(message.Header);

                  return lengthBytes.Concat(headerBytes).ToArray();
              }
          case MessageType.Iso8583:
              {
                  string header = String.Format("{0}{1}{2:0000}", settings.HeaderPrefix, settings.HeaderText, message.Payload.Length);
                  return Encoding.GetEncoding(1253).GetBytes(header);
              }
          case MessageType.Web:
              return Encoding.GetEncoding(1253).GetBytes(settings.HeaderPrefix + message.Header);
          case MessageType.Unknown:
          default:
              throw new Exception("unknown message type");
      }
       */
    }

    private static void CopyMaskedHeaderPart(byte[] headerTemplate, ref byte[] header, byte[] data, TcpMessageTemplateMasks tcpMessageTemplateMasks)
    {
      if (data.Length == 0)
        return;
      int iterationDataCounter = 0;
      for (int i = 0; i < headerTemplate.Length; i++)
      {
        if (headerTemplate[i] == (byte)tcpMessageTemplateMasks)
        {
          header[i] = data[iterationDataCounter++];
        }
      }
    }



    private static byte[] CalculateLengthIndicator(Message.Message message, TcpMessageSettings settings)
    {
      if (settings.LenghtIndicatorLength == 0)
        return new byte[0];
      int lengthIndicatorValue = 0;
      if ((settings.LengthIndicatorType & (int)TcpMessageParts.Body) != 0)
      {
        lengthIndicatorValue += message.Payload.Length;
      }
      if ((settings.LengthIndicatorType & (int)TcpMessageParts.Header) != 0)
      {
        lengthIndicatorValue += settings.HeaderTextLength;
      }
      if ((settings.LengthIndicatorType & (int)TcpMessageParts.HeaderPrefix) != 0)
      {
        lengthIndicatorValue += settings.HeaderPrefixLength;
      }
      if ((settings.LengthIndicatorType & (int)TcpMessageParts.HeaderSuffix) != 0)
      {
        lengthIndicatorValue += settings.HeaderSuffixLength;
      }
      if ((settings.LengthIndicatorType & (int)TcpMessageParts.Length) != 0)
      {
        lengthIndicatorValue += settings.LenghtIndicatorLength;
      }
      if ((settings.LengthIndicatorType & (int)TcpMessageParts.HeaderWildcards) != 0)
      {
        lengthIndicatorValue += settings.HeaderWildcardLength;
      }
      if ((settings.LengthIndicatorType & (int)TcpMessageParts.HeaderCopyBytes) != 0)
      {
        lengthIndicatorValue += settings.HeaderCopyBytesLength;
      }
      if ((settings.LengthIndicatorType & (int)TcpMessageParts.MessageSuffix) != 0)
      {
        lengthIndicatorValue += settings.MessageSuffixLength;
      }

      switch (settings.LengthIndicatorFormat)
      {
        case TcpMessageHeaderLengthIndicatorFormat.Text:
          string lengthIndicator = lengthIndicatorValue.ToString().PadLeft(settings.LenghtIndicatorLength, '0');
          if (lengthIndicator.Length != settings.LenghtIndicatorLength)
            throw new Exception("Length indicator value exceeded the defined space.");
          return Encoding.GetEncoding(1253).GetBytes(lengthIndicator);

        case TcpMessageHeaderLengthIndicatorFormat.Binary:
          //zero filled
          byte[] lengthIndicatorBytes = new byte[settings.LenghtIndicatorLength];

          int loopCounter = 0;
          //patience please..
          while (lengthIndicatorValue > 0)
          {
            int mask = (int)Math.Pow(2, 8 * (settings.LenghtIndicatorLength - (loopCounter + 1)));
            int result = lengthIndicatorValue / mask;
            if (loopCounter > settings.LenghtIndicatorLength)
              throw new Exception("Length indicator value exceeded the defined space.");
            lengthIndicatorBytes[loopCounter] = Convert.ToByte(result);
            lengthIndicatorValue -= result * mask;

            ++loopCounter;
          }
          return lengthIndicatorBytes;
        default:
          throw new Exception("Unknown TcpMessageHeaderLengthIndicatorFormat" + settings.LengthIndicatorFormat);
      }
    }
    #endregion

    #region TcpMessage extensions
    internal static Message.Message ToMessage(this TcpMessage tcpMessage)
    {
      string payload = Encoding.GetEncoding(1253).GetString(tcpMessage.Body.BodyData, 0, tcpMessage.Body.BodyData.Length - tcpMessage.MessageSettings.MessageSuffixLength);

      var type = tcpMessage.Type;


      MessageEndpoints endPoints = new MessageEndpoints()
      {
        LocalEndpoint = tcpMessage.LocalEndpoint.ToMessageEndpoint(),
        RemoteEndpoind = tcpMessage.RemoteEndpoint.ToMessageEndpoint()
      };


      byte[] headerCopyBytesData = null;
      //we anly need the wildcard-copy header
      if (tcpMessage.MessageSettings.HeaderCopyBytesLength > 0)
      {
        headerCopyBytesData = new byte[tcpMessage.MessageSettings.HeaderCopyBytesLength];

        ReadMaskedHeaderPart(tcpMessage.MessageSettings.HeaderTemplateBytes, tcpMessage.Header.HeaderData, ref headerCopyBytesData, TcpMessageTemplateMasks.HeaderCopyBytes);
      }

      string headerSuffix = null;
      if (tcpMessage.MessageSettings.HeaderSuffixLength > 0)
      {
        headerSuffix = Encoding.GetEncoding(1253).GetString(tcpMessage.MessageSettings.HeaderSuffix);
      }


      Message.Message message = new Message.Message(type, payload, endPoints, tcpMessage.MessageSettings.HeaderText,  headerCopyBytesData, headerSuffix);

      DatagramProcessorFactory.GetDatagramProcessor(message).PreprocessMessage(ref message);

      return message;
    }

    private static void ReadMaskedHeaderPart(byte[] headerTemplate, byte[] header, ref byte[] data, TcpMessageTemplateMasks tcpMessageTemplateMasks)
    {
      int iterationDataCounter = 0;
      for (int i = 0; i < headerTemplate.Length; i++)
      {
        if (headerTemplate[i] == (byte)tcpMessageTemplateMasks)
        {
          data[iterationDataCounter++] = header[i];
        }
      }
    }

    internal static TcpMessageEndpoint ToTcpMessageEndpoint(this Message.MessageEndpoint endpoint)
    {
      return new TcpMessageEndpoint(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(endpoint.Address), endpoint.Port));
    }

    internal static Message.MessageEndpoint ToMessageEndpoint(this TcpMessageEndpoint tcpEndpoint)
    {
      return new MessageEndpoint() { Address = tcpEndpoint.EndpointIP, Port = tcpEndpoint.EndpointPort };
    }
    #endregion

  }
}
