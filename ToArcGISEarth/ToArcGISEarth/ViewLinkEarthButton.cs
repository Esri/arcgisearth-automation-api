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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping.Events;
using ArcGIS.Desktop.Mapping;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using EarthAPIUtils;
using ArcGIS.Desktop.Framework.Threading.Tasks;

namespace ToArcGISEarth
{
    internal class ViewLinkEarthButton : Button
    {
        private EarthNamedpipeAPIUtils _utils;

        //  layerID(json), layerName, layerType
        private Dictionary<string, List<string>> idNameDic;

        public ViewLinkEarthButton()
        {
            _utils = new EarthNamedpipeAPIUtils();
            idNameDic = new Dictionary<string, List<string>>();
        }

        protected override async void OnClick()
        {
            if (this.IsChecked)
            {
                _utils.CloseConnect();
                this.IsChecked = false;
                this.Caption = "Connect";
                this.idNameDic.Clear();
            }
            else
            {
                string result = await _utils.Connect();
                if (result != "Success")
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please start ArcGIS Earth with automation API opened then connect to it");
                    return;
                }
                else
                {
                    MapViewCameraChangedEvent.Subscribe(SetCamera, false);
                    LayersAddedEvent.Subscribe(AddLayer, false);
                    LayersRemovedEvent.Subscribe(RemoveLayer, false);
                    this.IsChecked = true;
                    this.Caption = "Disconnect";
                }             
            }
        }

        private void SetCamera(MapViewCameraChangedEventArgs args)
        {
            MapView mapView = args.MapView;
            if (null != mapView && null != mapView.Camera)
            {
                //currentCameraJson = JsonConvert.SerializeObject(mapView.Camera);
                JObject camera_j_obj = new JObject();
                Dictionary<string, double> location = new Dictionary<string, double>
                {
                    ["x"] = mapView.Camera.X,
                    ["y"] = mapView.Camera.Y,
                    ["z"] = mapView.Camera.Z
                };

                // Convert ArcGIS Pro camera to ArcGIS Earth
                JObject location_j_obj = JObject.Parse(JsonConvert.SerializeObject(location));
                camera_j_obj["mapPoint"] = location_j_obj;
                camera_j_obj["heading"] = mapView.Camera.Heading > 0 ? 360 - mapView.Camera.Heading : -mapView.Camera.Heading;
                camera_j_obj["pitch"] = mapView.Camera.Pitch + 90;
                var currentCameraJson = camera_j_obj.ToString();
                if (currentCameraJson != null)
                {
                    currentCameraJson = currentCameraJson.Replace("\n", "");
                    currentCameraJson = currentCameraJson.Replace("\r", "");
                }
                _utils.SetCamera(currentCameraJson);
            }
        }

        private void AddLayer(LayerEventsArgs args)
        {
            List<Layer> layerList = args.Layers as List<Layer>;
            if (layerList != null && layerList.Count != 0)
            {
                Layer layer = layerList[0] as Layer;
                JObject addLayerJson = new JObject();
                addLayerJson["URI"] = GetLayerUrl(layer);
                if (layer.MapLayerType == ArcGIS.Core.CIM.MapLayerType.Operational)
                {
                    addLayerJson["target"] = "OperationalLayers";
                }
                if (layer.MapLayerType == ArcGIS.Core.CIM.MapLayerType.BasemapBackground)
                {
                    addLayerJson["target"] = "BasemapLayers";
                }
                string currentJson = addLayerJson.ToString();
                if (currentJson != null)
                {
                    currentJson = currentJson.Replace("\n", "");
                    currentJson = currentJson.Replace("\r", "");
                }
                List<string> nameAndType = new List<string>();
                nameAndType.Add(layer.Name);
                nameAndType.Add(layer.MapLayerType.ToString());
                string id = _utils.AddLayer(currentJson);
                if (!idNameDic.Keys.Contains(id))
                {
                    idNameDic.Add(id, nameAndType);
                }
            }
        }

        private void RemoveLayer(LayerEventsArgs args)
        {
            List<Layer> layerList = args.Layers as List<Layer>;
            if (layerList != null && layerList.Count != 0)
            {
                Layer layer = layerList[0];
                string id = "";
                // If there have some layers that they have same name, just remove one of them 
                foreach (var item in idNameDic)
                {
                    if (item.Value?.Count == 2 && item.Value[0] == layer.Name && item.Value[1] == layer.MapLayerType.ToString())
                    {
                        id = item.Key;
                        break;
                    }
                }
                _utils.RemoveLayer(id);
                this.idNameDic.Remove(id);
            }
        }

        private void AddLayerToTarget(Layer layer)
        {

        }

        private string GetLayerUrl(Layer layer)
        {
            if (layer is ServiceLayer)
            {
                return (layer as ServiceLayer).URL;
            }
            if (layer is RasterLayer)
            {

            }
            if (layer is FeatureLayer)
            {

            }
            return "";
        }


    }
}
