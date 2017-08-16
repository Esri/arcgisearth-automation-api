using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading; //SemaphoreSlim
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
