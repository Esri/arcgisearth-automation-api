using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.ComponentModel.Composition;

namespace ArcGISEarth.WCFNamedPipeIPC
{
    [ServiceContract]
    public interface IRestServiceImpl
    {
        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "camera")]
        string GetCamera();

        [OperationContract]
        [WebInvoke(Method = "PUT",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "camera/{json}")]
        string UpdateCamera(string json);

        [OperationContract]
        [WebInvoke(Method = "PUT",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "flyto/{json}")]
        string FlyTo(string json);

        [OperationContract]
        [WebInvoke(UriTemplate = "/layer",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        string AddOpertionalLayer(LayerShellContract lyr);

//        [OperationContract]
//        [WebInvoke(UriTemplate = "/layers",
//            RequestFormat = WebMessageFormat.Json,
//            ResponseFormat = WebMessageFormat.Json, Method = "POST")]
//        string AddLayers(EarthLayerDescription lyr);

        [OperationContract]
        [WebInvoke(Method = "GET",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "operationallayers")]
        string GetOperationalLayers();

        [OperationContract]
        [WebInvoke(Method = "DELETE",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "operationallayers")]
        string RemoveOperationalLayers();

        [OperationContract]
        [WebGet(UriTemplate = "snapshot")]
        Stream GetSnapshot();
   }

    [System.Runtime.Serialization.DataContractAttribute()]
    public class LayerShellContract
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string url { get; set; }
    }

}
