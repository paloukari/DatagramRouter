using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HsmLibrary
{
  public static class HsmMsgParser
  {
    public static HsmMsg Parse(string message)
    {
      HsmMsg msg = new HsmMsg(message);

      switch (msg.MessageType)
      {
        case "CY":
          {
            return new HsmMsg_CYRequest(msg.Header, msg.Body);
          } 
        case "CZ":
          {
            return new HsmMsg_CYResponse(msg.Header, msg.Body);
          } 
        case "NO":
          {
            return new HsmMsg_NORequest(msg.Header, msg.Body);
          } 
        case "NP":
          {
            return new HsmMsg_NOResponse(msg.Header, msg.Body);
          } 
        default:
          return null;          
      }
    }
  }
}
