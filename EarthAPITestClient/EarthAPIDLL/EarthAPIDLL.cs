using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EarthAPIUtils;
using System.Runtime.InteropServices;

namespace EarthAPIDLL
{

    [ComVisible(true)]
    public interface IEarthAPIDLL
    {

        string Init();

        string AddLayer(string json);

        string GetCamera();
    }

    [ComVisible(true)]
    public class EarthAPIDLLClass: IEarthAPIDLL
    {
        private EarthNamedpipeAPIUtils _utils = null;

        public string Init()
        {
            _utils = new EarthNamedpipeAPIUtils();
            if(_utils == null)
            {
                return "Failed to create utils";

            }
            return _utils.Connect().Result;
        }

        public string AddLayer(string json)
        {
            return _utils.AddLayer(json);
        }

        public string GetCamera()
        {
            return _utils.GetCamera();
        }

   }
}
