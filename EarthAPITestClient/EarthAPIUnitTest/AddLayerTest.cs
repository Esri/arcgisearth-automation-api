using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading; //SemaphoreSlim
using EarthAPIUtils; 

namespace EarthAPIUnitTest
{
    [TestClass]
    public class AddLayerTest
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private EarthNamedpipeAPIUtils _utils;
        private List<string> _msgs = new List<string>();
        private string _currentMsg;

        private void GetMessageFromArcGISEarth(Object sender, EventArgs e)
        {
            if (e is MessageStringEventArgs)
            {
                _currentMsg = (e as MessageStringEventArgs).Message;
                _semaphore.Release();
            }
        }

        private async void Connect()
        {
            await _utils.Connect();
        }

        internal class LayerTestCase
        {
            public string input;
            public string output;
            public string id;
            public string keyWords;
        }

        private List<LayerTestCase> GetTestCases()
        {
            List<LayerTestCase> lyrs = null;
            string appFolder = System.AppDomain.CurrentDomain.BaseDirectory;
            // Note, this json file is only used for internal test. So you might not be able to find the file.
            // Here is an example:
            
            // {
            // "testcases": [
            //   {
            //     "input": {
            //       "type": "FeatureService", 
            //       "target": "BasemapLayers", 
            //       "URI": "https://gis.internationalmedicalcorps.org/arcgis/rest/services/Nepal/Nepal_Earthquake_2015_data/FeatureServer/14"
            //     }, 
            //     "id": "2", 
            //     "output": {
            //       "expected": "succeeded"
            //     }
            //   }, 
            //   {
            //     "input": {
            //       "type": "Raster", 
            //       "target": "ElevationLayers", 
            //       "URIs": [
            //         "\\\\yoursharefolder\\NITF\\NITF1.1\\U_0001a.ntf", 
            //         "\\\\yoursharefolder\\NITF\\NITF1.1\\U_0001b.ntf", 
            //         "\\\\yoursharefolder\\NITF\\NITF1.1\\U_0001c.ntf"
            //       ]
            //     }, 
            //     "id": "28", 
            //     "output": {
            //       "expected": "succeeded"
            //     }
            //   }
            //}
            string path = appFolder + "../../../../../TestData/addlayer_testcases.json";

            if(!File.Exists(path))
            {
                return null;
            }

            string content = File.ReadAllText(path);
            JObject j_obj = JObject.Parse(content);

            if (j_obj == null)
            {
                return lyrs;
            }

            JArray children_obj = (JArray)j_obj["testcases"];
            if (children_obj != null)
            {
                lyrs = new List<LayerTestCase>();
                foreach (var child in children_obj)
                { 
                    LayerTestCase lyr = new LayerTestCase();
                    lyr.input = child["input"].ToString();
                    lyr.output = child["output"].ToString();
                    lyr.id = child["id"].ToString();
                    lyr.keyWords = child["output"]["expected"].ToString();
                    lyrs.Add(lyr);
                }
            }
            return lyrs;
        }

        int MaxWaitTime = 15000;

        void AddLayers(List<LayerTestCase> lyrs)
        {
            if (lyrs != null)
            {
                _semaphore.Wait(MaxWaitTime);
                foreach (var lyr in lyrs)
                {
                   _utils.AddLayer(lyr.input);
                   _semaphore.Wait(MaxWaitTime);
                   Assert.IsTrue(_currentMsg.Contains(lyr.keyWords));
                }
            }
        }
        
        [TestMethod]
        public void TestAddLayer()
        {
            _utils = new EarthNamedpipeAPIUtils();
            _utils.OnNotify += GetMessageFromArcGISEarth;
            Connect();
            List<LayerTestCase> lyrs = GetTestCases();
            AddLayers(lyrs);
        }
    }
}
