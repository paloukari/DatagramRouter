using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Corp.RouterService.Message.DatagramProcessor;
using Corp.RouterService.Message;
using HsmLibrary;
using System.Threading.Tasks;
using DatagramProcessor.HsmDatagramProcessor;
using System.Threading;

namespace Corp.RouterService.Adapter.WcfAdapter
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, 
    ConcurrencyMode = ConcurrencyMode.Multiple)]
  public class HsmService : IServiceType, AntigonisTypes.IHsm
  {
    static IServiceTypeDelegates _delegates;
    static HsmDatagramProcessor _DatagramProcessor = null;
    static private Dictionary<string, Dictionary<string, string>> _appSettings;
    static string _serverName = null;
        

    static HsmService()
    {
      //for logging if someone forgets..
      _delegates = new HsmServiceDelegates();
      _DatagramProcessor = new HsmDatagramProcessor();
      _appSettings = new Dictionary<string, Dictionary<string, string>>();
      _serverName = "";
    }

    Message.Message _response = null;
    private Message.Message _request = null;

    public HsmService()
    {
    }

    public HsmService(string serverName, Dictionary<string, Dictionary<string, string>> appSettings)
    {
      _serverName = serverName;
      _appSettings = appSettings;      
      _DatagramProcessor = new HsmDatagramProcessor();
    }

    public Type GetServiceType()
    {
      return typeof(HsmService);
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

    #region HsmRequest Implementation

    public IAsyncResult BeginHsmRequest(AntigonisTypes.HsmMessageRequest hsmRequest, AsyncCallback callback, object state)
    {
      Message.Message hsmMsg = ConstructHsmMessage(hsmRequest);

      ServiceAsyncResult asyncResult = new ServiceAsyncResult(state, hsmRequest);

      _request = hsmMsg;

      _delegates.HandleMessageDelegate(hsmMsg, (hsmResponse) =>
      {
        _response = hsmResponse;

        callback(asyncResult);
      });

      return asyncResult;
    }

    public AntigonisTypes.HsmMessageResponse EndHsmRequest(IAsyncResult result)
    {
      return ConstructHsmMessageResponse(ref _response);
    }

    private Message.Message ConstructHsmMessage(AntigonisTypes.HsmMessageRequest hsmRequest)
    {
      try
      {        
        HsmMsgRequest requestMsg = new HsmMsgRequest(hsmRequest.RequestMessageBody);

        Message.Message requestMessage = new Message.Message(MessageType.Hsm,
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

    private AntigonisTypes.HsmMessageResponse ConstructHsmMessageResponse(ref Message.Message hsmResponse)
    {
      try
      {
        if (hsmResponse == null)
          throw new Exception("No response!!");

        _DatagramProcessor.PreprocessMessage(ref hsmResponse);

        var hsmData = hsmResponse.ProcessorData as HsmData;
        
        if (_request == null || _response == null || _request.ID != _response.RetreivalID || hsmData == null || hsmData.HsmMsg as HsmMsgResponse == null)
          return new AntigonisTypes.HsmMessageResponse()
          {
            ErrorCode = AntigonisTypes.HsmError.InternalError,
            ErrorDescription = AntigonisTypes.HsmError.InternalError.ToString()
          };

        var hsmMsgResponse = hsmData.HsmMsg as HsmMsgResponse;

        AntigonisTypes.HsmMessageResponse result = new AntigonisTypes.HsmMessageResponse()
        {
          MessageBody = hsmMsgResponse.Body,
          IsSuccess = hsmMsgResponse.HsmErrorCode == AntigonisTypes.HsmError.NoError,
          ErrorCode = hsmMsgResponse.HsmErrorCode,
          ErrorDescription = hsmMsgResponse.HsmErrorCode.ToString()
        };

        return result;
      }
      catch (Exception ex)
      {
        throw new FaultException<Exception>(ex);
      }
    }

    #endregion
  }
}
