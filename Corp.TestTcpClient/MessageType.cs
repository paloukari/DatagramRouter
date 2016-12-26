using System.ComponentModel;

namespace Corp.TestTcpClient
{
  public enum MessageType
  {
    [Description("POS Message")]
    POS,
    [Description("Operation Message")]
    Operation,
    [Description("Internal Iso Message")]
    ISOInternal,
    [Description("8583 Iso Message")]
    ISO8583,

    [Description("All Messages")]
    All

  }
}
