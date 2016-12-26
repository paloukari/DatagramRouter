using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corp.TestTcpClient
{
    public static class MessageGenerator 
    {
        static IMessageGenerator Iso8583MessageGenerator = null;
        static IMessageGenerator IsoInternalMessageGenerator = null;
        static IMessageGenerator OperationMessageGenerator = null;
        static IMessageGenerator PosMessageGenerator = null;
        
        
        public static byte[] GetTransaction(MessageType type)
        {
            switch (type)
            {              
                case MessageType.ISOInternal:
                    if (IsoInternalMessageGenerator == null)
                        IsoInternalMessageGenerator = new IsoInternalMessageGenerator();
                    return IsoInternalMessageGenerator.GenerateTransactionMessage();
                case MessageType.ISO8583:
                    if (Iso8583MessageGenerator == null)
                        Iso8583MessageGenerator = new Iso8583MessageGenerator();
                    return Iso8583MessageGenerator.GenerateTransactionMessage();
                case MessageType.Operation:
                    if (OperationMessageGenerator == null)
                        OperationMessageGenerator = new OperationMessageGenerator();
                    return OperationMessageGenerator.GenerateTransactionMessage();
                
                    case MessageType.POS:
                    if (PosMessageGenerator == null)
                        PosMessageGenerator = new PosMessageGenerator();
                    return PosMessageGenerator.GenerateTransactionMessage();
                
                case MessageType.All:
                    if (OperationMessageGenerator == null)
                        OperationMessageGenerator = new OperationMessageGenerator();
                    if (Iso8583MessageGenerator == null)
                        Iso8583MessageGenerator = new Iso8583MessageGenerator();
                    if (IsoInternalMessageGenerator == null)
                        IsoInternalMessageGenerator = new IsoInternalMessageGenerator();
                    return OperationMessageGenerator.GenerateTransactionMessage().Concat(Iso8583MessageGenerator.GenerateTransactionMessage().Concat(IsoInternalMessageGenerator.GenerateTransactionMessage())).ToArray();   
                default:
                    break;
            }
            return null;
        }
    }
}
