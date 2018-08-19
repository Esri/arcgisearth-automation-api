using ArcGIS.Desktop.Mapping;
using EarthAPIUtils;
using System.Collections.Generic;
using System.Diagnostics;

namespace ToArcGISEarth
{
    public static class ToolHelper
    {
        public static EarthNamedpipeAPIUtils Utils { get; } = new EarthNamedpipeAPIUtils();
        public static bool IsConnectSuccessfully { get; set; } = false; // Recording the status of connecting to ArcGIS Earth
        public static Dictionary<string, string[]> IdNameDic { get; set; } = new Dictionary<string, string[]>(); // Recording the layer id, layer name and layer MapLayerType     
        public static bool IsSynchronizationButtonChecked { get { return SynchronizationButtonChecked(); } }
        public static bool IsArcGISEarthRunning { get { return ArcGISEarthRunning(); } }
        public static bool IsArcGISProSceneOpening { get { return ArcGISProSceneOpening(); } }

        private static bool SynchronizationButtonChecked()
        {
            return SetCameraButton.HasChecked || AddLayerButton.HasChecked || RemoveLayerButton.HasChecked;
        }

        private static bool ArcGISEarthRunning()
        {
            return Process.GetProcessesByName("ArcGISEarth").Length > 0;
        }

        private static bool ArcGISProSceneOpening()
        {
            return MapView.Active?.Map?.IsScene == true;
        }
    }
}
