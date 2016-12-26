using System;

namespace Corp.RouterService.Message
{
    public class MessageEndpoint
    {
        string _address;

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }
        int _port;

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        MessageEndpointType _endpointType;

        public MessageEndpoint()
        {

        }
        public MessageEndpoint(Uri uri)
        {
          switch (uri.Scheme)
          {
            case "tcp":
              {
                Address = uri.Host;
                Port = uri.Port;
                EndpointType = MessageEndpointType.tcp;
              } break;

            case "sql":
              {
                //todo
                //complete the copy
                Address = uri.Host;
                Port = -1;
                EndpointType = MessageEndpointType.sql;
              } break;
            case "wcf":
              {
                //todo
                //complete the copy
                Address = uri.Host;
                Port = -1;
                EndpointType = MessageEndpointType.wcf;
              } break;
            case "mem":
              {
                //todo
                //complete the copy
                Address = uri.Host;
                Port = -1;
                EndpointType = MessageEndpointType.mem;
              } break;
          }
        }

        public MessageEndpointType EndpointType
        {
            get { return _endpointType; }
            set { _endpointType = value; }
        }

        public override bool Equals(object obj)
        {
            MessageEndpoint to = obj as MessageEndpoint;
            if (to != null)
            {
                if (this.EndpointType == to.EndpointType && this.Address == to.Address && this.Port == to.Port)
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.EndpointType.GetHashCode() + this.Address.GetHashCode() + this.Port.GetHashCode();
        }
        public override string ToString()
        {
          return EndpointType.ToString() + ":" + (Address != null ? Address.ToString() : "(NULL)") + ":" + Port;
        }


    }
}
