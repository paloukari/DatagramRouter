using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Corp.RouterService.Adapter.SqlAdapter
{
  public static class SqlQueueFactory
  {
    public static IQueues GetSqlQueues(SqlAdapterSettings settings)
    {
      switch (settings.MessageType)
      {
        case Corp.RouterService.Message.MessageType.Iso8583:
          return new Iso8583Queues(settings.Guid, settings.ConnectionString, settings.Name, settings.AppSettings);
        case Corp.RouterService.Message.MessageType.Iso8583ATM:
          return new Iso8583ATMQueues(settings.Guid, settings.ConnectionString, settings.Name, settings.AppSettings);
        case Corp.RouterService.Message.MessageType.IsoInternal:
          return new IsoInternalQueues(settings.Guid, settings.ConnectionString, settings.Name, settings.AppSettings);
        case Corp.RouterService.Message.MessageType.IsoInternalPos:
          return new IsoInternalPosQueues(settings.Guid, settings.ConnectionString, settings.Name, settings.AppSettings);
        case Corp.RouterService.Message.MessageType.Web:
          break;
        case Corp.RouterService.Message.MessageType.Operation:
          return new OperationQueues(settings.Guid, settings.ConnectionString, settings.Name, settings.AppSettings);
        case Corp.RouterService.Message.MessageType.Hsm:
          return new HsmQueues(settings.Guid, settings.ConnectionString, settings.Name, settings.AppSettings);
        case Corp.RouterService.Message.MessageType.Unknown:
          break;
        default:
          break;
      }

      throw new Exception("Could not find IQueues for settings:" + settings);
    }
  }
}
