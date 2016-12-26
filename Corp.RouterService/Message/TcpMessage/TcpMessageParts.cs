
namespace Corp.RouterService.Message
{
    public enum TcpMessageParts : int
    {
        HeaderPrefix = 1,
        Header = 2,
        HeaderWildcards = 4,
        HeaderCopyBytes = 8,
        HeaderSuffix = 16,
        Length = 32,
        Body = 64,
        MessageSuffix = 128
    }
}
