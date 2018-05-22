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
        protected string[] urls;

        [System.Runtime.Serialization.DataMemberAttribute()]
        protected string type;

        [System.Runtime.Serialization.DataMemberAttribute()]
        protected string addTo;
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
        [WebInvoke(UriTemplate = "/layer",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "GET")]
        string GetLayerInformation(EarthLayerDescription lyr);

        [OperationContract]
        [WebInvoke(UriTemplate = "/layers",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "GET")]
        string GetLayersInformation(EarthLayerDescription lyr);

        [OperationContract]
        [WebInvoke(UriTemplate = "/layers",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "POST")]
        string ImportLayers(EarthLayerDescription lyr);

        [OperationContract]
        [WebInvoke(UriTemplate = "/layers",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "DELETE")]
        string ClearLayers(EarthLayerDescription lyr);

        [OperationContract]
        [WebInvoke(UriTemplate = "/layers",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json, Method = "DELETE")]
        string RemoveLayer(EarthLayerDescription lyr);


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
