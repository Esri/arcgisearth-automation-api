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
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ArcGISEarth.AutoAPI.Utils
{
    public static class AutomationAPIHelper
    {
        private const string CAMERA_CONTROLLER_NAME = "camera";
        private const string FLIGHT_CONTROLLER_NAME = "flight";
        private const string LAYER_CONTROLLER_NAME = "layer";
        private const string LAYERS_CONTROLLER_NAME = "layers";
        private const string WORKSPACE_CONTROLLER_NAME = "workspace";
        private const string SNAPSHOT_CONTROLLER_NAME = "snapshot";
        private const string DEFAULT_BASEURL = "http://localhost:8000";
        private const string END_POINT = "/arcgisearth";

        #region Automation API Functions
        /// <summary>
        /// Get or set the base URL of the Earth Automation API.
        /// </summary>
        public static string APIBaseUrl { get; set; } = GetBaseUrl();

        /// <summary>
        /// Get the camera information.
        /// </summary>
        /// <returns>The current camera information in JSON format.</returns>
        /// JSON response example:
        /// {
        ///     "position": {
        ///         "x": -92,
        ///         "y": 41,
        ///         "z": 11000000,
        ///         "spatialReference": {
        ///             "wkid": 4326
        ///         }
        ///     },
        ///     "heading": 2.3335941892764884E-17,
        ///     "tilt": 6.144145559063083E-15,
        ///     "roll": 0
        /// }
        public static async Task<string> GetCamera()
        {
            try
            {
                string cameraRequestUrl = $"{APIBaseUrl}/{CAMERA_CONTROLLER_NAME}";
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage responseMessage = await httpClient.GetAsync(cameraRequestUrl);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Set the camera information.
        /// </summary>
        /// <param name="inputJsonStr">The parameters in JSON format.</param>
        /// JSON parameters example:
        /// {
        ///     "camera": {
        ///         "position": {
        ///             "x": -92,
        ///             "y": 41,
        ///             "z": 11000000,
        ///             "spatialReference": {
        ///                 "wkid": 4326
        ///             }
        ///         },
        ///         "heading": 2.3335941892764884E-17,
        ///         "tilt": 6.144145559063083E-15,
        ///         "roll": 0
        ///     },
        ///     "duration": 2
        /// }
        /// <returns>Automation API response message.</returns>
        public static async Task<string> SetCamera(string inputJsonStr)
        {
            try
            {
                string cameraRequestUrl = $"{APIBaseUrl}/{CAMERA_CONTROLLER_NAME}";
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

        /// <summary>
        /// Fly-to a target position
        /// </summary>
        /// <param name="inputJsonStr">The parameters in JSON format.</param>
        /// JSON parameters example:
        /// {
        ///     "camera": {
        ///         "position": {
        ///             "x": -92,
        ///             "y": 41,
        ///             "z": 11000000,
        ///             "spatialReference": {
        ///                 "wkid": 4326
        ///             }
        ///         },
        ///         "heading": 2.3335941892764884E-17,
        ///         "tilt": 6.144145559063083E-15,
        ///         "roll": 0
        ///     },
        ///     "duration": 2
        /// }
        /// <returns>Automation API response message.</returns>
        public static async Task<string> SetFlight(string inputJsonStr)
        {
            try
            {
                string flightRequestUrl = $"{APIBaseUrl}/{FLIGHT_CONTROLLER_NAME}";
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

        /// <summary>
        /// Add a layer to Earth.
        /// </summary>
        /// <param name="inputJsonStr">The parameters in JSON format.</param>
        /// JSON parameters example:
        /// {
        ///     "URI": "https://www.arcgis.com/home/item.html?id=19dcff93eeb64f208d09d328656dd492",
        ///     "target": "operationalLayers",
        ///     "type": "PortalItem"
        /// }
        /// <returns>Automation API response message.</returns>
        public static async Task<string> AddLayer(string inputJsonStr)
        {
            try
            {
                string layerRequestUrl = $"{APIBaseUrl}/{LAYER_CONTROLLER_NAME}";
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

        /// <summary>
        /// Get layer information by id.
        /// </summary>
        /// <param name="layerId">The layer id.</param>
        /// <returns>The layer information in JSON format.</returns>
        /// JSON response example:
        /// {
        ///     "displayName": "Visualize New Developments",
        ///     "isVisible": true,
        ///     "classType": "ArcGISScene",
        ///     "id": "311b7317-94f8-4f80-89f2-0e3ca5e77d28",
        ///     "sourceURI": "https://www.arcgis.com/sharing/rest/content/items/19dcff93eeb64f208d09d328656dd492",
        ///     "loadStatus": "Loaded"
        /// }
        public static async Task<string> GetLayer(string layerId)
        {
            try
            {
                string layerRequestUrl = $"{APIBaseUrl}/{LAYER_CONTROLLER_NAME}";
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

        /// <summary>
        /// Delete a layer by id.
        /// </summary>
        /// <param name="layerId">The layer id.</param>
        /// <returns>Automation API response message.</returns>
        public static async Task<string> RemoveLayer(string layerId)
        {
            try
            {
                string layerRequestUrl = $"{APIBaseUrl}/{LAYER_CONTROLLER_NAME}";
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

        /// <summary>
        /// Remove layers from the ArcGIS Earth workspace by target.
        /// </summary>
        /// <param name="targetType">Specifies the target place where the layers are deleted. Values are operationalLayers, baseMaps or elevationLayers</param>
        /// <returns>Automation API response message.</returns>
        public static async Task<string> ClearLayers(string targetType)
        {
            try
            {
                string layersRequestUrl = $"{APIBaseUrl}/{LAYERS_CONTROLLER_NAME}/{targetType}";
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage responseMessage = await httpClient.DeleteAsync(layersRequestUrl);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Get workspace contents in a .zip format.
        /// </summary>
        /// <returns>Automation API response message.</returns>
        /// JSON response example:
        /// {
        ///     "url": "http://localhost:8000/workspaces/4855c0d4-9b11-4832-876b-ee3a3730dfdb.zip",
        ///     "path": "C:\\Users\\Username\\Documents\\ArcGISEarth\\automation\\workspaces\\4855c0d4-9b11-4832-876b-ee3a3730dfdb.zip"
        /// }
        public static async Task<string> GetWorkspace()
        {
            try
            {
                string workspaceRequestUrl = $"{APIBaseUrl}/{WORKSPACE_CONTROLLER_NAME}";
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage responseMessage = await httpClient.GetAsync(workspaceRequestUrl);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Import a workspace in .zip format into ArcGIS Earth.
        /// </summary>
        /// <param name="inputJsonStr">The parameters in JSON format.</param>
        /// JSON parameters example:
        /// {
        ///   "url": "http://localhost:8000/workspaces/4855c0d4-9b11-4832-876b-ee3a3730dfdb.zip",
        ///   "path": "C:\\Users\\Username\\Documents\\ArcGISEarth\\automation\\workspaces\\4855c0d4-9b11-4832-876b-ee3a3730dfdb.zip"
        /// }
        /// <returns>Automation API response message.</returns>
        public static async Task<string> ImportWorkspace(string inputJsonStr)
        {
            try
            {
                string workspaceRequestUrl = $"{APIBaseUrl}/{WORKSPACE_CONTROLLER_NAME}";
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

        /// <summary>
        /// Remove the current workspace from ArcGIS Earth.
        /// </summary>
        /// <returns>Automation API response message.</returns>
        public static async Task<string> ClearWorkspace()
        {
            try
            {
                string workspaceRequestUrl = $"{APIBaseUrl}/{WORKSPACE_CONTROLLER_NAME}";
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage responseMessage = await httpClient.DeleteAsync(workspaceRequestUrl);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Take a snapshot of the current view of ArcGIS Earth. The image bytes are directly streamed to the client.
        /// </summary>
        /// <returns>The image source.</returns>
        public static async Task<ImageSource> TakeSnapshot()
        {
            try
            {
                string snapshotRequestUrl = $"{APIBaseUrl}/{SNAPSHOT_CONTROLLER_NAME}";
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage responseMessage = await httpClient.GetAsync(snapshotRequestUrl);
                HttpContent content = responseMessage.Content;
                BitmapImage bmpImg = new BitmapImage();
                using (Stream stream = await content.ReadAsStreamAsync())
                {
                    bmpImg.BeginInit();
                    bmpImg.StreamSource = stream;
                    bmpImg.EndInit();
                }
                return bmpImg;
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region private methods
        private static HttpContent ConvertJsonToHttpContent(string json)
        {
            byte[] data = Encoding.UTF8.GetBytes(json);
            var byteArrayContent = new ByteArrayContent(data);
            byteArrayContent.Headers.Add("Content-Type", "application/json");
            return byteArrayContent;
        }

        private static async Task<string> GetResponseContent(HttpResponseMessage responseMessage)
        {
            HttpContent content = responseMessage.Content;
            return await content.ReadAsStringAsync();
        }

        private static string DefaultWebRootFolder
        {
            get
            {
                string myFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (!myFolder.EndsWith("\\"))
                {
                    myFolder += "\\";
                }
                myFolder += "ArcGISEarth\\automation\\";

                if (!Directory.Exists(myFolder))
                {
                    try
                    {
                        Directory.CreateDirectory(myFolder);
                    }
                    catch { }
                }
                return myFolder;
            }
        }

        private static string GetBaseUrl()
        {
            string baseUrl = null;

            try
            {
                string expectedSettingPath = Path.Combine(DefaultWebRootFolder, "settings.json");

                if (File.Exists(expectedSettingPath))
                {
                    string json = File.ReadAllText(expectedSettingPath);
                    JObject setting = JObject.Parse(json);
                    if (setting != null)
                    {
                        baseUrl = setting["baseUrl"].ToString();
                    }
                    else
                    {
                        return DEFAULT_BASEURL + END_POINT;
                    }
                }
                else
                {
                    return DEFAULT_BASEURL + END_POINT;
                }

                return baseUrl + END_POINT;
            }
            catch
            {
                return DEFAULT_BASEURL + END_POINT;
            }
        }
        #endregion
    }
}
