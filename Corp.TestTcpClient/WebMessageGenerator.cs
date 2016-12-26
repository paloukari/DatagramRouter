using System;
using System.Threading;
using System.Text;

namespace Corp.TestTcpClient
{
    public class WebMessageGenerator : IMessageGenerator
  {    
    private static int STAN = 0;
    private static object syncObj = new object();

    public byte[] GenerateTransactionMessage()
    {
        lock (syncObj)
        {
            Interlocked.Increment(ref STAN);
            if (STAN > 9999)
                STAN = 1;

            //int length = STAN;
            int length = 4;
            string webMsgID = String.Format("{0:0000}", STAN);
            string webMsgData = webMsgID + new String('A', length);
            string webMsg = String.Format("WEB008{0:0000}", webMsgData.Length) + webMsgData;
            return Encoding.ASCII.GetBytes(webMsg);

        }
    }

    public byte[] GenerateDiagnosticMessage()
    {
        throw new NotImplementedException();
    }

    public byte[] GenerateEchoMessage()
    {
        throw new NotImplementedException();
    }

    public byte[] GenerateWrongMessage()
    {
        throw new NotImplementedException();
    }
  }
}
