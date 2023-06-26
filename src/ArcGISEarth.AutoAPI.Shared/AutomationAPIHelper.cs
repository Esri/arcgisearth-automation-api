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

using System.Text.Json.Nodes;
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
        private const string GRAPHIC_CONTROLLER_NAME = "graphics";
        private const string DRAWING_CONTROLLER_NAME = "drawings";
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
                HttpResponseMessage responseMessage = await httpClient.GetAsync(cameraRequestUrl).ConfigureAwait(false);
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
                HttpResponseMessage responseMessage = await httpClient.PutAsync(cameraRequestUrl, putContent).ConfigureAwait(false);
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
                HttpResponseMessage responseMessage = await httpClient.PostAsync(flightRequestUrl, postContent).ConfigureAwait(false);
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
                HttpResponseMessage responseMessage = await httpClient.PostAsync(layerRequestUrl, postContent).ConfigureAwait(false);
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
                HttpResponseMessage responseMessage = await httpClient.GetAsync(layerIdUrl).ConfigureAwait(false);
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
                HttpResponseMessage responseMessage = await httpClient.DeleteAsync(layerIdUrl).ConfigureAwait(false);
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
                HttpResponseMessage responseMessage = await httpClient.DeleteAsync(layersRequestUrl).ConfigureAwait(false);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Add a graphic to Earth.
        /// </summary>
        /// <param name="inputJsonStr">The parameters in JSON format.</param>
        /// JSON parameters example:
        /// {
        ///     "geometry": {
        ///         "type": "point",
        ///         "x": -100,
        ///         "y": 40
        ///     },
        ///     "symbol": {
        ///         "type": "picture-marker",
        ///         "url": "https://static.arcgis.com/images/Symbols/Shapes/BlackStarLargeB.png",
        ///         "width": "64px",
        ///         "height": "64px",
        ///         "xoffset": "10px",
        ///         "yoffset": "10px"
        ///     }
        /// }
        /// <returns>Automation API response message.</returns>
        public static async Task<string> AddGraphic(string inputJsonStr)
        {
            try
            {
                string graphicRequestUrl = $"{APIBaseUrl}/{GRAPHIC_CONTROLLER_NAME}";
                HttpClient httpClient = new HttpClient();
                HttpContent postContent = ConvertJsonToHttpContent(inputJsonStr);
                HttpResponseMessage responseMessage = await httpClient.PostAsync(graphicRequestUrl, postContent).ConfigureAwait(false);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Get graphic information by id.
        /// </summary>
        /// <param name="graphicId">The graphic id.</param>
        /// <returns>Automation API response message.</returns>
        public static async Task<string> GetGraphic(string graphicId)
        {
            try
            {
                string graphicRequestUrl = $"{APIBaseUrl}/{GRAPHIC_CONTROLLER_NAME}";
                HttpClient httpClient = new HttpClient();
                var graphicIdUrl = $"{graphicRequestUrl}/{graphicId}";
                HttpResponseMessage responseMessage = await httpClient.GetAsync(graphicIdUrl).ConfigureAwait(false);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Update a graphic in Earth.
        /// </summary>
        /// <param name="inputJsonStr">The parameters in JSON format.</param>
        /// JSON parameters example:
        /// {
        ///     "id": "311b7317-94f8-4f80-89f2-0e3ca5e77d28",
        ///     "geometry": {
        ///         "type": "point",
        ///         "x": -100,
        ///         "y": 40
        ///     },
        ///     "symbol": {
        ///         "type": "picture-marker",
        ///         "url": "https://static.arcgis.com/images/Symbols/Basic/RedSphere.png",
        ///         "width": "64px",
        ///         "height": "64px",
        ///         "xoffset": "10px",
        ///         "yoffset": "10px"
        ///     }
        /// }
        /// <returns>Automation API response message. If success, the content is null.</returns>
        public static async Task<string> UpdateGraphic(string inputJsonStr)
        {
            try
            {
                string graphicRequestUrl = $"{APIBaseUrl}/{GRAPHIC_CONTROLLER_NAME}";
                HttpClient httpClient = new HttpClient();
                HttpContent postContent = ConvertJsonToHttpContent(inputJsonStr);
                HttpResponseMessage responseMessage = await httpClient.PatchAsync(graphicRequestUrl, postContent).ConfigureAwait(false);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Delete a graphic by id.
        /// </summary>
        /// <param name="graphicId">The graphic id.</param>
        /// <returns>Automation API response message. If success, the content is null.</returns>
        public static async Task<string> RemoveGraphic(string graphicId)
        {
            try
            {
                string graphicRequestUrl = $"{APIBaseUrl}/{GRAPHIC_CONTROLLER_NAME}";
                HttpClient httpClient = new HttpClient();
                var graphicIdUrl = $"{graphicRequestUrl}/{graphicId}";
                HttpResponseMessage responseMessage = await httpClient.DeleteAsync(graphicIdUrl).ConfigureAwait(false);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Remove all graphics from ArcGIS Earth.
        /// </summary>
        /// <returns>Automation API response message.</returns>
        public static async Task<string> ClearGraphics()
        {
            try
            {
                string graphicsRequestUrl = $"{APIBaseUrl}/{GRAPHIC_CONTROLLER_NAME}";
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage responseMessage = await httpClient.DeleteAsync(graphicsRequestUrl).ConfigureAwait(false);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Add a drawing element to Earth.
        /// </summary>
        /// <param name="inputJsonStr">The parameters in JSON format.</param>
        /// JSON parameters example:
        /// {
        ///     "id": "8a1701c9-b8e1-1b0a-c1a7-ac6242c7645e",
        ///     "visible": true,
        ///     "title": "Point",
        ///     "geometry": {
        ///         "x": -100,
        ///         "y": 40,
        ///         "spatialReference": {
        ///             "wkid": 4326
        ///         }
        ///     },
        ///     "symbol": {
        ///         "type": "picture-marker",
        ///         "url": "https://static.arcgis.com/images/Symbols/Shapes/BlackStarLargeB.png",
        ///         "size": "32px"
        ///     },
        ///     "labelSymbol": {
        ///         "type": "text",
        ///         "color": [
        ///             100,
        ///             100,
        ///             100,
        ///             255
        ///         ],
        ///         "font": {
        ///             "size": "16px"
        ///         }
        ///     }
        /// }
        /// <returns>Automation API response message.</returns>
        public static async Task<string> AddDrawing(string inputJsonStr)
        {
            try
            {
                string drawingsRequestUrl = $"{APIBaseUrl}/{DRAWING_CONTROLLER_NAME}";
                HttpClient httpClient = new HttpClient();
                HttpContent postContent = ConvertJsonToHttpContent(inputJsonStr);
                HttpResponseMessage responseMessage = await httpClient.PostAsync(drawingsRequestUrl, postContent).ConfigureAwait(false);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Delete a drawing element by id.
        /// </summary>
        /// <param name="drawingId">The drawing element id.</param>
        /// <returns>Automation API response message. If success, the content is null.</returns>
        public static async Task<string> RemoveDrawing(string drawingId)
        {
            try
            {
                string drawingsRequestUrl = $"{APIBaseUrl}/{DRAWING_CONTROLLER_NAME}";
                HttpClient httpClient = new HttpClient();
                var drawingIdUrl = $"{drawingsRequestUrl}/{drawingId}";
                HttpResponseMessage responseMessage = await httpClient.DeleteAsync(drawingIdUrl).ConfigureAwait(false);
                return await GetResponseContent(responseMessage);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Remove all graphics from ArcGIS Earth.
        /// </summary>
        /// <returns>Automation API response message.</returns>
        public static async Task<string> ClearDrawings()
        {
            try
            {
                string drawingsRequestUrl = $"{APIBaseUrl}/{DRAWING_CONTROLLER_NAME}";
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage responseMessage = await httpClient.DeleteAsync(drawingsRequestUrl).ConfigureAwait(false);
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
                HttpResponseMessage responseMessage = await httpClient.GetAsync(workspaceRequestUrl).ConfigureAwait(false);
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
                HttpResponseMessage responseMessage = await httpClient.PutAsync(workspaceRequestUrl, putContent).ConfigureAwait(false);
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
                HttpResponseMessage responseMessage = await httpClient.DeleteAsync(workspaceRequestUrl).ConfigureAwait(false);
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
                using (Stream stream = await content.ReadAsStreamAsync().ConfigureAwait(false))
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
                    JsonObject setting = JsonNode.Parse(json).AsObject();
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

    //Refer to https://stackoverflow.com/a/29772349/5765982
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content)
        {
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, new Uri(requestUri))
            {
                Content = content
            };

            HttpResponseMessage response = new HttpResponseMessage();
            response = await client.SendAsync(request);

            return response;
        }
    }
}
