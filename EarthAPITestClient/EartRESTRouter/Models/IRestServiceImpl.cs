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
        string GetCameraJson();

        [OperationContract]
        [WebInvoke(Method = "PUT",
            ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Wrapped,
            UriTemplate = "camera/{json}")]
        string SetCamera(string json);

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
        string AddLayer(EarthLayerDescription lyr);

        [OperationContract]
        [WebInvoke(UriTemplate = "/layer/{layerId}/load_status",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "GET")]
        string GetLayerLoadStatus(string layerId);

        [OperationContract]
        [WebInvoke(UriTemplate = "/layer/{layerId}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "DELETE")]
        string RemoveLayer(string layerId);

        [OperationContract]
        [WebInvoke(UriTemplate = "/workspace",
            ResponseFormat = WebMessageFormat.Json, Method = "PUT")]
        string ImportWorkspace(Stream stream);

        [OperationContract]
        [WebInvoke(UriTemplate = "/workspace",
            ResponseFormat = WebMessageFormat.Json, Method = "GET")]
        string GetWorkspace();

        [OperationContract]
        [WebInvoke(UriTemplate = "/layers/{json}",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "DELETE")]
        string ClearLayers(string json);

        [OperationContract]
        [WebGet(UriTemplate = "snapshot")]
        Stream GetSnapshot();
   }
}
