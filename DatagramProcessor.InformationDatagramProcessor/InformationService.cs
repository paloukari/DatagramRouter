using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Corp.RouterService.Message.DatagramProcessor;
using Corp.RouterService.Message;
using System.Threading.Tasks;
using System.Threading;
using DatagramProcessor.InformationDatagramProcessor;
using InformationLibrary;

namespace Corp.RouterService.Adapter.WcfAdapter
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
  public class InformationService : IServiceType, AntigonisTypes.IInformationServer
  {
    static IServiceTypeDelegates _delegates;
    static InformationDatagramProcessor _DatagramProcessor = null;
    static private Dictionary<string, Dictionary<string, string>> _appSettings;
    static string _serverName = null;

    static InformationService()
    {
      //for logging if someone forgets..
      _delegates = new InformationServiceDelegates();
      _DatagramProcessor = new InformationDatagramProcessor();
      _appSettings = new Dictionary<string, Dictionary<string, string>>();
      _serverName = "";
    }

    Message.Message _reponse = null;
    Message.Message _request = null;

    AntigonisTypes.Information.InformationMessageRequest _informationMessageRequest = null;

    public InformationService()
    {
    }

    public InformationService(string serverName, Dictionary<string, Dictionary<string, string>> appSettings)
    {
      _serverName = serverName;
      _appSettings = appSettings;

      _DatagramProcessor = new InformationDatagramProcessor();
    }

    public Type GetServiceType()
    {
      return typeof(InformationService);
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

    public IAsyncResult BeginPing(AntigonisTypes.Information.InformationMessageRequest request, AsyncCallback callback, object state)
    {
      return BeginProcessInformationRequest(request, callback, state);
    }

    public AntigonisTypes.Information.InformationMessageResponse EndPing(IAsyncResult result)
    {
      return EndProcessInformationRequest(result);
    }

    public IAsyncResult BeginProcessInformationRequest(AntigonisTypes.Information.InformationMessageRequest request, AsyncCallback callback, object state)
    {
      request.DisassembleFullMessageText();

      Message.Message requestMessage = ConstructInformationRequestMessage(request);

      ServiceAsyncResult asyncResult = new ServiceAsyncResult(state, request);

      _request = requestMessage;

      _informationMessageRequest = request;

      _delegates.HandleMessageDelegate(requestMessage, (response) =>
      {
        _reponse = response;

        asyncResult.IsCompleted = true;

        callback(asyncResult);
      });

      return asyncResult;
    }

    public AntigonisTypes.Information.InformationMessageResponse EndProcessInformationRequest(IAsyncResult result)
    {
      return ConstructInformationMessageResponse(ref _reponse);
    }




    private Message.Message ConstructInformationRequestMessage(AntigonisTypes.Information.InformationMessageRequest request)
    {
      try
      {
        InformationMsg requestMsg = new InformationMsgRequest(request.MessageHeader, request.MessageBody);

        Message.Message requestMessage = new Message.Message(MessageType.Information,
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

    private AntigonisTypes.Information.InformationMessageResponse ConstructInformationMessageResponse(ref Message.Message response)
    {
      try
      {
        if (response == null)
          throw new Exception("No response!!");

        _DatagramProcessor.PreprocessMessage(ref response);

        var InformationData = response.ProcessorData as InformationData;

        AntigonisTypes.Information.InformationMessageResponse result = 
          new AntigonisTypes.Information.InformationMessageResponse(InformationData.InformationMsg.ToString());

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