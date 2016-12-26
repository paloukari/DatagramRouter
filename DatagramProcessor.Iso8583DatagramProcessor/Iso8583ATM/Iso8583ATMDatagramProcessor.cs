using System;

namespace Corp.RouterService.Message.DatagramProcessor
{
  public class Iso8583ATMDatagramProcessor : DatagramProcessor
    {
        public override void PreprocessMessage(ref Message inMessage)
        {
            if (inMessage.ProcessorData == null)
            {
              var isodata = new Iso8583ATMData(inMessage.Payload);
                inMessage.ProcessorData = isodata;
            }
        }

        public override Message ProcessDiagnostic(Message inMessage)
        {
          string diagnosticResponseIso = Iso8583ATMData.GenerateNetworkManagementMessageResponse(inMessage);

          Message response = new Message(MessageType.Iso8583ATM,
                diagnosticResponseIso, 
                inMessage.Info.OutgoingEndpoints,
                inMessage.HeaderText,
                inMessage.HeaderCopyData);
            PreprocessMessage(ref response);

            return response;
        }        
    }
}
