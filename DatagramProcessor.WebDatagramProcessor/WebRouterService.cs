﻿using System;

namespace Corp.RouterService.Message.RouterService
{
  public class WebRouterService : RouterService
  {
    private global::Corp.RouterService.Message.MessageRoutingTable _routingTable;

    public WebRouterService(global::Corp.RouterService.Message.MessageRoutingTable routingTable)
    {
      _routingTable = routingTable;
    }
    public override void RouteMessage(ref Message inMessage)
    {
      Uri destination = _routingTable.Route(inMessage);

      //the first should be the most significant


      inMessage.Info.OutgoingEndpoints = new MessageEndpoints()
      {
        RemoteEndpoind = new MessageEndpoint(destination)
      };

    }
  }
}
