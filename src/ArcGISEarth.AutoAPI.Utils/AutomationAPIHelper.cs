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
using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArcGISEarth.AutoAPI.Utils
{
    public class AutomationAPIHelper
    {
        private const string TARGET_OPERATIONALLAYERS = "operationalLayers";

        private const string TARGET_BASEMAPS = "baseMaps";

        private const string TARGET_ELEVATIONLAYERS = "elevationLayers";

        private const string API_URL = "http://localhost:8000/api";

        private string _cameraRequestUrl = $"{API_URL}/Camera";

        private string _flightRequestUrl = $"{API_URL}/Flight";

        private string _layerRequestUrl = $"{API_URL}/Layer";

        private string _layersRequestUrl = $"{API_URL}/Layers";

        private string _workspaceRequestUrl = $"{API_URL}/Workspace";

        private string _snapshotRequestUrl = $"{API_URL}/Snapshot";

        HttpClient _httpClient = null;
        public AutomationAPIHelper()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
        }

        public async Task<string> GetCamera()
        {
            try
            {
                HttpResponseMessage responseMessage = await _httpClient.GetAsync(_cameraRequestUrl);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> SetCamera(string inputJsonStr)
        {
            try
            {
                HttpContent putContent = ConverteToHttpContent(inputJsonStr);
                HttpResponseMessage responseMessage = await _httpClient.PutAsync(_cameraRequestUrl, putContent);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> SetFlight(string inputJsonStr)
        {
            try
            {
                HttpContent putContent = ConverteToHttpContent(inputJsonStr);
                HttpResponseMessage responseMessage = await _httpClient.PostAsync(_flightRequestUrl, putContent);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> AddLayer(string inputJsonStr)
        {
            try
            {
                HttpContent putContent = ConverteToHttpContent(inputJsonStr);
                HttpResponseMessage responseMessage = await _httpClient.PostAsync(_layerRequestUrl, putContent);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> GetLayer(string layerId)
        {
            try
            {
                var layerIdUrl = $"{_layerRequestUrl}/{layerId}";
                HttpResponseMessage responseMessage = await _httpClient.GetAsync(layerIdUrl);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> RemoveLayer(string layerId)
        {
            try
            {
                var layerIdUrl = $"{_layerRequestUrl}/{layerId}";
                HttpResponseMessage responseMessage = await _httpClient.DeleteAsync(layerIdUrl);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> ClearLayers(string inputJsonStr)
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
                HttpResponseMessage responseMessage = await _httpClient.DeleteAsync(url);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> GetWorkspace()
        {
            try
            {
                HttpResponseMessage responseMessage = await _httpClient.GetAsync(_workspaceRequestUrl);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> SetWorkspace(string inputJsonStr)
        {
            try
            {
                HttpContent putContent = ConverteToHttpContent(inputJsonStr);
                HttpResponseMessage responseMessage = await _httpClient.PutAsync(_workspaceRequestUrl, putContent);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> ClearWorkspace()
        {
            try
            {
                HttpResponseMessage responseMessage = await _httpClient.DeleteAsync(_workspaceRequestUrl);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> GetSnapshot(string imagePath)
        {
            try
            {
                var uri = new Uri(imagePath);
                if (uri.IsAbsoluteUri && uri.IsFile)
                {
                    var ext = Path.GetExtension(imagePath);
                    if (string.IsNullOrEmpty(ext))
                    {
                        imagePath += ".jpg";
                    }
                    ext = Path.GetExtension(imagePath).ToLower();
                    if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".tif" || ext == ".tiff")
                    {
                        HttpResponseMessage responseMessage = await _httpClient.GetAsync(_snapshotRequestUrl);
                        HttpContent content = responseMessage.Content;                        
                        using (Stream stream = await content.ReadAsStreamAsync())
                        {
                            var image = Image.FromStream(stream);
                            image.Save(imagePath);
                        }                                             
                        return "Save snapshot successful!";
                    }
                    else
                    {
                        return "Save snapshot Failed, Please type correct image format!";
                    }
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

        private HttpContent ConverteToHttpContent(string str)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            var byteArrayContent = new ByteArrayContent(data);
            byteArrayContent.Headers.Add("Content-Type", "application/json");
            return byteArrayContent;
        }

        private async Task<string> GetResponseContent(HttpResponseMessage responseMessage)
        {
            HttpContent content = responseMessage.Content;
            return await content.ReadAsStringAsync();
        }
    }
}
