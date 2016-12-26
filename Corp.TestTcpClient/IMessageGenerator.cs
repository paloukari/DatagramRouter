using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corp.TestTcpClient
{
    public interface IMessageGenerator
    {
        byte[] GenerateTransactionMessage();
        byte[] GenerateDiagnosticMessage();
        byte[] GenerateEchoMessage();
        byte[] GenerateWrongMessage();
    }
}
