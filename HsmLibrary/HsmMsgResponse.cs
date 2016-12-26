using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using AntigonisTypes;

namespace HsmLibrary
{
  public class HsmMsgResponse : HsmMsg
  {
    private static readonly LoggingLibrary.Log4Net.ILog Log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
       System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    public static string MessageCode { get; set; }


     public HsmMsgResponse(string messageHeader, string commandBody)
      : base(messageHeader + commandBody)
    {
    }

     public HsmMsgResponse(string commandBody)
      : base(HeaderGenerator.GenerateHeader() + commandBody)
    {
    }
   
    public string ResponseCode
    {
      get
      {

        return _message.Substring(HeaderLength, 2);
      }
    }

    public string ErrorCode
    {
      get
      {
        return _message.Substring(HeaderLength + ResponseCode.Length, 2);        
      }
    }

    public HsmError HsmErrorCode
    {
      get
      {        
        string errorString = _message.Substring(HeaderLength + ResponseCode.Length, 2);
        HsmError error = HsmError.GeneralError;

        if (String.IsNullOrEmpty(errorString))
          error = HsmError.InternalError;

        if (!Enum.TryParse<HsmError>(errorString, out error))
        {
          if(Log.IsWarnEnabled)
          {
            Log.Warn(errorString + " not expected as an HSM error..");
          }
        }
        return error;
      }
    }
  }
}
