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

        public static string GetDataSource(CIMObject dataConnection)
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
                    return GetNotFeatureServerUrl(dataConnection as CIMAGSServiceConnection);
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

        private static string GetNotFeatureServerUrl(CIMAGSServiceConnection dataConnection)
        {
            // Imager server
            if (dataConnection?.ObjectType == "ImageServer")
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

        public static List<string[]> AddedOrRemovedElevationSources(CIMMapElevationSurface[] previousElevationSurfaces, CIMMapElevationSurface[] currentElevationSurfaces, ref ElevationSourcesOperation operation)
        {
            if (currentElevationSurfaces != null)
            {
                operation = ElevationSourcesOperation.None;
                List<string[]> previousList = GetAllElevationSources(previousElevationSurfaces, out int e1);
                List<string[]> currentList = GetAllElevationSources(currentElevationSurfaces, out int e2);
                if (e1 < e2)
                {
                    operation = ElevationSourcesOperation.Add;
                    return GetChangedElevationSources(previousList, currentList, operation);
                }
                if (e1 > e2)
                {
                    operation = ElevationSourcesOperation.Remove;
                    return GetChangedElevationSources(previousList, currentList, operation);
                }
                return null;
            }
            return null;
        }

        private static List<string[]> GetAllElevationSources(CIMMapElevationSurface[] elevationSurfaces, out int count)
        {
            count = 0;
            List<string[]> sourcesList = new List<string[]>();
            if (elevationSurfaces != null)
            {
                for (int i = 0; i < elevationSurfaces.Length; i++)
                {
                    var baseSources = elevationSurfaces[i].BaseSources;
                    for (int j = 0; j < baseSources?.Length; j++)
                    {
                        sourcesList.Add(new string[3] { elevationSurfaces[i].Name, baseSources[j].Name, ToolHelper.GetDataSource(baseSources[j].DataConnection) });
                        count++;
                    }
                }
            }
            return sourcesList;
        }

        private static List<string[]> GetChangedElevationSources(List<string[]> previousList, List<string[]> currentList, ElevationSourcesOperation operation)
        {
            if (operation == ElevationSourcesOperation.Add)
            {
                return currentList?.Except(previousList, new ListStringArrComparer<string[]>())?.ToList();
            }
            if (operation == ElevationSourcesOperation.Remove)
            {
                return previousList?.Except(currentList, new ListStringArrComparer<string[]>())?.ToList();
            }
            return null;
        }

        private class ListStringArrComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y)
            {
                if (x is string[] && y is string[])
                {
                    string[] xArr = x as string[];
                    string[] yArr = y as string[];
                    if (xArr?.Length == 3 && yArr?.Length == 3)
                    {
                        for (int i = 0; i < xArr.Length; i++)
                        {
                            if (xArr[i] != yArr[i])
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                    return false;
                }
                return false;
            }

            public int GetHashCode(T obj)
            {
                return obj.ToString().GetHashCode();
            }
        }

        public enum ElevationSourcesOperation
        {
            None,
            Add,
            Remove
        }

        #endregion Elevation sufaces
    }
}


