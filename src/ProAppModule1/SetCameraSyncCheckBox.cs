using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using ArcGISEarth.AutoAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ProAppModule1
{
    internal class SetCameraSyncCheckBox : Button
    {       
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
