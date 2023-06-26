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

using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using ArcGISEarth.AutoAPI.Utils;
using System.Text.Json.Nodes;

namespace ArcGISProToArcGISEarth
{
    public class SetCameraSyncCheckBox : Button
    {
        public SetCameraSyncCheckBox()
        {
            Enabled = false;
        }

        protected override void OnClick()
        {
            if (IsChecked)
            {
                MapViewCameraChangedEvent.Unsubscribe(SetCameraInEarth);
                IsChecked = false;
            }
            else
            {
                MapViewCameraChangedEvent.Subscribe(SetCameraInEarth, false);
                IsChecked = true;
            }
        }

        protected override void OnUpdate()
        {
            // Set button status when status of ArcGIS Earth or ArcGIS Pro changed.
            if (ToolHelper.IsArcGISEarthRunning && ToolHelper.IsArcGISProGlobalSceneOpening)
            {
                Enabled = true;
            }
            else
            {
                MapViewCameraChangedEvent.Unsubscribe(SetCameraInEarth);
                Enabled = false;
                IsChecked = false;
            }
        }

        private async void SetCameraInEarth(MapViewCameraChangedEventArgs args)
        {
            try
            {
                // Get current camera of ArcGIS Pro.
                MapView mapView = args.MapView;
                if (null != mapView && null != mapView.Camera && mapView.ViewingMode == MapViewingMode.SceneGlobal)
                {
                    JsonObject cameraJObject = new JsonObject
                    {
                        // Get position.
                        ["position"] = new JsonObject
                        {
                            ["x"] = mapView.Camera.X,
                            ["y"] = mapView.Camera.Y,
                            ["z"] = mapView.Camera.Z,
                            ["spatialReference"] = new JsonObject
                            {
                                ["wkid"] = mapView.Camera.SpatialReference?.Wkid
                            }
                        },
                        // Get heading.
                        ["heading"] = mapView.Camera.Heading > 0 ? 360 - mapView.Camera.Heading : -mapView.Camera.Heading,
                        // Get tilt.
                        ["tilt"] = mapView.Camera.Pitch + 90,
                        // Get roll.
                        ["roll"] = mapView.Camera.Roll
                    };
                    // Set camera in ArcGIS Earth.
                    await AutomationAPIHelper.SetCamera(cameraJObject.ToString());
                }
            }
            catch
            {
            }
        }
    }
}
