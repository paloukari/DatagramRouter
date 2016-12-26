using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HsmLibrary
{
  public class HsmMsg_CYRequest : HsmMsgRequest
  {
    private const string C_MESSAGE_CODE = "CY";
    private const int I_MESSAGE_CODE_LENGTH = 2;
    private const int I_CVK_LENGTH = 32;
    private const int I_CVV_LENGTH = 3;

    private const int I_DELIMITER_LENGTH = 1;

    private const int I_EXPIRATION_DATE_LENGTH = 4;
    private const int I_SERVICE_CODE_LENGTH = 3;

    public static new string MessageCode { get { return C_MESSAGE_CODE; } }



    private static string CreateCYBody(string cvk,
      string cvv,
      string primaryAccoundNumber,
      string expirationDate,
      string serviceCode)
    {
      //Debug.Assert(serviceCode.Length == I_SERVICE_CODE_LENGTH);
      //Debug.Assert(expirationDate.Length == I_EXPIRATION_DATE_LENGTH);
      //Debug.Assert(cvv.Length == I_CVV_LENGTH);
      //Debug.Assert(cvk.Length == I_CVK_LENGTH);

      return C_MESSAGE_CODE + cvk + cvv + primaryAccoundNumber + ";" + expirationDate + serviceCode;
    }

    public HsmMsg_CYRequest(string messageHeader, string messageBody)
      : base(messageHeader, messageBody)
    {
    }
    public HsmMsg_CYRequest(string cvk,
      string cvv,
      string primaryAccoundNumber,
      string expirationDate,
      string serviceCode)
      : base(HeaderGenerator.GenerateHeader(), HsmMsg_CYRequest.CreateCYBody(cvk, cvv, primaryAccoundNumber, expirationDate, serviceCode))
    {
      Debug.Assert(MessageType == C_MESSAGE_CODE);
    }

    public string CVK
    {
      get
      {
        return _message.Substring(HeaderLength + I_MESSAGE_CODE_LENGTH, I_CVK_LENGTH);
      }
    }

    public string CVV
    {
      get
      {
        return _message.Substring(HeaderLength + I_MESSAGE_CODE_LENGTH + I_CVK_LENGTH, I_CVV_LENGTH);
      }
    }

    public string PrimaryAccoundNumber
    {
      get
      {
        return _message.Substring(HeaderLength + I_MESSAGE_CODE_LENGTH + I_CVK_LENGTH + I_CVV_LENGTH, _message.IndexOf(';', (HeaderLength + I_MESSAGE_CODE_LENGTH + I_CVK_LENGTH + I_CVV_LENGTH)) - (HeaderLength + I_MESSAGE_CODE_LENGTH + I_CVK_LENGTH + I_CVV_LENGTH));
      }
    }

    public string Delimiter
    {
      get
      {
        return _message.Substring(HeaderLength + I_MESSAGE_CODE_LENGTH + I_CVK_LENGTH + I_CVV_LENGTH + PrimaryAccoundNumber.Length, I_DELIMITER_LENGTH);
      }
    }

    public string ExpirationDate
    {
      get
      {
        return _message.Substring(HeaderLength + I_MESSAGE_CODE_LENGTH + I_CVK_LENGTH + I_CVV_LENGTH + PrimaryAccoundNumber.Length + I_DELIMITER_LENGTH, I_EXPIRATION_DATE_LENGTH);
      }
    }

    public string ServiceCode
    {
      get
      {
        return _message.Substring(HeaderLength + I_MESSAGE_CODE_LENGTH + I_CVK_LENGTH + I_CVV_LENGTH + PrimaryAccoundNumber.Length + I_DELIMITER_LENGTH + I_EXPIRATION_DATE_LENGTH, I_SERVICE_CODE_LENGTH);
      }
    }
  }
}
