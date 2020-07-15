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

        public string APIUrl { get; set; }

        public async Task<string> GetCamera()
        {
            try
            {
                string cameraRequestUrl = $"{APIUrl}/Camera";
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage responseMessage = await httpClient.GetAsync(cameraRequestUrl);
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
                string cameraRequestUrl = $"{APIUrl}/Camera";
                HttpClient httpClient = new HttpClient();
                HttpContent putContent = ConvertJsonToHttpContent(inputJsonStr);
                HttpResponseMessage responseMessage = await httpClient.PutAsync(cameraRequestUrl, putContent);
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
                string flightRequestUrl = $"{APIUrl}/Flight";
                HttpClient httpClient = new HttpClient();
                HttpContent postContent = ConvertJsonToHttpContent(inputJsonStr);
                HttpResponseMessage responseMessage = await httpClient.PostAsync(flightRequestUrl, postContent);
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
                string layerRequestUrl = $"{APIUrl}/Layer";
                HttpClient httpClient = new HttpClient();
                HttpContent postContent = ConvertJsonToHttpContent(inputJsonStr);
                HttpResponseMessage responseMessage = await httpClient.PostAsync(layerRequestUrl, postContent);
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
                string layerRequestUrl = $"{APIUrl}/Layer";
                HttpClient httpClient = new HttpClient();
                var layerIdUrl = $"{layerRequestUrl}/{layerId}";
                HttpResponseMessage responseMessage = await httpClient.GetAsync(layerIdUrl);
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
                string layerRequestUrl = $"{APIUrl}/Layer";
                HttpClient httpClient = new HttpClient();
                var layerIdUrl = $"{layerRequestUrl}/{layerId}";
                HttpResponseMessage responseMessage = await httpClient.DeleteAsync(layerIdUrl);
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
                string layersRequestUrl = $"{APIUrl}/Layers";
                JObject jobject = JObject.Parse(inputJsonStr);
                string tartget = jobject["target"].ToString();
                string url = null;
                if (tartget.Equals(TARGET_OPERATIONALLAYERS, StringComparison.OrdinalIgnoreCase))
                {
                    url = $"{layersRequestUrl}/{TARGET_OPERATIONALLAYERS}";
                }
                if (tartget.Equals(TARGET_BASEMAPS, StringComparison.OrdinalIgnoreCase))
                {
                    url = $"{layersRequestUrl}/{TARGET_BASEMAPS}";
                }
                if (tartget.Equals(TARGET_ELEVATIONLAYERS, StringComparison.OrdinalIgnoreCase))
                {
                    url = $"{layersRequestUrl}/{TARGET_ELEVATIONLAYERS}";
                }
                if (url == null)
                {
                    throw new Exception("Please type correct string.");
                }
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage responseMessage = await httpClient.DeleteAsync(url);
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
                string workspaceRequestUrl = $"{APIUrl}/Workspace";
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage responseMessage = await httpClient.GetAsync(workspaceRequestUrl);
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
                string workspaceRequestUrl = $"{APIUrl}/Workspace";
                HttpClient httpClient = new HttpClient();
                HttpContent putContent = ConvertJsonToHttpContent(inputJsonStr);
                HttpResponseMessage responseMessage = await httpClient.PutAsync(workspaceRequestUrl, putContent);
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
                string workspaceRequestUrl = $"{APIUrl}/Workspace";
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage responseMessage = await httpClient.DeleteAsync(workspaceRequestUrl);
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
                string snapshotRequestUrl = $"{APIUrl}/Snapshot";
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
                        HttpClient httpClient = new HttpClient();
                        HttpResponseMessage responseMessage = await httpClient.GetAsync(snapshotRequestUrl);
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
                        return "Save snapshot failed, please type correct image format!";
                    }
                }
                else
                {
                    return "Save snapshot failed, please type correct file path!";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private HttpContent ConvertJsonToHttpContent(string json)
        {
            byte[] data = Encoding.UTF8.GetBytes(json);
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
