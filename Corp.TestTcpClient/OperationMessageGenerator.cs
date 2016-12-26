using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corp.TestTcpClient
{
    class OperationMessageGenerator : IMessageGenerator
    {
        string requestTemplate = "WEB000061REQ101|{0}|TESTDATA";
        public byte[] GenerateTransactionMessage()
        {
            string request= String.Format(requestTemplate, Guid.NewGuid().ToString());

            return Encoding.ASCII.GetBytes(request);
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
