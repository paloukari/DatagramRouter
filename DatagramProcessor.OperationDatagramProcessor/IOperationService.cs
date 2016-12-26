using System;
using System.ServiceModel;
using System.Runtime.Serialization;
using AntigonisTypes.Operation;
namespace Corp.RouterService.Adapter.WcfAdapter
{
  [ServiceContract]
  [ServiceKnownType(typeof(OperationMessageRequest))]
  [ServiceKnownType(typeof(OperationMessageResponse))]
  [ServiceKnownType(typeof(OperationMessageBase))]
  public interface IOperationService
  {
    [OperationContract(AsyncPattern = true)]
    IAsyncResult BeginGetCardSum(OperationGetCardSumRequest getCardSumRequest, AsyncCallback callback, object state);
    OperationCardSummaryResponse EndGetCardSum(IAsyncResult result);

    [OperationContract(AsyncPattern = true)]
    IAsyncResult BeginProcessOperationRequest(OperationMessageRequest request, AsyncCallback callback, object state);
    OperationMessageResponse EndProcessOperationRequest(IAsyncResult result);

    [OperationContract(AsyncPattern = true)]
    IAsyncResult BeginPing(OperationPingRequest request, AsyncCallback callback, object state);
    OperationPingResponse EndPing(IAsyncResult result);
  }

  [DataContract]
  public class OperationPingRequest 
  {    
    public OperationMessageRequest OperationMessageRequest
    {
      get
      {
        return new OperationMessageRequest("", "", "", "", "");
      }
    }
  }

  [DataContract]
  public class OperationPingResponse
  {
    public OperationPingResponse(OperationMessageBase operationMessageBase)
    {
    }
  }

  [DataContract]
  public class OperationGetCardSumRequest
  {
    [DataMember]
    public string CardNumber { get; set; }

    public OperationMessageRequest OperationMessageRequest
    {
      get
      {
        return new OperationMessageRequest("", "", "", "", CardNumber);
      }
    }
  }

  [DataContract]
  public class OperationCardSummaryResponse 
  {
    public OperationCardSummaryResponse(OperationMessageBase operationMessageBase)
    {
    }
  }
}
