// Copyright 2020 Esri
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

using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;

namespace ArcGISEarth.AutoAPI.Utils
{
    public class AutomationAPIHelper
    {
        private const string TARGET_OPERATIONALLAYERS = "OperationalLayers";

        private const string TARGET_BASEMAPS = "BasemapLayers";

        private const string TARGET_ELEVATIONLAYERS = "ElevationLayers";

        private const string API_URL = "http://localhost:8000/api";

        private string _cameraRequestUrl = $"{API_URL}/Camera";

        private string _flightRequestUrl = $"{API_URL}/Flight";

        private string _layerRequestUrl = $"{API_URL}/Layer";

        private string _layersRequestUrl = $"{API_URL}/Layers";

        private string _workspaceRequestUrl = $"{API_URL}/Workspace";

        private string _snapshotRequestUrl = $"{API_URL}/Snapshot";        

        public AutomationAPIHelper()
        {            
        }

        public string GetCamera()
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(_cameraRequestUrl);
                request.Method = "GET";
                request.Accept = "*/*";
                return GetResponseContent(request);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string SetCamera(string inputJsonStr)
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(_cameraRequestUrl);
                request.Method = "PUT";
                request.Accept = "*/*";
                request.ContentType = "application/json";
                byte[] data = Encoding.UTF8.GetBytes(inputJsonStr);
                request.ContentLength = data.Length;
                request.GetRequestStream().Write(data, 0, data.Length);
                request.GetRequestStream().Close();
                return GetResponseContent(request);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string SetFlight(string inputJsonStr)
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(_flightRequestUrl);
                request.Method = "POST";
                request.Accept = "*/*";
                request.ContentType = "application/json";
                byte[] data = Encoding.UTF8.GetBytes(inputJsonStr);
                request.ContentLength = data.Length;
                request.GetRequestStream().Write(data, 0, data.Length);
                request.GetRequestStream().Close();
                return GetResponseContent(request);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string AddLayer(string inputJsonStr)
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(_layerRequestUrl);
                request.Method = "POST";
                request.Accept = "*/*";
                request.ContentType = "application/json";
                byte[] data = Encoding.UTF8.GetBytes(inputJsonStr);
                request.ContentLength = data.Length;
                request.GetRequestStream().Write(data, 0, data.Length);
                request.GetRequestStream().Close();
                return GetResponseContent(request);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string GetLayer(string layerId)
        {
            try
            {
                var layerIdUrl = $"{_layerRequestUrl}/{layerId}";
                HttpWebRequest request = WebRequest.CreateHttp(layerIdUrl);
                request.Method = "GET";
                request.Accept = "*/*";
                return GetResponseContent(request);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string RemoveLayer(string layerId)
        {
            try
            {
                var layerIdUrl = $"{_layerRequestUrl}/{layerId}";
                HttpWebRequest request = WebRequest.CreateHttp(layerIdUrl);
                request.Method = "DELETE";
                request.Accept = "*/*";
                return GetResponseContent(request);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string ClearLayers(string inputJsonStr)
        {
            try
            {
                JObject jobject = JObject.Parse(inputJsonStr);
                string tartget = jobject["target"].ToString();
                string url = null;
                if (tartget.Equals(TARGET_OPERATIONALLAYERS, StringComparison.OrdinalIgnoreCase))
                {
                    url = $"{_layersRequestUrl}/{TARGET_OPERATIONALLAYERS}";
                }
                if (tartget.Equals(TARGET_BASEMAPS, StringComparison.OrdinalIgnoreCase))
                {
                    url = $"{_layersRequestUrl}/{TARGET_BASEMAPS}";
                }
                if (tartget.Equals(TARGET_ELEVATIONLAYERS, StringComparison.OrdinalIgnoreCase))
                {
                    url = $"{_layersRequestUrl}/{TARGET_ELEVATIONLAYERS}";
                }
                if (url == null)
                {
                    throw new Exception("Please type correct string");
                }
                HttpWebRequest request = WebRequest.CreateHttp(url);
                request.Method = "DELETE";
                request.Accept = "*/*";
                return GetResponseContent(request);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string GetSnapshot(string imagePath)
        {
            try
            {
                Uri uri = new Uri(imagePath);
                if (uri.IsAbsoluteUri && uri.IsFile)
                {
                    HttpWebRequest request = WebRequest.CreateHttp(_workspaceRequestUrl);
                    request.Method = "GET";
                    request.Accept = "*/*";
                    var httpResponse = (HttpWebResponse)request.GetResponse();                    
                    Stream st = httpResponse.GetResponseStream();
                    using (FileStream file = new FileStream("file.bin", FileMode.Create, System.IO.FileAccess.Write))
                    {
                        byte[] bytes = new byte[st.Length];
                        st.Read(bytes, 0, (int)st.Length);
                        file.Write(bytes, 0, bytes.Length);
                        st.Close();
                    }
                    return "Save snapshot successful!";
                }
                else
                {
                    return "Save snapshot Failed, Please type correct file path!";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string GetWorkspace()
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(_workspaceRequestUrl);
                request.Method = "GET";
                request.Accept = "*/*";
                return GetResponseContent(request);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string SetWorkspace(string inputJsonStr)
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(_workspaceRequestUrl);
                request.Method = "PUT";
                request.Accept = "*/*";
                request.ContentType = "application/json";
                byte[] data = Encoding.UTF8.GetBytes(inputJsonStr);
                request.ContentLength = data.Length;
                request.GetRequestStream().Write(data, 0, data.Length);
                request.GetRequestStream().Close();
                return GetResponseContent(request);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string ClearWorkspace()
        {
            try
            {
                HttpWebRequest request = WebRequest.CreateHttp(_workspaceRequestUrl);
                request.Method = "DELETE";
                request.Accept = "*/*";
                return GetResponseContent(request);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string GetResponseContent(HttpWebRequest request)
        {
            var httpResponse = (HttpWebResponse)request.GetResponse();
            using (Stream webStream = httpResponse.GetResponseStream())
            {
                var responseReader = new StreamReader(webStream);
                return responseReader.ReadToEnd();
            }
        }
    }
}
