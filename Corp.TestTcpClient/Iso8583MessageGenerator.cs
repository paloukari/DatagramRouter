using System;
using System.Threading;
using Iso8583Library;
using Iso8583Library.Formatters;
using System.Text;

namespace Corp.TestTcpClient
{
    public class Iso8583MessageGenerator : IMessageGenerator
    {
        private static string transactionIsoTemplate = "02007024058000C0800216430252012713000021300000000010000000000013020120200000000028000000000000028978084004222{{5F8146EC-727E-4BD2-9021-69526855AB76}}00223300000000000280300002000857120317";
        private static int STAN = 0;
        private static object syncObj = new object();
        public byte[] GenerateTransactionMessage()
        {
            lock (syncObj)
            {
                Iso8583Msg msg = Iso8583Msg.Parse(transactionIsoTemplate, Iso8583MsgFormatterFactory.CreateIso8583MsgFormatter(Iso8583MsgFormatterType.8583Version2010R01));
                STAN++;
                if (STAN == 10000)
                    STAN = 0;
                msg.STAN = String.Format("{0:000000}", STAN);
                msg.RRN = String.Format("{0:123412340000}", STAN);

                string asciiIso = msg.ToAscii(Iso8583MsgFormatterFactory.CreateIso8583MsgFormatter(Iso8583MsgFormatterType.8583Version2010R01));
                asciiIso = String.Format("ISO5V0{0:0000}", asciiIso.Length) + asciiIso;
                return Encoding.ASCII.GetBytes(asciiIso);
            }
        }

    

        public byte[] GenerateDiagnosticMessage()
        {
            return new byte[0];
        }

        public byte[] GenerateEchoMessage()
        {
            return new byte[0];
        }

        public byte[] GenerateWrongMessage()
        {
            return new byte[0];
        }
    }
}
