using System;
using Iso8583Library;
using Iso8583Library.Formatters;

namespace Corp.RouterService.Message.DatagramProcessor
{
  internal class Iso8583Data : IProcessorData
    {
        Iso8583Msg _msg;

        internal Iso8583Msg Iso8583Msg
        {
            get { return _msg; }
            set { _msg = value; }
        }

        internal Iso8583Data(string asciiData)
        {
            _msg = Iso8583Msg.Parse(asciiData, Iso8583MsgFormatterFactory.CreateIso8583MsgFormatter(Iso8583MsgFormatterType.8583Version2010R01));          
            _msg.ParseSubfieldsOfField63();
        }

        internal static string GenerateNetworkManagementMessageResponse(string stan)
        {
          string result = default(string);

          Iso8583Msg message = new Iso8583Msg(810);
          Iso8583MsgFormatter formatter = Iso8583MsgFormatterFactory.CreateIso8583MsgFormatter(
              Iso8583Library.Formatters.Iso8583MsgFormatterType.8583Version2010R01);

          message.TransmissionDateAndTime = DateTime.Now.ToString("yyMMddhhmm");
          message.STAN = stan;
          message.NetworkManagementInformationCode = "301";

          result = message.ToAscii(formatter);

          return result;
        }

        public bool IsDiagnostic
        {
          get
          {
            return _msg == null || _msg.MessageTypeIdentifier == 800 || _msg.MessageTypeIdentifier == 810;// && _msg.NetworkManagementInformationCode=="301";
          }         
        }

        internal static string GenerateNetworkManagementMessageResponse(Message inMessage)
        {

          var inData=inMessage.ProcessorData as Iso8583Data;
          string result = default(string);

          Iso8583Msg message = new Iso8583Msg(810);
          Iso8583MsgFormatter formatter = Iso8583MsgFormatterFactory.CreateIso8583MsgFormatter(
              Iso8583Library.Formatters.Iso8583MsgFormatterType.8583Version2010R01);

          message.TransmissionDateAndTime = DateTime.Now.ToString("yyMMddhhmm");
          message.STAN = inData.Iso8583Msg.STAN;
          message.NetworkManagementInformationCode = inData.Iso8583Msg.NetworkManagementInformationCode;

          result = message.ToAscii(formatter);

          return result;
        }

        public string TransactionType
        {
          get { return _msg.MessageTypeIdentifier.ToString(); }
        }

        public string MessageID
        {
          get { return _msg.STAN; }
        }

        public string RetreivalID
        {            
            get { return _msg.RRN; }
        }
    }
}
