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
    [System.Runtime.Serialization.DataContractAttribute()]
    public class EarthLayerDescription
    {
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string URI;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string type;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public string target;
    }

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
        string AddLayerSync(EarthLayerDescription lyr);

        [OperationContract]
        [WebInvoke(UriTemplate = "/layer/{layerId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "GET")]
        string GetLayerInformation(string layerId);

        [OperationContract]
        [WebInvoke(UriTemplate = "/layers",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "GET")]
        string GetLayersInformation();

        [OperationContract]
        [WebInvoke(UriTemplate = "/layers/{json}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        string ImportLayers(string json);

        [OperationContract]
        [WebInvoke(UriTemplate = "/layers/{json}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "DELETE")]
        string ClearLayers(string json);

        [OperationContract]
        [WebInvoke(UriTemplate = "/layer/{layerId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "DELETE")]
        string RemoveLayer(string layerId);

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
