﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ServeurSOAP.ServiceReference1 {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference1.IProxy")]
    public interface IProxy {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProxy/getStation", ReplyAction="http://tempuri.org/IProxy/getStationResponse")]
        string getStation(int number, string chosenContract);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProxy/getStation", ReplyAction="http://tempuri.org/IProxy/getStationResponse")]
        System.Threading.Tasks.Task<string> getStationAsync(int number, string chosenContract);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProxy/getAllStationsOfAContract", ReplyAction="http://tempuri.org/IProxy/getAllStationsOfAContractResponse")]
        string getAllStationsOfAContract(string chosenContract);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IProxy/getAllStationsOfAContract", ReplyAction="http://tempuri.org/IProxy/getAllStationsOfAContractResponse")]
        System.Threading.Tasks.Task<string> getAllStationsOfAContractAsync(string chosenContract);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IProxyChannel : ServeurSOAP.ServiceReference1.IProxy, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ProxyClient : System.ServiceModel.ClientBase<ServeurSOAP.ServiceReference1.IProxy>, ServeurSOAP.ServiceReference1.IProxy {
        
        public ProxyClient() {
        }
        
        public ProxyClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ProxyClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProxyClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ProxyClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string getStation(int number, string chosenContract) {
            return base.Channel.getStation(number, chosenContract);
        }
        
        public System.Threading.Tasks.Task<string> getStationAsync(int number, string chosenContract) {
            return base.Channel.getStationAsync(number, chosenContract);
        }
        
        public string getAllStationsOfAContract(string chosenContract) {
            return base.Channel.getAllStationsOfAContract(chosenContract);
        }
        
        public System.Threading.Tasks.Task<string> getAllStationsOfAContractAsync(string chosenContract) {
            return base.Channel.getAllStationsOfAContractAsync(chosenContract);
        }
    }
}
