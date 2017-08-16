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
