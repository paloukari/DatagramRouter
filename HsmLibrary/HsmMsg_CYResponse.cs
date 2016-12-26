using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HsmLibrary
{
  public class HsmMsg_CYResponse : HsmMsgResponse
  {
    private const string C_MESSAGE_CODE = "CZ";

    public static new string MessageCode { get { return C_MESSAGE_CODE; } }

    public HsmMsg_CYResponse(string body)
      : base(body)
    {
      Debug.Assert(ResponseCode == C_MESSAGE_CODE);
    }


    public HsmMsg_CYResponse(string header, string body)
      : base(header, body)
    {
      Debug.Assert(ResponseCode == C_MESSAGE_CODE);
    }
  }
}
