
namespace Corp.TestTcpClient
{
    internal class TestResult
    {
        internal bool Connected { get; set; }
        internal bool SentData { get; set; }
        internal bool ServerReplied {
            get
            {
                return ServerResponse != null;              
            }
        }
        internal string ServerResponse { get; set; }

        internal int Id { get; set; }

        internal TestResult(int id)
        {
            Id = id;
            Connected = false;
            SentData = false;            
        }
    }
}
