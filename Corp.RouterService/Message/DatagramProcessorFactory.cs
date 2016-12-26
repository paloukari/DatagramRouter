using Corp.RouterService.Message.DatagramProcessor;

namespace Corp.RouterService.Message
{
  public static class DatagramProcessorFactory
  {
    static Iso8583DatagramProcessor _iso8583DatagramProcessor = null;
    static Iso8583ATMDatagramProcessor _iso8583ATMDatagramProcessor = null;
    static IsoInternalDatagramProcessor _isoInternalDatagramProcessor = null;
    static IsoInternalPosDatagramProcessor _isoInternalPosDatagramProcessor = null;
    static WebDatagramProcessor _webDatagramProcessor = null;
    static OperationDatagramProcessor _operationDatagramProcessor = null;
    static InformationDatagramProcessor _informationDatagramProcessor = null;
    static HsmDatagramProcessor _hsmDatagramProcessor = null;
    static ThreePleLayerSecurityDatagramProcessor _threePleLayerSecurityDatagramProcessor = null;
    static ItpDatagramProcessor _itpDatagramProcessor = null;


    public static DatagramProcessor.DatagramProcessor GetDatagramProcessor(Message message)
    {
      switch (message.Type)
      {
        case MessageType.Iso8583:
          {
            if (_iso8583DatagramProcessor == null)
              _iso8583DatagramProcessor = new DatagramProcessor.Iso8583DatagramProcessor();
            return _iso8583DatagramProcessor;
          }
        case MessageType.Iso8583ATM:
          {
            if (_iso8583ATMDatagramProcessor == null)
              _iso8583ATMDatagramProcessor = new DatagramProcessor.Iso8583ATMDatagramProcessor();
            return _iso8583ATMDatagramProcessor;
          }
        case MessageType.IsoInternal:
          {
            if (_isoInternalDatagramProcessor == null)
            {
              _isoInternalDatagramProcessor = new DatagramProcessor.IsoInternalDatagramProcessor();
            }
            return _isoInternalDatagramProcessor;
          }
        case MessageType.IsoInternalPos:
          {
            if (_isoInternalPosDatagramProcessor == null)
            {
              _isoInternalPosDatagramProcessor = new DatagramProcessor.IsoInternalPosDatagramProcessor();
            }
            return _isoInternalPosDatagramProcessor;
          }
        case MessageType.Web:
          {
            if (_webDatagramProcessor == null)
            {
              _webDatagramProcessor = new DatagramProcessor.WebDatagramProcessor();
            }
            return _webDatagramProcessor;
          }
        case MessageType.Operation:
          {
            if (_operationDatagramProcessor == null)
            {
              _operationDatagramProcessor = new DatagramProcessor.OperationDatagramProcessor();
            }
            return _operationDatagramProcessor;
          }
        case MessageType.Information:
          {
            if (_informationDatagramProcessor == null)
            {
              _informationDatagramProcessor = new DatagramProcessor.InformationDatagramProcessor();
            }
            return _informationDatagramProcessor;
          }
        case MessageType.Hsm:
          {
            if (_hsmDatagramProcessor == null)
            {
              _hsmDatagramProcessor = new DatagramProcessor.HsmDatagramProcessor();
            }
            return _hsmDatagramProcessor;
          }
        case MessageType.ThreePleLayerSecurity:
          {
            if (_threePleLayerSecurityDatagramProcessor == null)
            {
              _threePleLayerSecurityDatagramProcessor = new DatagramProcessor.ThreePleLayerSecurityDatagramProcessor();
            }
            return _threePleLayerSecurityDatagramProcessor;
          }
        case MessageType.Itp:
          {
              if (_itpDatagramProcessor == null)
              {
                  _itpDatagramProcessor = new DatagramProcessor.ItpDatagramProcessor();
              }
              return _itpDatagramProcessor;
          }
        default:
          return new DatagramProcessor.DatagramProcessor();
      }
    }
  }
}
