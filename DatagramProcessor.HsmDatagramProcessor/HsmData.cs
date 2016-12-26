using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corp.RouterService.Message;
using HsmLibrary;

namespace Corp.RouterService.Message.DatagramProcessor
{
  internal class HsmData : IProcessorData
  {
    HsmMsg _msg;

    public HsmData(string asciiData)
    {
      _msg = HsmMsgParser.Parse(asciiData);
    }

    internal HsmMsg HsmMsg
    {
      get { return _msg; }
      set { _msg = value; }
    }

    public string MessageID
    {
      get { return _msg.Header; }
    }

    public string RetreivalID
    {
      get { return _msg.Header; }
    }

    public string TransactionType
    {
      get { return _msg.MessageType; }
    }

    public bool IsDiagnostic
    {
      get { return _msg == null || _msg.MessageType == "NO" || _msg.MessageType == "NP"; }
    }
  }
}
