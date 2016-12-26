
namespace Corp.RouterService.TcpServer
{
    internal enum TcpServerState
    {
        Uninitialized, 
        Initializing,
        Initialized, 
        Starting,
        Started, 
        Stopping, 
        Stopped, 
        Faulted
    }
}
