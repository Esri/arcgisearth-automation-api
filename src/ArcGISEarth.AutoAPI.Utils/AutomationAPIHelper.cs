// COPYRIGHT © 2019 ESRI
//
// TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
// Unpublished material - all rights reserved under the
// Copyright Laws of the United States and applicable international
// laws, treaties, and conventions.
//
// For additional information, contact:
// Environmental Systems Research Institute, Inc.
// Attn: Contracts and Legal Services Department
// 380 New York Street
// Redlands, California, 92373
// USA
//
// email: contracts@esri.com

using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.IO;

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

        private RestClient _client = null;

        public AutomationAPIHelper()
        {
            _client = new RestClient();
        }

        public string GetCamera()
        {
            try
            {
                var request = new RestRequest(_cameraRequestUrl, Method.GET);
                request.AddParameter("undefind", _cameraRequestUrl);
                request.AddHeader("accept", "*/*");
                IRestResponse response = _client.Execute(request);
                return response.Content;
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
                var request = new RestRequest(_cameraRequestUrl, Method.PUT);
                request.AddParameter("undefind", _cameraRequestUrl);
                request.AddHeader("accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefind", inputJsonStr, ParameterType.RequestBody);
                IRestResponse response = _client.Execute(request);
                return response.Content;
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
                var request = new RestRequest(_flightRequestUrl, Method.POST);
                request.AddParameter("undefind", _flightRequestUrl);
                request.AddHeader("accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefind", inputJsonStr, ParameterType.RequestBody);
                IRestResponse response = _client.Execute(request);
                return response.Content;
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
                var request = new RestRequest(_layerRequestUrl, Method.POST);
                request.AddParameter("undefind", _layerRequestUrl);
                request.AddHeader("accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefind", inputJsonStr, ParameterType.RequestBody);
                IRestResponse response = _client.Execute(request);
                return response.Content;
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
                var request = new RestRequest(layerIdUrl, Method.GET);
                request.AddParameter("undefind", layerIdUrl);
                request.AddHeader("accept", "*/*");
                IRestResponse response = _client.Execute(request);
                return response.Content;
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
                string layerIdUrl = $"{_layerRequestUrl}/{layerId}";
                var request = new RestRequest(layerIdUrl, Method.DELETE);
                request.AddParameter("undefind", layerIdUrl);
                request.AddHeader("accept", "*/*");
                IRestResponse response = _client.Execute(request);
                return response.Content;
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
                RestRequest request = null;
                if (tartget.Equals(TARGET_OPERATIONALLAYERS, StringComparison.OrdinalIgnoreCase))
                {
                    request = new RestRequest($"{_layersRequestUrl}/{TARGET_OPERATIONALLAYERS}", Method.DELETE);
                }
                if (tartget.Equals(TARGET_BASEMAPS, StringComparison.OrdinalIgnoreCase))
                {
                    request = new RestRequest($"{_layersRequestUrl}/{TARGET_BASEMAPS}", Method.DELETE);
                }
                if (tartget.Equals(TARGET_ELEVATIONLAYERS, StringComparison.OrdinalIgnoreCase))
                {
                    request = new RestRequest($"{_layersRequestUrl}/{TARGET_ELEVATIONLAYERS}", Method.DELETE);
                }
                if (request == null)
                {
                    throw new Exception("Please type correct string");
                }
                request.AddParameter("undefind", _snapshotRequestUrl);
                request.AddHeader("accept", "*/*");
                IRestResponse response = _client.Execute(request);
                return response.Content;
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
                var request = new RestRequest(_snapshotRequestUrl, Method.GET);
                request.AddParameter("undefind", _snapshotRequestUrl);
                request.AddHeader("accept", "*/*");
                request.AddHeader("Content-Type", "image/jpeg");
                IRestResponse response = _client.Execute(request);
                Uri uri = new Uri(imagePath);
                if (uri.IsAbsoluteUri && uri.IsFile)
                {
                    using (var imageFile = new FileStream(imagePath, FileMode.Create))
                    {
                        byte[] bytes = response.RawBytes;
                        imageFile.Write(bytes, 0, bytes.Length);
                        imageFile.Flush();
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
                var request = new RestRequest(_workspaceRequestUrl, Method.GET);
                request.AddParameter("undefind", _workspaceRequestUrl);
                request.AddHeader("accept", "*/*");
                request.AddHeader("Content-Type", "image/jpeg");
                IRestResponse response = _client.Execute(request);
                return response.Content;
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
                var request = new RestRequest(_workspaceRequestUrl, Method.PUT);
                request.AddParameter("undefind", _workspaceRequestUrl);
                request.AddHeader("accept", "*/*");
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("undefind", inputJsonStr, ParameterType.RequestBody);
                IRestResponse response = _client.Execute(request);
                return response.Content;
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
                var request = new RestRequest(_workspaceRequestUrl, Method.DELETE);
                IRestResponse response = _client.Execute(request);
                return response.Content;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
