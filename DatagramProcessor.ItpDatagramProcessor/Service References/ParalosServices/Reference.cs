﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Corp.RouterService.Message.DatagramProcessor.AntigonisServices {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="MessageRequest", Namespace="http://schemas.datacontract.org/2004/07/seCoral.CoralLoyalty")]
    [System.SerializableAttribute()]
    public partial class MessageRequest : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="MessageResponse", Namespace="http://schemas.datacontract.org/2004/07/seCoral.CoralLoyalty")]
    [System.SerializableAttribute()]
    public partial class MessageResponse : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="urn:CoralLoyalty", ConfigurationName="AntigonisServices.ICoralLoyalty")]
    public interface ICoralLoyalty {
        
        [System.ServiceModel.OperationContractAttribute(Action="Ping", ReplyAction="urn:CoralLoyalty/ICoralLoyalty/PingResponse")]
        Corp.RouterService.Message.DatagramProcessor.AntigonisServices.MessageResponse Ping(Corp.RouterService.Message.DatagramProcessor.AntigonisServices.MessageRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="RequestDispatcher", ReplyAction="urn:CoralLoyalty/ICoralLoyalty/RequestDispatcherResponse")]
        Corp.RouterService.Message.DatagramProcessor.AntigonisServices.MessageResponse RequestDispatcher(Corp.RouterService.Message.DatagramProcessor.AntigonisServices.MessageRequest request);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICoralLoyaltyChannel : Corp.RouterService.Message.DatagramProcessor.AntigonisServices.ICoralLoyalty, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CoralLoyaltyClient : System.ServiceModel.ClientBase<Corp.RouterService.Message.DatagramProcessor.AntigonisServices.ICoralLoyalty>, Corp.RouterService.Message.DatagramProcessor.AntigonisServices.ICoralLoyalty {
        
        public CoralLoyaltyClient() {
        }
        
        public CoralLoyaltyClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public CoralLoyaltyClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CoralLoyaltyClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CoralLoyaltyClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Corp.RouterService.Message.DatagramProcessor.AntigonisServices.MessageResponse Ping(Corp.RouterService.Message.DatagramProcessor.AntigonisServices.MessageRequest request) {
            return base.Channel.Ping(request);
        }
        
        public Corp.RouterService.Message.DatagramProcessor.AntigonisServices.MessageResponse RequestDispatcher(Corp.RouterService.Message.DatagramProcessor.AntigonisServices.MessageRequest request) {
            return base.Channel.RequestDispatcher(request);
        }
    }
}
