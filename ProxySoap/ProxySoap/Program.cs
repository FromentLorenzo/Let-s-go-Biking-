using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ProxySoap
{
    public class Program
    {
        static void Main(string[] args)
        {
            Uri httpUrl = new Uri("http://localhost:8090/IProxy/Proxy");

            //Create ServiceHost
            ServiceHost host = new ServiceHost(typeof(Proxy), httpUrl);

            //Add a service endpoint
            host.AddServiceEndpoint(typeof(IProxy), new WSHttpBinding(), "");

            //Enable metadata exchange
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);

            //Start the Service
            host.Open();

            Console.WriteLine("Service is host at " + DateTime.Now.ToString());
            Console.WriteLine("Host is running... Press <Enter> key to stop");
            Console.ReadLine();

        }
    }
}
