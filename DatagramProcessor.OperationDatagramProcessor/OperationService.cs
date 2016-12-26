using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Corp.RouterService.Message.DatagramProcessor;
using Corp.RouterService.Message;
using System.Threading.Tasks;
using System.Threading;
using DatagramProcessor.OperationDatagramProcessor;

namespace Corp.RouterService.Adapter.WcfAdapter
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
  public class OperationService : IServiceType,IOperationService
  {
    static IServiceTypeDelegates _delegates;
    static OperationDatagramProcessor _DatagramProcessor = null;
    static private Dictionary<string, Dictionary<string, string>> _appSettings;
    static string _serverName = null;    

    static OperationService()
    {
      //for logging if someone forgets..
      _delegates = new OperationServiceDelegates();
      _DatagramProcessor = new OperationDatagramProcessor();
      _appSettings = new Dictionary<string, Dictionary<string, string>>();
      _serverName = "";
    }

    Message.Message _result = null;

    public OperationService()
    {
    }

    public OperationService(string serverName, Dictionary<string, Dictionary<string, string>> appSettings)
    {
      _serverName = serverName;
      _appSettings = appSettings;

      _DatagramProcessor = new OperationDatagramProcessor();
    }

    public Type GetServiceType()
    {
      return typeof(OperationService);
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

    public IAsyncResult BeginGetCardSum(OperationGetCardSumRequest getCardSumRequest, AsyncCallback callback, object state)
    {
      return BeginProcessOperationRequest(getCardSumRequest.OperationMessageRequest, callback, state);      
    }

    public OperationCardSummaryResponse EndGetCardSum(IAsyncResult result)
    {
      return new OperationCardSummaryResponse(ConstructOperationMessageResponse(ref _result));
    }
       
    public IAsyncResult BeginPing(OperationPingRequest request, AsyncCallback callback, object state)
    {
      return BeginProcessOperationRequest(request.OperationMessageRequest, callback, state);      
    }

    public OperationPingResponse EndPing(IAsyncResult result)
    {
      return new OperationPingResponse(EndProcessOperationRequest(result));
    }
  
    public IAsyncResult BeginProcessOperationRequest(AntigonisTypes.Operation.OperationMessageRequest request, AsyncCallback callback, object state)
    {
      Message.Message pingMsg = ConstructOperationRequestMessage(request);

      ServiceAsyncResult asyncResult = new ServiceAsyncResult(state, request);

      _delegates.HandleMessageDelegate(pingMsg, (response) =>
      {
        _result = response;

        asyncResult.IsCompleted = true;

        callback(asyncResult);
      });

      return asyncResult;
    }

    public AntigonisTypes.Operation.OperationMessageResponse EndProcessOperationRequest(IAsyncResult result)
    {
      return ConstructOperationMessageResponse(ref _result);
    }




    private Message.Message ConstructOperationRequestMessage(AntigonisTypes.Operation.OperationMessageRequest request)
    {
      try
      {
        OperationData operationMessage = new OperationData(request);

        Message.Message msg = new Message.Message(MessageType.Operation, operationMessage.ToString(), new MessageEndpoints() { LocalEndpoint = new MessageEndpoint(new Uri("wcf://" + _serverName)) });

        _DatagramProcessor.PreprocessMessage(ref msg);

        return msg;
      }
      catch (Exception ex)
      {
        throw new FaultException<Exception>(ex);
      }
    }
    
    private AntigonisTypes.Operation.OperationMessageResponse ConstructOperationMessageResponse(ref Message.Message response)
    {
      try
      {
        if (response == null)
          throw new Exception("No response!!");

        _DatagramProcessor.PreprocessMessage(ref response);

        var operationData = response.ProcessorData as OperationData;

        AntigonisTypes.Operation.OperationMessageResponse result = new AntigonisTypes.Operation.OperationMessageResponse(operationData.OperationMessage.FullMessageText);

        return result;

      }
      catch (Exception ex)
      {
        throw new FaultException<Exception>(ex);
      }
    }


  }
}