using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ArcGISEarth.AutoAPI.Utils
{
    public class AutomationAPIHelper
    {
        private RestClient _client = null;

        private const string API_URL = "http://localhost:8000/api";
        private string _camera_request_url = $"{API_URL}/Camera";
        private string _flight_request_url = $"{API_URL}/Flight";
        private string _snapshot_request_url = $"{API_URL}/Snapshot";
        private string _workspace_request_url = $"{API_URL}/Workspace";

        public AutomationAPIHelper()
        {
            _client = new RestClient();
        }

        public string GetCamera()
        {
            var request = new RestRequest(_camera_request_url, Method.GET);
            request.AddParameter("undefind", _camera_request_url);
            request.AddHeader("accept", "*/*");
            IRestResponse response = _client.Execute(request);
            return response.Content;
        }

        public string SetCamera(string inputJsonStr)
        {
            var request = new RestRequest(_camera_request_url, Method.PUT);
            request.AddParameter("undefind", _camera_request_url);
            request.AddHeader("accept", "*/*");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefind", inputJsonStr, ParameterType.RequestBody);
            IRestResponse response = _client.Execute(request);
            return response.Content;
        }

        public string FlyTo(string inputJsonStr)
        {
            var request = new RestRequest(_flight_request_url, Method.POST);
            request.AddParameter("undefind", _flight_request_url);
            request.AddHeader("accept", "*/*");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefind", inputJsonStr, ParameterType.RequestBody);
            IRestResponse response = _client.Execute(request);
            return response.Content;
        }

        // 1. To Do : Layer
        // 2. Save image


        public string GetSnapshot(string imagePath)
        {
            var request = new RestRequest(_snapshot_request_url, Method.GET);
            request.AddParameter("undefind", _snapshot_request_url);
            request.AddHeader("accept", "*/*");
            request.AddHeader("Content-Type", "image/jpeg");
            IRestResponse response = _client.Execute(request);
            StringToImage(response.Content, imagePath);           
            if (File.Exists(imagePath))
            {
                return "Save snapshot successful!";
            }
            else
            {
                return "Save snapshot Failed!";
            }
        }

        public string GetWorkspace()
        {
            var request = new RestRequest(_workspace_request_url, Method.GET);
            request.AddParameter("undefind", _workspace_request_url);
            request.AddHeader("accept", "*/*");
            request.AddHeader("Content-Type", "image/jpeg");
            IRestResponse response = _client.Execute(request);
            return response.Content;
        }

        public string ImportWorkspace(string inputJsonStr)
        {
            var request = new RestRequest(_workspace_request_url, Method.PUT);
            request.AddParameter("undefind", _workspace_request_url);
            request.AddHeader("accept", "*/*");
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("undefind", inputJsonStr, ParameterType.RequestBody);
            IRestResponse response = _client.Execute(request);
            return response.Content;
        }

        public string DeleteWorkspace()
        {
            var request = new RestRequest(_workspace_request_url, Method.DELETE);
            IRestResponse response = _client.Execute(request);
            return response.Content;
        }

        private void StringToImage(string inputString, string imagePath)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            using (var imageFile = new FileStream(imagePath, FileMode.Create))
            {
                imageFile.Write(bytes, 0, bytes.Length);
                imageFile.Flush();
            }
        }
    }
}
