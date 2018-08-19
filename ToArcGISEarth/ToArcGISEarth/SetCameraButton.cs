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
        public static bool HasChecked { get; set; }

        public SetCameraButton()
        {
            this.Enabled = false;
        }

        protected override void OnClick()
        {
            if (this.IsChecked)
            {
                MapViewCameraChangedEvent.Unsubscribe(SetCamera);
                this.IsChecked = false;
                HasChecked = false;
            }
            else
            {
                MapViewCameraChangedEvent.Subscribe(SetCamera, false);
                this.IsChecked = true;
                HasChecked = true;
            }
        }

        protected override void OnUpdate()
        {
            if (ToolHelper.IsConnectSuccessfully)
            {
                this.Enabled = true;
            }
            else
            {             
                MapViewCameraChangedEvent.Unsubscribe(SetCamera); 
                this.Enabled = false;
                this.IsChecked = false;
                HasChecked = false;
            }
        }

        private void SetCamera(MapViewCameraChangedEventArgs args)
        {           
            try
            {
                MapView mapView = args.MapView;
                if (null != mapView && null != mapView.Camera && mapView.ViewingMode == MapViewingMode.SceneGlobal)
                {
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
                    string currentCameraJson = camera_j_obj.ToString();
                    ToolHelper.Utils.SetCamera(currentCameraJson);
                }
            }
            catch
            {
                return;
            }
        }
    }
}
