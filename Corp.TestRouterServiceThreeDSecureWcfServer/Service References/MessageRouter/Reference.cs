﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Corp.TestRouterServiceThreePleLayerSecurityWcfServer.RouterService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ThreePleLayerSecurityMessageBase", Namespace="http://schemas.datacontract.org/2004/07/AntigonisTypes.ThreePleLayerSecurity")]
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Corp.TestRouterServiceThreePleLayerSecurityWcfServer.RouterService.ThreePleLayerSecurityMessageResponse))]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(Corp.TestRouterServiceThreePleLayerSecurityWcfServer.RouterService.ThreePleLayerSecurityMessageRequest))]
    public partial class ThreePleLayerSecurityMessageBase : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string MessageBodyField;
        
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
        public string MessageBody {
            get {
                return this.MessageBodyField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageBodyField, value) != true)) {
                    this.MessageBodyField = value;
                    this.RaisePropertyChanged("MessageBody");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="ThreePleLayerSecurityMessageResponse", Namespace="http://schemas.datacontract.org/2004/07/AntigonisTypes.ThreePleLayerSecurity")]
    [System.SerializableAttribute()]
    public partial class ThreePleLayerSecurityMessageResponse : Corp.TestRouterServiceThreePleLayerSecurityWcfServer.RouterService.ThreePleLayerSecurityMessageBase {
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ErrorDescriptionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool IsSuccessField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string ErrorDescription {
            get {
                return this.ErrorDescriptionField;
            }
            set {
                if ((object.ReferenceEquals(this.ErrorDescriptionField, value) != true)) {
                    this.ErrorDescriptionField = value;
                    this.RaisePropertyChanged("ErrorDescription");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool IsSuccess {
            get {
                return this.IsSuccessField;
            }
            set {
                if ((this.IsSuccessField.Equals(value) != true)) {
                    this.IsSuccessField = value;
                    this.RaisePropertyChanged("IsSuccess");
                }
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ThreePleLayerSecurityMessageRequest", Namespace="http://schemas.datacontract.org/2004/07/AntigonisTypes.ThreePleLayerSecurity")]
    [System.SerializableAttribute()]
    public partial class ThreePleLayerSecurityMessageRequest : Corp.TestRouterServiceThreePleLayerSecurityWcfServer.RouterService.ThreePleLayerSecurityMessageBase {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ThreePleLayerSecurityServerFault", Namespace="http://schemas.datacontract.org/2004/07/AntigonisTypes")]
    [System.SerializableAttribute()]
    public partial class ThreePleLayerSecurityServerFault : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CodeField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DetailsField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SourceField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TextField;
        
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
        public string Code {
            get {
                return this.CodeField;
            }
            set {
                if ((object.ReferenceEquals(this.CodeField, value) != true)) {
                    this.CodeField = value;
                    this.RaisePropertyChanged("Code");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Details {
            get {
                return this.DetailsField;
            }
            set {
                if ((object.ReferenceEquals(this.DetailsField, value) != true)) {
                    this.DetailsField = value;
                    this.RaisePropertyChanged("Details");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Source {
            get {
                return this.SourceField;
            }
            set {
                if ((object.ReferenceEquals(this.SourceField, value) != true)) {
                    this.SourceField = value;
                    this.RaisePropertyChanged("Source");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Text {
            get {
                return this.TextField;
            }
            set {
                if ((object.ReferenceEquals(this.TextField, value) != true)) {
                    this.TextField = value;
                    this.RaisePropertyChanged("Text");
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
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="RouterService.IThreePleLayerSecurityServer")]
    public interface IThreePleLayerSecurityServer {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IThreePleLayerSecurityServer/ProcessThreePleLayerSecurityRequest", ReplyAction="http://tempuri.org/IThreePleLayerSecurityServer/ProcessThreePleLayerSecurityRequestResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Corp.TestRouterServiceThreePleLayerSecurityWcfServer.RouterService.ThreePleLayerSecurityServerFault), Action="http://tempuri.org/IThreePleLayerSecurityServer/ProcessThreePleLayerSecurityRequestThreePleLayerSecurityServ" +
            "erFaultFault", Name="ThreePleLayerSecurityServerFault", Namespace="http://schemas.datacontract.org/2004/07/AntigonisTypes")]
        Corp.TestRouterServiceThreePleLayerSecurityWcfServer.RouterService.ThreePleLayerSecurityMessageResponse ProcessThreePleLayerSecurityRequest(Corp.TestRouterServiceThreePleLayerSecurityWcfServer.RouterService.ThreePleLayerSecurityMessageRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IThreePleLayerSecurityServer/Ping", ReplyAction="http://tempuri.org/IThreePleLayerSecurityServer/PingResponse")]
        [System.ServiceModel.FaultContractAttribute(typeof(Corp.TestRouterServiceThreePleLayerSecurityWcfServer.RouterService.ThreePleLayerSecurityServerFault), Action="http://tempuri.org/IThreePleLayerSecurityServer/PingThreePleLayerSecurityServerFaultFault", Name="ThreePleLayerSecurityServerFault", Namespace="http://schemas.datacontract.org/2004/07/AntigonisTypes")]
        Corp.TestRouterServiceThreePleLayerSecurityWcfServer.RouterService.ThreePleLayerSecurityMessageResponse Ping(Corp.TestRouterServiceThreePleLayerSecurityWcfServer.RouterService.ThreePleLayerSecurityMessageRequest request);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IThreePleLayerSecurityServerChannel : Corp.TestRouterServiceThreePleLayerSecurityWcfServer.RouterService.IThreePleLayerSecurityServer, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ThreePleLayerSecurityServerClient : System.ServiceModel.ClientBase<Corp.TestRouterServiceThreePleLayerSecurityWcfServer.RouterService.IThreePleLayerSecurityServer>, Corp.TestRouterServiceThreePleLayerSecurityWcfServer.RouterService.IThreePleLayerSecurityServer {
        
        public ThreePleLayerSecurityServerClient() {
        }
        
        public ThreePleLayerSecurityServerClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ThreePleLayerSecurityServerClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ThreePleLayerSecurityServerClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ThreePleLayerSecurityServerClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Corp.TestRouterServiceThreePleLayerSecurityWcfServer.RouterService.ThreePleLayerSecurityMessageResponse ProcessThreePleLayerSecurityRequest(Corp.TestRouterServiceThreePleLayerSecurityWcfServer.RouterService.ThreePleLayerSecurityMessageRequest request) {
            return base.Channel.ProcessThreePleLayerSecurityRequest(request);
        }
        
        public Corp.TestRouterServiceThreePleLayerSecurityWcfServer.RouterService.ThreePleLayerSecurityMessageResponse Ping(Corp.TestRouterServiceThreePleLayerSecurityWcfServer.RouterService.ThreePleLayerSecurityMessageRequest request) {
            return base.Channel.Ping(request);
        }
    }
}