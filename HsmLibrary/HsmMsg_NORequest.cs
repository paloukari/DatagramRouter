using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HsmLibrary
{
  public class HsmMsg_NORequest : HsmMsgRequest
  {
    private const string C_MESSAGE_CODE = "NO";
    private const string C_NO_MODE_FLAG = "00";

    public static new string MessageCode { get { return C_MESSAGE_CODE; } }

    private static string CreateNOBody(string comCode, string modeFlag)
    {
      return comCode+modeFlag;

    }
    public HsmMsg_NORequest()
      : base(HeaderGenerator.GenerateHeader(), HsmMsg_NORequest.CreateNOBody(C_MESSAGE_CODE, C_NO_MODE_FLAG))
    {
    }

    public HsmMsg_NORequest(string body)
      : base(body)
    {

    }

    public HsmMsg_NORequest(string header, string body)
      : base(header, body)
    {

    }

    public string ModeFlag
    {
      get
      {
        return _message.Substring(HeaderLength + MessageTypeLength, 2);
      }
    }
  }
}
