using System;
using System.Text;
using Corp.RouterService.Common;


namespace Corp.RouterService.Message
{
  class TcpMessageHeaderDataHandler
  {
      private TcpTrafficSettings _settings;      

      internal TcpMessageHeaderDataHandler(TcpTrafficSettings settings)
    {        
      _settings = settings;
    }



    internal bool PartiallyDeserializeHeaderData(TcpMessageHeader header, TcpMessageBuffer buffer)
    {
        //we don't know the message type yet.
        //we will read the bytes and compare them with the settings until we know the message type
        while (true)
        {
            int dataToRead = Math.Min(buffer.Count, header.UnhandledData);

            if (dataToRead == 0)
                break;
            if (dataToRead > 0)
                header.HandledData += buffer.Remove(header.HeaderData,
                    header.HandledData, dataToRead);

            if (header.UnhandledData == 0)
            {
                //we have the entire header                                
                ParseHeader(header);
                return true;
            }
        }
      return false;
    }

    private void ParseHeader(TcpMessageHeader header)
    {
      try
      {
          MessageType type = header.Type;

          var messageSettings = header.MessageSettings;
          
          //double check
          if (!messageSettings.ValidateHeader(header.HeaderData))
          {
            throw new Exception("Header is not valid: " + Encoding.GetEncoding(1253).GetString(header.HeaderData));
          }

          header.BodyLength = messageSettings.CalculateBodyLengthFromHeader(header.HeaderData);      
      }
      catch (Exception ex)
      {
        //Debug.WriteLine(message.TokenId + ":ERROR:" + header);
        throw ex;
      }
    }




    internal bool PartiallySerializeHeaderData(TcpMessageHeader header, TcpMessageBuffer buffer)
    {
      int dataToWrite = Math.Min(buffer.FreeCount, header.UnhandledData);

      if (dataToWrite > 0)
        header.HandledData += buffer.Add(header.HeaderData,
            header.HandledData, dataToWrite);

      if (header.UnhandledData == 0)
      {
        return true;
      }
      return false;
    }
  }
}
