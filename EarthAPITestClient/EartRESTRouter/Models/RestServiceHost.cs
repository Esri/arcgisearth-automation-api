using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Windows;

namespace ArcGISEarth.WCFNamedPipeIPC.Models.rest
{
    public class RestServiceHost
    {
        static ServiceHost host;
        static Uri baseAddress = new Uri("http://localhost:80/Temporary_Listen_Addresses/arcgisearth/api/v1");

        public static bool Start()
        {
            try
            {
                host = new ServiceHost(typeof(ArcGISEarth.WCFNamedPipeIPC.RestServiceImpl), baseAddress);
                // Enable metadata publishing.
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true
                };
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);
                WebHttpBinding binding = new WebHttpBinding();
                ServiceEndpoint ep = new ServiceEndpoint(
                    ContractDescription.GetContract(
                    typeof(IRestServiceImpl)),
                    binding,
                    new EndpointAddress(baseAddress));
                ep.EndpointBehaviors.Add(new WebHttpBehavior());
                host.AddServiceEndpoint(ep);

                ServiceDebugBehavior sdb = host.Description.Behaviors.Find<ServiceDebugBehavior>();
                sdb.IncludeExceptionDetailInFaults = true;

                host.Open();
                MessageBox.Show("Start service now!");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                MessageBox.Show(ex.Message);
            }

            return false;
        }

        public static bool Stop()
        {
            host.Close();
            MessageBox.Show("Stop service now!");
            return true;
        }
    }
}
