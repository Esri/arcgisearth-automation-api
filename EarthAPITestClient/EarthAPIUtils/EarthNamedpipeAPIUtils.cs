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

using ArcGISEarth.WCFNamedPipeIPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EarthAPIUtils
{
    public class MessageStringEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public class EarthNamedpipeAPIUtils : IEarthNamedpipeCallbackService
    {
        public const string c_processName = "ArcGISEarth";
        public const string cBasePipeAddress = "net.pipe://localhost/arcgisearth";
        public const string cNeedConnect = "Please start earth then connect to it";
        public const string cSuccess = "Success";
        public const string cFailed = "Failed";
        public const int cMaxBuffer = 2147483647;
        public const string cConnectError = "Something wrong with the namedpipe communication!";
        public const string cWaitAddingLayer = "Waiting for the result of adding layer";

        private IEarthNamedpipeService _channel = null;
        private ChannelFactory<IEarthNamedpipeService> _factory = null;
        public EventHandler OnNotify;

        static public string ConstructLayerInformationJson(string uri, string lyrType = null, string target = null)
        {
            try
            {
                JObject lyrInfoObj = new JObject();
                if (string.IsNullOrEmpty(uri))
                {
                    return null;
                }

                lyrInfoObj["uri"] = uri;
                if (!string.IsNullOrEmpty(lyrType))
                {
                    lyrInfoObj["type"] = lyrType;
                }

                if (!string.IsNullOrEmpty(target))
                {
                    lyrInfoObj["target"] = target;
                }

                return lyrInfoObj.ToString(Formatting.None);
            }
            catch
            {
                return null;
            }

        }

        static public string ConstructLayerInformationJson(string[] uris, string lyrType = null, string target = null)
        {
            try
            {
                JObject lyrInfoObj = new JObject();
                if(uris == null || uris.Length == 0)
                {
                    return null;
                }

                JArray uriArray = new JArray();
                foreach(string uri in uris)
                {
                    uriArray.Add(uri);
                }

                lyrInfoObj["uris"] = uriArray;
                if (!string.IsNullOrEmpty(lyrType))
                {
                    lyrInfoObj["type"] = lyrType;
                }

                if (!string.IsNullOrEmpty(target))
                {
                    lyrInfoObj["target"] = target;
                }

                return lyrInfoObj.ToString(Formatting.None);
            }
            catch
            {
                return null;
            }
        }

        static public string ConstructCameraJson(double lng, double lat, double alt, double heading, double pitch, int wkid = 4326)
        {
            try
            {
                JObject mapPointObj = new JObject();
                mapPointObj["x"] = lng;
                mapPointObj["y"] = lat;
                mapPointObj["z"] = alt;
                JObject srObj = new JObject();
                srObj["wkid"] = wkid;
                mapPointObj["spatialReference"] = srObj;
                JObject cameraObj = new JObject();
                cameraObj["mapPoint"] = mapPointObj;
                cameraObj["heading"] = heading;
                cameraObj["pitch"] = pitch;
                return cameraObj.ToString(Formatting.None);
            }
            catch
            {
                return null;
            }
        }

        static public bool PaserCameraJson(
            string cameraJson,
            out double lng, out double lat, out double alt, 
            out double heading, out double pitch, 
            out int wkid)
        {
            lng = lat = alt = heading = pitch = wkid = 0;

            try
            {
                JObject cameraObj = JObject.Parse(cameraJson);

                if (cameraObj == null)
                {
                    return false;
                }

                string mapPointJson = cameraObj["mapPoint"].ToString();
                JObject mapPointObj = JObject.Parse(mapPointJson);
                if (mapPointObj["spatialReference"] != null)
                {
                    if(mapPointObj["spatialReference"]["wkid"] != null)
                    {
                        wkid = (int)mapPointObj["spatialReference"]["wkid"];
                    }
                }

                lng = (double)mapPointObj["x"];
                lat = (double)mapPointObj["y"];
                alt = (double)mapPointObj["z"];
                heading = (double)cameraObj["heading"];
                pitch = (double)cameraObj["pitch"];

                return true;
            }
            catch
            {
                return false;
            }
        }


        public void CloseConnect()
        {
            if(_factory != null)
            {
                _factory.Close();
                _factory = null;
            }
            _channel = null;
        }

        private bool ProcessStdOutCallBack(string message, ref string address)
        {
            if (!String.IsNullOrEmpty(message) && message.Contains("net.pipe"))
            {
                address = message;
                return true;
            }
            return false;
        }

        public void Notify(string message)
        {
            OnNotify?.Invoke(this, new MessageStringEventArgs() { Message = message });
        }

        public IEarthNamedpipeService CreateChannel(string address)
        {
            try
            {
                NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                binding.MaxBufferPoolSize = cMaxBuffer;
                binding.MaxBufferSize = cMaxBuffer;
                binding.MaxReceivedMessageSize = cMaxBuffer;
                binding.ReceiveTimeout = TimeSpan.MaxValue;

                ServiceEndpoint se = new ServiceEndpoint(
                    ContractDescription.GetContract(typeof(IEarthNamedpipeService)),
                    binding,
                    new EndpointAddress(address));

                _factory = new DuplexChannelFactory<IEarthNamedpipeService>(new InstanceContext(this), se);
                IEarthNamedpipeService channel = _factory.CreateChannel();
                return channel;
            }
            catch
            {
                Console.WriteLine(cConnectError);
                return null;
            }
        }

        public async Task<string> Connect(string exePath = null)
        {
            try
            {
              string address = null;
              ProcessUtils proc = new ProcessUtils(c_processName, exePath);
              if (!proc.IsRunning())
              {
                  await proc.Start((msg) => ProcessStdOutCallBack(msg, ref address));
                  if(String.IsNullOrEmpty(address))
                  {
                      return cFailed;
                  }
              }
              else
              {
                  address = cBasePipeAddress;
              }

              if (!String.IsNullOrEmpty(address))
              {
                  _channel = CreateChannel(address);

                  // call a function to test consistency of contract file.
                  string test = _channel.GetCameraJson();

                  if (_channel != null)
                  {
                      return cSuccess;
                  }
                  else
                  {
                      return cFailed;
                  }
              }
              return cFailed;
            }
            catch
            {
                _channel = null;
                return cFailed;
            }
        }

        public string AddLayer(string json)
        {
            if(_channel == null)
            {
                return cNeedConnect;
            }

            try
            {
                return _channel.AddLayerSync(json);
            }
            catch (FaultException<EarthNamedpipeFault> ex)
            {
                return ex.Message;
            }
            return cWaitAddingLayer; 
        }

        public string ClearLayers(string json)
        {
            if (_channel == null)
            {
                return cNeedConnect;
            }

            try
            {
                if (_channel.ClearLayers(json))
                {
                    return cSuccess;
                }
            }
            catch (Exception ex)
            {
                if(ex is FaultException<EarthNamedpipeFault>)
                {
                    return (ex as FaultException<EarthNamedpipeFault>).Message;
                }
                else
                {
                    return ex.Message;
                }
            }

            return cFailed;
        }

        public string FlyTo(string json)
        {
            if (_channel == null)
            {
                return cNeedConnect;
            }

            try
            {
                if (_channel.FlyTo(json))
                {
                    return cSuccess;
                }
            }
            catch (Exception ex)
            {
                if (ex is FaultException<EarthNamedpipeFault>)
                {
                    return (ex as FaultException<EarthNamedpipeFault>).Message;
                }
                else
                {
                    return ex.Message;
                }
            }

            return cFailed;
        }

        public string SetCamera(string json)
        {
            if (_channel == null)
            {
                return cNeedConnect;
            }

            try
            {
                if (_channel.SetCamera(json))
                {
                    return cSuccess;
                }
            }
            catch (Exception ex)
            {
                if (ex is FaultException<EarthNamedpipeFault>)
                {
                    return (ex as FaultException<EarthNamedpipeFault>).Message;
                }
                else
                {
                    return ex.Message;
                }
            }

            return cFailed;
        }

        public string GetCamera()
        {
            if (_channel == null)
            {
                return cNeedConnect;
            }

            try
            {
                return _channel.GetCameraJson();
            }
            catch (Exception ex)
            {
                if (ex is FaultException<EarthNamedpipeFault>)
                {
                    return (ex as FaultException<EarthNamedpipeFault>).Message;
                }
                else
                {
                    return ex.Message;
                }
            }
        }

        public async Task<string> GetSnapshot(string path)
        {
            if (_channel == null)
            {
                return cNeedConnect;
            }

            try
            {
                System.Drawing.Bitmap bitmap = await _channel.GetSnapshotAsync();
                bitmap.Save(path);
                return cSuccess;
            }
            catch (Exception ex)
            {
                if (ex is FaultException<EarthNamedpipeFault>)
                {
                    return (ex as FaultException<EarthNamedpipeFault>).Message;
                }
                else
                {
                    return ex.Message;
                }
            }
        }
    }
}
