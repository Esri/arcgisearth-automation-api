using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using EarthAPIUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToArcGISEarth
{
    public class SyncToArcGISEarthCheckBox : CheckBox
    {    
        protected override void OnClick()
        {
            if ((bool)IsChecked)
            {
                if (!ConnectToArcGISEarthButton.IsConnectSuccessful)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please connect to ArcGIS Earth");
                    this.IsChecked = false;
                    return;
                }
                else
                {
                    MapViewCameraChangedEvent.Subscribe(SetCamera,false);
                }               
            }
            else
            {
                MapViewCameraChangedEvent.Unsubscribe(SetCamera);
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
                ConnectToArcGISEarthButton.Utils.SetCamera(currentCameraJson);
            }
        }      
    }
}
