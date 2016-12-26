using System;

namespace Corp.RouterService.Message
{
  public class RouterServicePredicate
  {

    Uri _originLocal;
    Uri _originRemote;
    Uri _destination;

    bool _includeDiagnostics;

    public Uri Destination
    {
      get { return _destination; }
    }
    MessageType _type;

    private RouterServicePredicate()
    {

    }

    public RouterServicePredicate(MessageType messageType, string originLocalUri, string originRemoteUri, string destinationUri, bool includeDiagnostics)
    {
      if (!string.IsNullOrEmpty(originLocalUri))
        _originLocal = new Uri(originLocalUri);
      if (!string.IsNullOrEmpty(originRemoteUri))
        _originRemote = new Uri(originRemoteUri);


      _destination = new Uri(destinationUri);
      _type = messageType;

      _includeDiagnostics = includeDiagnostics;
    }

    public bool CanHandleMessage(Message message)
    {
      bool foundMatch = false;
      bool foundMismatch = false;

      if (message.Type == _type)
      {
        foundMatch = true;
      }
      else
        return false;

      if (message.ProcessorData.IsDiagnostic && !_includeDiagnostics)
        foundMismatch = true; 

      if (_originLocal != null)
      {
        CompareUriWithEndpoint(ref foundMatch, ref foundMismatch, _originLocal, message.Info.IncomingEndpoints.LocalEndpoint);
      }
      if (_originRemote != null)
      {
        CompareUriWithEndpoint(ref foundMatch, ref foundMismatch, _originRemote, message.Info.IncomingEndpoints.LocalEndpoint);
      }

      return foundMatch && !foundMismatch;
    }

    private void CompareUriWithEndpoint(ref bool foundMatch, ref bool foundMismatch, Uri uri, MessageEndpoint messageLocalOrigin)
    {          
        if (uri.Scheme.ToString() == messageLocalOrigin.EndpointType.ToString()
          && uri.Port == messageLocalOrigin.Port
          && uri.Host == messageLocalOrigin.Address)
      {
        foundMatch = true;
      }
      else
      {
        foundMismatch = true;
      }
    }
  }
}
