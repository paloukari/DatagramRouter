using System;
using System.Linq;
using System.Text;

namespace Corp.TestTcpClient
{
    public class IsoInternalMessageGenerator : IMessageGenerator
    {
      private static string transactionIsoTemplate = "ISO0160000130220B23882012EE080180000004010000004311000000000000000071710545200{0}135452071707180010214374830160000102004=15051010000023710000{1}        00000055S0101072        S0101072       TEST ALPHA BANK       ATHENS          GR978012ANETDEV1+000013HSBCDEV1000000200000000003804071713545227071800000000000271042& 0000200042! B400020 021500            0 ";
        private static int STAN = 0;
        private static object syncObj = new object();
        public  byte[] GenerateTransactionMessage()
        {
            lock (syncObj)
            {
                var temp = transactionIsoTemplate;
                temp = string.Format(temp, STAN.ToString().PadLeft(4, '0'), STAN.ToString().PadLeft(4, '0'));
                STAN++;
                if (STAN == 10000)
                    STAN = 0;
                var messageLength = temp.Length + 3;

                byte msb = Convert.ToByte(messageLength / 256);
                byte lsb = Convert.ToByte(messageLength % 256);
                byte[] tmp = new byte[] { msb, lsb };

                char eom = (char)(byte)3;
                tmp = tmp.Concat(Encoding.ASCII.GetBytes(temp + eom)).ToArray();
                return tmp;
            }
        }

        public byte[] GenerateDiagnosticMessage()
        {
            lock (syncObj)
            {
                var temp = transactionIsoTemplate;
                temp = string.Format(temp, STAN.ToString().PadLeft(4, '0'), STAN.ToString().PadLeft(4, '0'));
                STAN++;
                if (STAN == 10000)
                    STAN = 0;
                var messageLength = temp.Length + 3;

                byte msb = Convert.ToByte(messageLength / 256);
                byte lsb = Convert.ToByte(messageLength % 256);
                byte[] tmp = new byte[] { msb, lsb };

                char eom = (char)(byte)3;
                tmp = tmp.Concat(Encoding.ASCII.GetBytes(temp + eom)).ToArray();
                return tmp;
            }
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
