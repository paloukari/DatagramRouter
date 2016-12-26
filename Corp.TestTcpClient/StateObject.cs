using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Corp.TestTcpClient
{
    // State object for receiving data from remote device.
    internal class StateObject
    {
        internal ManualResetEvent ConnectDone =
            new ManualResetEvent(false);
        internal ManualResetEvent SendDone =
            new ManualResetEvent(false);
        internal ManualResetEvent ReceiveDone =
            new ManualResetEvent(false);

        internal MessageType messageType;

        internal int SendDataLength;
        internal int messagesSent;
        // Client socket.
        internal Socket workSocket = null;
        // Size of receive buffer.
        internal const int BufferSize = 1024;
        // Receive buffer.
        internal byte[] buffer = new byte[BufferSize];
        // Received data string.
        internal StringBuilder rec = new StringBuilder("");

        internal StringBuilder send = new StringBuilder("");

        internal int receivedCounter = 0;

        internal bool ReadyToCloseSocket = false;

    }

}
