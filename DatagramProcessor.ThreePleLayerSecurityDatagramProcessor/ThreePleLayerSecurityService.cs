using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Corp.RouterService.Message.DatagramProcessor;
using Corp.RouterService.Message;
using System.Threading.Tasks;
using System.Threading;
using DatagramProcessor.ThreePleLayerSecurityDatagramProcessor;
using ThreePleLayerSecurityLibrary;

namespace Corp.RouterService.Adapter.WcfAdapter
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
  public class ThreePleLayerSecurityService : IServiceType, AntigonisTypes.IThreePleLayerSecurityServer
  {
    static IServiceTypeDelegates _delegates;
    static ThreePleLayerSecurityDatagramProcessor _DatagramProcessor = null;
    static private Dictionary<string, Dictionary<string, string>> _appSettings;
    static string _serverName = null;

    static ThreePleLayerSecurityService()
    {
      //for logging if someone forgets..
      _delegates = new ThreePleLayerSecurityServiceDelegates();
      _DatagramProcessor = new ThreePleLayerSecurityDatagramProcessor();
      _appSettings = new Dictionary<string, Dictionary<string, string>>();
      _serverName = "";
    }

    Message.Message _reponse = null;
    Message.Message _request = null;

    AntigonisTypes.ThreePleLayerSecurity.ThreePleLayerSecurityMessageRequest _ThreePleLayerSecurityMessageRequest = null;

    public ThreePleLayerSecurityService()
    {
    }

    public ThreePleLayerSecurityService(string serverName, Dictionary<string, Dictionary<string, string>> appSettings)
    {
      _serverName = serverName;
      _appSettings = appSettings;

      _DatagramProcessor = new ThreePleLayerSecurityDatagramProcessor();
    }

    public Type GetServiceType()
    {
      return typeof(ThreePleLayerSecurityService);
    }

    IServiceTypeDelegates IServiceType.ServiceTypeDelegates
    {
      get
      {
        return _delegates;
      }
      set
      {
        _delegates = value;
      }
    }   

    public IAsyncResult BeginPing(AntigonisTypes.ThreePleLayerSecurity.ThreePleLayerSecurityMessageRequest request, AsyncCallback callback, object state)
    {
      ServiceAsyncResult asyncResult = new ServiceAsyncResult(state, request);
      
      asyncResult.IsCompleted = true;

      callback(asyncResult);

      return asyncResult;    
    }

    public AntigonisTypes.ThreePleLayerSecurity.ThreePleLayerSecurityMessageResponse EndPing(IAsyncResult result)
    {
      var res =
          new AntigonisTypes.ThreePleLayerSecurity.ThreePleLayerSecurityMessageResponse();

      res.DisassembleFullMessageText();

      res.IsSuccess = true;

      return res;
    }

    public IAsyncResult BeginProcessThreePleLayerSecurityRequest(AntigonisTypes.ThreePleLayerSecurity.ThreePleLayerSecurityMessageRequest request, AsyncCallback callback, object state)
    {
      request.DisassembleFullMessageText();

      Message.Message requestMessage = ConstructThreePleLayerSecurityRequestMessage(request);

      ServiceAsyncResult asyncResult = new ServiceAsyncResult(state, request);

      _request = requestMessage;

      _ThreePleLayerSecurityMessageRequest = request;

      _delegates.HandleMessageDelegate(requestMessage, (response) =>
      {
        _reponse = response;

        asyncResult.IsCompleted = true;

        callback(asyncResult);
      });

      return asyncResult;
    }

    public AntigonisTypes.ThreePleLayerSecurity.ThreePleLayerSecurityMessageResponse EndProcessThreePleLayerSecurityRequest(IAsyncResult result)
    {
      return ConstructThreePleLayerSecurityMessageResponse(ref _reponse);
    }




    private Message.Message ConstructThreePleLayerSecurityRequestMessage(AntigonisTypes.ThreePleLayerSecurity.ThreePleLayerSecurityMessageRequest request)
    {
      try
      {
        ThreePleLayerSecurityMsg requestMsg = new ThreePleLayerSecurityMsgRequest(request.MessageHeader, request.MessageBody);

        Message.Message requestMessage = new Message.Message(MessageType.ThreePleLayerSecurity,
          requestMsg.ToString(),
          new MessageEndpoints()
          {
            LocalEndpoint = new MessageEndpoint(new Uri("wcf://" + _serverName))
          });

        _DatagramProcessor.PreprocessMessage(ref requestMessage);

        return requestMessage;
      }
      catch (Exception ex)
      {
        throw new FaultException<Exception>(ex);
      }
    }

    private AntigonisTypes.ThreePleLayerSecurity.ThreePleLayerSecurityMessageResponse ConstructThreePleLayerSecurityMessageResponse(ref Message.Message response)
    {
      try
      {
        if (response == null)
          throw new Exception("No response!!");

        _DatagramProcessor.PreprocessMessage(ref response);

        var ThreePleLayerSecurityData = response.ProcessorData as ThreePleLayerSecurityData;

        AntigonisTypes.ThreePleLayerSecurity.ThreePleLayerSecurityMessageResponse result = 
          new AntigonisTypes.ThreePleLayerSecurity.ThreePleLayerSecurityMessageResponse(ThreePleLayerSecurityData.ThreePleLayerSecurityMsg.ToString());

        result.DisassembleFullMessageText();

        result.IsSuccess = true;

        return result;

      }
      catch (Exception ex)
      {
        throw new FaultException<Exception>(ex);
      }
    }
  }
}