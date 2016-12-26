using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Corp.RouterService.Adapter.SqlAdapter;
using Corp.RouterService.Message.DatagramProcessor;

namespace Corp.RouterService.Adapter.SqlAdapter
{
  public class HsmQueues : IQueues
  {
    private static readonly LoggingLibrary.Log4Net.ILog log = LoggingLibrary.LoggerManager.GetLog4NetLogger(
   System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private string _serverGuid;
    private string _connectionString;
    private string _serverName;
    Queue<Message.Message> _memoryQueue = null;
    HsmDatagramProcessor _DatagramProcessor = null;
    private Dictionary<string, Dictionary<string, string>> _appSettings;

    public HsmQueues(string serverGuid, string connectionString, string serverName, Dictionary<string, Dictionary<string, string>> appSettings)
    {
      _serverGuid = serverGuid;
      _connectionString = connectionString;
      _serverName = serverName;

      _memoryQueue = new Queue<Message.Message>();

      _DatagramProcessor = new HsmDatagramProcessor();

      _appSettings = appSettings;
    }
    public void InputQueueEnqueue(Corp.RouterService.Message.Message message)
    {
      throw new NotImplementedException();
    }

    public Corp.RouterService.Message.Message InputQueueDequeue()
    {
      throw new NotImplementedException();
    }

    public void OutputQueueEnqueue(Corp.RouterService.Message.Message message)
    {
      throw new NotImplementedException();
    }

    public Corp.RouterService.Message.Message OutputQueueDequeue()
    {
      throw new NotImplementedException();
    }
  }
}
