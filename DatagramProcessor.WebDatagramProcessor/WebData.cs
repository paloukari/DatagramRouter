using System;


namespace Corp.RouterService.Message.DatagramProcessor
{
    internal class WebData : IProcessorData
    {
      string _msg;
      string _id;

        internal string Web123Msg
        {
          get { return _msg; }
          set { _msg = value; }
        }

        internal string Web123Id
        {
          get { return _id; }
          set { _id = value; }
        }

        internal WebData(string asciiData)
        {
          _id = asciiData.Substring(0, 4);
          _msg = asciiData;
        }

        public bool IsDiagnostic
        {
          get { return false; }
        }

        public string TransactionType
        {
          get { throw new NotImplementedException(); }
        }

        public string MessageID
        {
          get { return _id; }
        }

        public string RetreivalID
        {
            get { return _id; } 
        }
    }
}
