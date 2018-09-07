using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

namespace ArcGISEarth.WCFNamedPipeIPC
{
    public class ServiceRelatedUtils
    {
        static public bool DoesBaseURLWork(string BaseAddress)
        {
            try
            {
                ServiceHost host = new ServiceHost(typeof(ArcGISEarth.WCFNamedPipeIPC.RestServiceImpl), new Uri(BaseAddress));
                // Enable metadata publishing.
                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(smb);
                WebHttpBinding binding = new WebHttpBinding();
                ServiceEndpoint ep = new ServiceEndpoint(
                    ContractDescription.GetContract(
                    typeof(IRestServiceImpl)),
                    binding,
                    new EndpointAddress(BaseAddress));
                ep.EndpointBehaviors.Add(new WebHttpBehavior());
                host.AddServiceEndpoint(ep);
                ServiceDebugBehavior sdb = host.Description.Behaviors.Find<ServiceDebugBehavior>();
                sdb.IncludeExceptionDetailInFaults = true;
                host.Open();
                host.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        static public bool IsFreePort(int port)
        {
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();
            List<int> usedPorts = tcpEndPoints.Select(p => p.Port).ToList<int>();
            usedPorts.Sort();
            return !usedPorts.Contains(port);
        }

        static public string AutoFindFreePort()
        {
            int PortStartIndex = 50000;
            int PortEndIndex = 60000;
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();

            List<int> usedPorts = tcpEndPoints.Select(p => p.Port).ToList<int>();
            usedPorts.Sort();
            int unusedPort = 0;

            for (int port = PortStartIndex; port < PortEndIndex; port++)
            {
                if (!usedPorts.Contains(port))
                {
                    unusedPort = port;
                    break;
                }
            }
            return unusedPort.ToString();
        }

        // Example: netsh http add urlacl url=http://+:8001/ user=\EveryOne
        // add sepcial url to access control list
        static public bool AddtoUrlAcl(string port, string endpoint)
        {
            System.Diagnostics.ProcessStartInfo netshprocess = new System.Diagnostics.ProcessStartInfo();
            netshprocess.FileName = "netsh.exe";

            string temp = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            // TODO: more setting about user
            string user = "";
            for (int i = 0; i < temp.Length; i++)
            {
                user += temp[i];
                if (temp[i] == '\\')
                {
                    user = "";
                }
            }
            netshprocess.Verb = "runas";
            netshprocess.Arguments = "";
            netshprocess.Arguments = "http add urlacl url=http://+:" + port + endpoint + @" user=\EveryOne";
            netshprocess.UseShellExecute = true;

            // TODO: More details about error / warning
            try
            {
                System.Diagnostics.Process.Start(netshprocess).WaitForExit();
            }
            catch (Exception)
            {

            };
            return true;
        }

        // TODO pay more attention for admin privilege
        static public bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

    }
}
