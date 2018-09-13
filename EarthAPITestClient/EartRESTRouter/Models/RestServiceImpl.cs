// Copyright 2018 Esri
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

using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Web;
using EarthAPIUtils;

namespace ArcGISEarth.WCFNamedPipeIPC
{
    public class RestServiceImpl : IRestServiceImpl
    {
        private static EarthNamedpipeAPIUtils _utils = null;

        RestServiceImpl()
        {
            if(_utils == null)
            {
                _utils = new EarthNamedpipeAPIUtils();
                _utils.Connect();
            }
        }

        public Stream GetSnapshot()
        {
            string TmpFolderPath = System.IO.Path.GetTempPath();
            TmpFolderPath += "tmp.jpg";
            _utils.GetSnapshot(TmpFolderPath);
            WebOperationContext.Current.OutgoingResponse.ContentType = "image/jpg";
            try
            {
                FileStream f = new FileStream(TmpFolderPath, FileMode.Open);
                int length = (int)f.Length;
                WebOperationContext.Current.OutgoingResponse.ContentLength = length;
                byte[] buffer = new byte[length];
                int sum = 0;
                int count;
                while ((count = f.Read(buffer, sum, length - sum)) > 0)
                {
                    sum += count;
                }
                f.Close();
                MemoryStream ms = new MemoryStream(buffer);
                ms.Position = 0;
                return ms;
            }
            catch (Exception)
            {
            }
            return null;
        }

        public string GetCameraJson()
        {
            return _utils.GetCamera();
        }

        public string FlyTo(string json)
        {
            return _utils.FlyTo(json);
        }

        public string SetCamera(string json)
        {
            return _utils.SetCamera(json);
        }

        public string AddLayer(EarthLayerDescription lyr)
        {
            EarthLayerDescriptionImpl earthLayerDescriptionImpl = new EarthLayerDescriptionImpl(lyr);
            return _utils.AddLayer(earthLayerDescriptionImpl.ToJson());
        }

        public string GetLayerLoadStatus(string layerId)
        {
            string json = "{ \"id\":\"" + layerId + "\"}";
            return _utils.GetLayerLoadStatus(json);
        }

        public string ImportWorkspace(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string json = reader.ReadToEnd();
            string JSONstring = OperationContext.Current.RequestContext.RequestMessage.ToString();
            return _utils.ImportWorkspace(json);
        }

        public string GetWorkspace()
        {
            return _utils.GetWorkspace();
        }

        public string ClearLayers(string target)
        {
            string json = "{ \"target\":\"" + target + "\"}";
            return _utils.ClearLayers(json);
        }

        public string RemoveLayer(string layerId)
        {
            string json = "{ \"id\":\"" + layerId + "\"}";
            return _utils.RemoveLayer(json);
        }
    }
}
