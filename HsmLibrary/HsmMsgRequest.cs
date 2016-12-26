using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HsmLibrary
{
  public class HsmMsgRequest : HsmMsg
  {
    public HsmMsgRequest(string messageHeader, string commandBody)
      : base(messageHeader + commandBody)
    {
    }

    public HsmMsgRequest(string commandBody)
      : base(HeaderGenerator.GenerateHeader() + commandBody)
    {
    }

    public static string MessageCode { get; set; }
  }
}
