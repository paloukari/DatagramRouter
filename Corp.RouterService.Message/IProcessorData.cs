
namespace Corp.RouterService.Message
{
    public interface IProcessorData
    {
        string MessageID { get; }
        string RetreivalID { get; }
        string TransactionType { get; }
        bool IsDiagnostic { get; }
    }
}
