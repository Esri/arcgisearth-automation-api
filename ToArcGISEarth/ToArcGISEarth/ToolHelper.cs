using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Mapping;
using EarthAPIUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ToArcGISEarth
{
    public static class ToolHelper
    {
        public static EarthNamedpipeAPIUtils Utils { get; } = new EarthNamedpipeAPIUtils();
        public static bool IsConnectSuccessfully { get; set; } = false; // Recording the status of connecting to ArcGIS Earth
        public static Dictionary<string, string[]> IdNameDic { get; set; } = new Dictionary<string, string[]>(); // Recording the layer id, layer name and layer MapLayerType         
        public static bool IsArcGISEarthRunning { get { return ArcGISEarthRunning(); } }
        public static bool IsArcGISProSceneOpening { get { return ArcGISProSceneOpening(); } }

        private static bool ArcGISEarthRunning()
        {
            return Process.GetProcessesByName("ArcGISEarth").Length > 0;
        }

        private static bool ArcGISProSceneOpening()
        {
            return MapView.Active?.Map?.IsScene == true;
        }

        #region Get data source

        public static string GetDataSource(CIMObject dataConnection, bool isGetElevationSource)
        {
            if (dataConnection != null)
            {
                //  Shapfile, raster, tpk, Feature Layer
                if (dataConnection is CIMStandardDataConnection)
                {
                    WorkspaceFactory factory = (dataConnection as CIMStandardDataConnection).WorkspaceFactory;
                    if (factory == WorkspaceFactory.FeatureService)
                    {
                        return GetFeatureServerUrl(dataConnection as CIMStandardDataConnection);
                    }
                    if (factory == WorkspaceFactory.Shapefile || factory == WorkspaceFactory.Raster)
                    {
                        return GetShpRasterUrl(dataConnection as CIMStandardDataConnection);
                    }
                }
                // Kml, kmz
                if (dataConnection is CIMKMLDataConnection)
                {
                    return GetKmlUrl(dataConnection as CIMKMLDataConnection);
                }
                // Spk, slpk
                if (dataConnection is CIMSceneDataConnection)
                {
                    return GetSpkUrl(dataConnection as CIMSceneDataConnection);
                }
                // Imagary Layer, Map Image Layer, Tile Layer , Scene Layer
                if (dataConnection is CIMAGSServiceConnection)
                {
                    return GetNotFeatureServerUrl(dataConnection as CIMAGSServiceConnection, isGetElevationSource);
                }
                // Wms
                if (dataConnection is CIMWMSServiceConnection)
                {
                    return GetWmsUrl(dataConnection as CIMWMSServiceConnection);
                }
            }
            return null;
        }

        private static string GetFeatureServerUrl(CIMStandardDataConnection dataConnection)
        {
            string connectStr = dataConnection?.WorkspaceConnectionString; // e.g.  "URL=http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/SanFrancisco/311Incidents/FeatureServer/0"
            if (connectStr?.Length > 4)
            {
                return connectStr.Substring(4);
            }
            return null;
        }

        private static string GetShpRasterUrl(CIMStandardDataConnection dataConnection)
        {
            string fileDirectory = "";
            string fileName = dataConnection?.Dataset; // e.g.  "test.shp"
            string connectStr = dataConnection?.WorkspaceConnectionString; // e.g.  "DATABASE=D:\Temp"
            if (connectStr?.Length > 9)
            {
                fileDirectory = connectStr.Substring(9) + Path.DirectorySeparatorChar;
            }
            return fileDirectory + fileName;
        }

        private static string GetKmlUrl(CIMKMLDataConnection dataConnection)
        {
            return dataConnection?.KMLURI;
        }

        private static string GetSpkUrl(CIMSceneDataConnection dataConnection)
        {
            string realUrl = dataConnection?.URI; // e.g. "file:/D:/temp/test.slpk/layers/0"
            Uri.TryCreate(realUrl, UriKind.RelativeOrAbsolute, out Uri uri);
            if (uri != null)
            {
                realUrl = uri.AbsolutePath;
                if (realUrl.Length >= 9)
                {
                    return realUrl.Remove(realUrl.Length - 9, 9); // e.g.  "/D:/temp/test.slpk"
                }
            }
            return null;
        }

        private static string GetNotFeatureServerUrl(CIMAGSServiceConnection dataConnection, bool isAddToElevationSource)
        {
            // Imager server
            if (dataConnection?.ObjectType == "ImageServer" && isAddToElevationSource == false)
            {
                string url = dataConnection?.URL;
                if (url.Contains("services"))
                {
                    string[] splitStr = url.Split(new string[] { "services" }, StringSplitOptions.None);
                    if (splitStr?.Length >= 2 && splitStr.FirstOrDefault() != null)
                    {
                        return splitStr[0] + "rest/" + "services" + splitStr[1];
                    }
                }
                return null;
            }
            return dataConnection.URL;
        }

        private static string GetWmsUrl(CIMWMSServiceConnection dataConnection)
        {
            if (dataConnection?.ServerConnection is CIMProjectServerConnection)
            {
                return (dataConnection?.ServerConnection as CIMProjectServerConnection).URL;
            }
            if (dataConnection?.ServerConnection is CIMInternetServerConnection)
            {
                return (dataConnection?.ServerConnection as CIMInternetServerConnection).URL;
            }
            return null;
        }

        #endregion  Get data source

        #region Elevation sufaces

        public static Dictionary<int, string> AddedOrRemovedElevationSource(CIMMapElevationSurface[] previousElevationSurfaces, CIMMapElevationSurface[] currentElevationSurfaces, ref bool? addOrRemove)
        {
            if (currentElevationSurfaces != null)
            {
                addOrRemove = null;
                Dictionary<int, string> previousDic = GetAllElevationSource(previousElevationSurfaces, out int e1);
                Dictionary<int, string> currentDic = GetAllElevationSource(currentElevationSurfaces, out int e2);
                if (e1 < e2)
                {
                    addOrRemove = true;
                    return GetAddedElevationSource(previousDic, currentDic);
                }
                if (e1 > e2)
                {
                    addOrRemove = false;
                    return GetRemovedElevationSource(previousDic, currentDic);
                }
                return null;
            }
            return null;
        }

        private static Dictionary<int, string> GetAllElevationSource(CIMMapElevationSurface[] elevationSurfaces, out int count)
        {
            count = 0;
            Dictionary<int, string> idUrlDirc = new Dictionary<int, string>();
            if (elevationSurfaces != null)
            {
                for (int i = 0; i < elevationSurfaces.Length; i++)
                {
                    var baseSources = elevationSurfaces[i].BaseSources;
                    for (int j = 0; j < baseSources?.Length; j++)
                    {
                        // id , url
                        idUrlDirc.Add(count, ToolHelper.GetDataSource(baseSources[j].DataConnection, true));
                        count++;
                    }
                }
            }
            return idUrlDirc;
        }

        private static Dictionary<int, string> GetAddedElevationSource(Dictionary<int, string> previousDic, Dictionary<int, string> currentDic)
        {
            if (currentDic?.Count > previousDic?.Count)
            {
                currentDic.OrderBy(p => p.Value);
                previousDic.OrderBy(p => p.Value);
                foreach (var item in currentDic)
                {
                    if (previousDic.ContainsKey(item.Key))
                    {
                        if (previousDic.TryGetValue(item.Key, out string url) && url != item.Value)
                        {
                            return new Dictionary<int, string> { { item.Key, item.Value } };
                        }
                    }
                    else
                    {
                        // Pro can only load an elevation source at once
                        return new Dictionary<int, string> { { item.Key, item.Value } };
                    }
                }
                return null;
            }
            return null;
        }

        private static Dictionary<int, string> GetRemovedElevationSource(Dictionary<int, string> previousDic, Dictionary<int, string> currentDic)
        {
            if (currentDic?.Count < previousDic?.Count)
            {
                currentDic.OrderBy(p => p.Value);
                previousDic.OrderBy(p => p.Value);
                foreach (var item in previousDic)
                {
                    if (currentDic.ContainsKey(item.Key))
                    {
                        if (currentDic.TryGetValue(item.Key, out string url) && url != item.Value)
                        {
                            return new Dictionary<int, string> { { item.Key, item.Value } };
                        }
                    }
                    else
                    {
                        return new Dictionary<int, string> { { item.Key, item.Value } };
                    }
                }
                return null;
            }
            return null;
        }

        #endregion Elevation sufaces
    }
}
