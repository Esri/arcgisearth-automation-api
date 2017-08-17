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
using EarthAPITest;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ToArcGISEarth
{
    internal class SyncToArcGISEarthButton : Button
    {
        private EarthNamedpipeAPIUtils _utils = null;
        private SyncToArcGISEarthButton()
        {
            _utils = new EarthNamedpipeAPIUtils();
        }

        protected override void OnClick()
        {
            if (this.IsChecked)
            {
                MapViewCameraChangedEvent.Unsubscribe(MapViewCameraCanged);
                this.IsChecked = false;
                this.Caption = "Connect";
            }
            else
            {
                if(!_utils.Connect())
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please start ArcGIS Earth with automation API opened then connect to it");
                    return;
                }
                MapViewCameraChangedEvent.Subscribe(MapViewCameraCanged, false);
                this.IsChecked = true;
                this.Caption = "Disconnect";
            }
        }

        private void MapViewCameraCanged(MapViewCameraChangedEventArgs args)
        {
            MapView mapView = args.MapView;
            if (null != mapView && null != mapView.Camera)
            {
                JObject camraObj = new JObject();

                //currentCameraJson = JsonConvert.SerializeObject(mapView.Camera);
                JObject camera_j_obj = new JObject();
                Dictionary<string, double> location = new Dictionary<string, double>();
                location["x"] = mapView.Camera.X;
                location["y"] = mapView.Camera.Y;
                location["z"] = mapView.Camera.Z;

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
    }
}
