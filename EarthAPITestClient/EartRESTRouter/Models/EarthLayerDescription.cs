using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArcGISEarth.WCFNamedPipeIPC
{
    [System.Runtime.Serialization.DataContractAttribute()]
    public class EarthLayerDescriptionImpl : EarthLayerDescription
    {
        public EarthLayerDescriptionImpl(EarthLayerDescription impl)
        {
            URI = impl.URI;
            type = impl.type;
            target = impl.target;
        }

        public static EarthLayerDescription FromJson(string json) { return null; }
        public string ToJson()
        {
            JObject jObj = new JObject
            {
                ["URI"] = this.URI,
                ["type"] = this.type,
                ["target"] = this.target
            };
            return jObj.ToString();
        }
    }
}
