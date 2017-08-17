// Copyright 2017 Esri
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using EarthAPIUtils;

namespace EarthAPIUnitTest
{
    [TestClass]
    public class CameraTest
    {
        private EarthNamedpipeAPIUtils _utils;

        private async Task<string> Connect()
        {
            return await _utils.Connect();
        }

        [TestMethod]
        public void TestGetCameraJson()
        {
            if(_utils == null)
            {
                _utils = new EarthNamedpipeAPIUtils();
                string rlt = Connect().Result;
                Assert.AreEqual(EarthNamedpipeAPIUtils.cSuccess, rlt);
            }
            double lng;
            double lat;
            double alt;
            double heading;
            double pitch;
            int wkid;
            string cameraJson = _utils.GetCamera();
            Assert.IsTrue(EarthNamedpipeAPIUtils.PaserCameraJson(cameraJson, out lng, out lat, out alt, out heading, out pitch, out wkid));
        }


        [TestMethod]
        public void TestSetCamera()
        {
            if(_utils == null)
            {
                _utils = new EarthNamedpipeAPIUtils();
                string rlt = Connect().Result;
                Assert.AreEqual(EarthNamedpipeAPIUtils.cSuccess, rlt);
            }

            double lng = 100;
            double lat = 10;
            double alt = 5000000;
            double heading = 0;
            double pitch = 0;
            int wkid = 4326;
            string cameraJson = EarthNamedpipeAPIUtils.ConstructCameraJson(lng, lat, alt, heading, pitch, wkid);
            Assert.AreEqual(EarthNamedpipeAPIUtils.cSuccess, _utils.SetCamera(cameraJson));
        }
    }
}
