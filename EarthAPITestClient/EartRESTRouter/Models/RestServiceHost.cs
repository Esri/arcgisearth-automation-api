using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ArcGISEarth.WCFNamedPipeIPC.Models.rest
{
    public class RestServiceHost
    {
        static ServiceHost host;
        static Uri baseAddress = new Uri("http://localhost:50066/arcgisearth");

        public static bool Start()
        {
            //SettingController setting = new SettingController();
            //setting.Apply(true);
            //baseAddress = new Uri(setting.BaseAddress);

            // TODO
            try
            {
                //if (false == RestServiceButton.ServiceStarted)
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
                    //RestServiceButton.ServiceStarted = true;
                    return true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }

            return false;
        }

        public static bool Stop()
        {
            //if (true == RestServiceButton.ServiceStarted)
            {
                host.Close();
                //RestServiceButton.ServiceStarted = false;
                return true;
            }
            return false;
        }
    }
}
