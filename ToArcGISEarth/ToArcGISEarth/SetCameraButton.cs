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

using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace ToArcGISEarth
{
    public class SetCameraButton : Button
    {
        public SetCameraButton()
        {
            Enabled = false;
        }

        protected override void OnClick()
        {
            if (IsChecked)
            {
                MapViewCameraChangedEvent.Unsubscribe(SetCameraInEarthEvent);
                IsChecked = false;
            }
            else
            {
                MapViewCameraChangedEvent.Subscribe(SetCameraInEarthEvent, false);
                IsChecked = true;
            }
        }

        protected override void OnUpdate()
        {
            if (ToolHelper.IsConnectSuccessfully)
            {
                Enabled = true;
            }
            else
            {
                MapViewCameraChangedEvent.Unsubscribe(SetCameraInEarthEvent);
                Enabled = false;
                IsChecked = false;
            }
        }

        private void SetCameraInEarthEvent(MapViewCameraChangedEventArgs args)
        {
            try
            {
                MapView mapView = args.MapView;
                if (null != mapView && null != mapView.Camera && mapView.ViewingMode == MapViewingMode.SceneGlobal)
                {
                    JObject cameraJObject = new JObject();
                    Dictionary<string, double> location = new Dictionary<string, double>
                    {
                        ["x"] = mapView.Camera.X,
                        ["y"] = mapView.Camera.Y,
                        ["z"] = mapView.Camera.Z
                    };

                    // Convert ArcGIS Pro camera to ArcGIS Earth
                    JObject locationJObject = JObject.Parse(JsonConvert.SerializeObject(location));
                    cameraJObject["mapPoint"] = locationJObject;
                    cameraJObject["heading"] = mapView.Camera.Heading > 0 ? 360 - mapView.Camera.Heading : -mapView.Camera.Heading;
                    cameraJObject["pitch"] = mapView.Camera.Pitch + 90;
                    ToolHelper.Utils.SetCamera(cameraJObject.ToString());
                }
            }
            catch
            {
            }
        }
    }
}
