using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcGISEarth.WCFNamedPipeIPC
{
    [System.Runtime.Serialization.DataContractAttribute()]
    public class EarthLayerDescriptionImpl : EarthLayerDescription
    {
        public static EarthLayerDescription FromJson(string json) { return null; }
        public string ToJson() { return null; }
    }
}
