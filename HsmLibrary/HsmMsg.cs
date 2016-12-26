using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HsmLibrary
{
  public class HsmMsg
  {
    protected string _message;

    public HsmMsg(string message)
    {
      _message = message;
    }

    protected int HeaderLength
    {
      get
      {
        return HeaderGenerator.HeaderLength;
      }
    }

    public string Header
    {
      get
      {
        if (HeaderLength == 0)
          return String.Empty;
        return _message.Substring(0, HeaderLength);
      }
    }

    public string Body
    {
      get
      {
        return _message.Substring(HeaderLength, _message.Length - HeaderLength);
      }
    }

    protected int MessageTypeLength
    {
      get
      {
        return 2;
      }
    }

    public string MessageType
    {
      get
      {
        return _message.Substring(HeaderLength, MessageTypeLength);
      }
    }

    public override string ToString()
    {
      return _message;
    }
  }
}
