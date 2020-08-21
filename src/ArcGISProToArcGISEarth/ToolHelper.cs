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
using ArcGIS.Desktop.Mapping;
using ArcGISEarth.AutoAPI.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ToArcGISEarth
{
    public static class ToolHelper
    {
        // Replace with your own api url setting
        private const string DEFAULT_API_URL = "http://localhost:8000/arcgisearth";

        // ArcGIS Earth automation api utils.
        public static AutomationAPIHelper Utils { get; } = new AutomationAPIHelper()
        {
            APIBaseUrl = DEFAULT_API_URL
        };

        // Logging id and it's infomation when adding layer or elevation source.
        public static Dictionary<string, string[]> IdInfoDictionary { get; set; } = new Dictionary<string, string[]>();

        // Logging the status of ArcGIS Earth running.
        public static bool IsArcGISEarthRunning
        {
            get { return Process.GetProcessesByName("ArcGISEarth").Length > 0; }
        }

        // Logging the status of ArcGIS Pro global scene opening.
        public static bool IsArcGISProGlobalSceneOpening
        {
            get { return MapView.Active?.Map?.IsScene == true && MapView.Active?.Map.DefaultViewingMode == MapViewingMode.SceneGlobal; }
        }

        public static string GetDataSource(CIMDataConnection dataConnection)
        {
            // For more information about CIMDataConnect class and its derived classes, please refer to http://pro.arcgis.com/en/pro-app/sdk/api-reference/#topic943.html.
            if (dataConnection != null)
            {
                //  DataConnection of Shapefile, Raster, TPK and Feature Layer.
                if (dataConnection is CIMStandardDataConnection)
                {
                    WorkspaceFactory factory = (dataConnection as CIMStandardDataConnection).WorkspaceFactory;
                    if (factory == WorkspaceFactory.FeatureService)
                    {
                        // Get url of Feature Layer.
                        return GetServiceUrl(dataConnection as CIMStandardDataConnection);
                    }
                    if (factory == WorkspaceFactory.Shapefile || factory == WorkspaceFactory.Raster)
                    {
                        // Get path name of Shapefile or Rater.
                        return GetStandardDataPathName(dataConnection as CIMStandardDataConnection);
                    }
                }
                // DataConnection of Kml and Kmz.
                if (dataConnection is CIMKMLDataConnection)
                {
                    // Get path name of Kml and Kmz.
                    return GetKMLDataPathName(dataConnection as CIMKMLDataConnection);
                }
                // DataConnection of SPK and SLPK.
                if (dataConnection is CIMSceneDataConnection)
                {
                    // Get path name of SPK and SLPK.
                    return GetSceneDataPathName(dataConnection as CIMSceneDataConnection);
                }
                // DataConnection of Imagary Layer, Map Image Layer, Tile Layer , Scene Layer.
                if (dataConnection is CIMAGSServiceConnection)
                {
                    // Get url of Imagary Layer, Map Image Layer, Tile Layer , Scene Layer.
                    return GetServiceUrl(dataConnection as CIMAGSServiceConnection);
                }
                // DataConnection of WMS
                if (dataConnection is CIMWMSServiceConnection)
                {
                    // Get url of WMS.
                    return GetWmsUrl(dataConnection as CIMWMSServiceConnection);
                }
            }
            return null;
        }

        private static string GetStandardDataPathName(CIMStandardDataConnection dataConnection)
        {
            string connectStr = dataConnection?.WorkspaceConnectionString; // e.g.  "DATABASE=D:\Temp".
            string fileName = dataConnection?.Dataset; // e.g.  "test.shp".  
            string fileDirectory = "";
            if (connectStr?.Length > 9)
            {
                fileDirectory = connectStr.Substring(9) + Path.DirectorySeparatorChar; // e.g. "D:\Temp\".
            }
            return fileDirectory + fileName; // e.g. "D:\Temp\test.shp".
        }

        private static string GetKMLDataPathName(CIMKMLDataConnection dataConnection)
        {
            return dataConnection?.KMLURI; // e.g "D:\Temp\KML_Samples.kml".
        }

        private static string GetSceneDataPathName(CIMSceneDataConnection dataConnection)
        {
            string realUrl = dataConnection?.URI; // e.g. "file:/D:/temp/test.slpk/layers/0".
            Uri.TryCreate(realUrl, UriKind.RelativeOrAbsolute, out Uri uri);
            if (uri != null)
            {
                realUrl = uri.AbsolutePath; // e.g. "D:/temp/test.slpk/layers/0".
                if (realUrl.Length >= 9)
                {
                    return realUrl.Remove(realUrl.Length - 9, 9); // e.g.  "/D:/temp/test.slpk".
                }
            }
            return null;
        }

        private static string GetServiceUrl(CIMDataConnection dataConnection)
        {
            if (dataConnection is CIMStandardDataConnection)
            {
                // Feature service 
                string connectStr = (dataConnection as CIMStandardDataConnection)?.WorkspaceConnectionString; // e.g.  "URL=http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/SanFrancisco/311Incidents/FeatureServer/0".
                if (connectStr?.Length > 4)
                {
                    return connectStr.Substring(4); // e.g. "http://sampleserver3.arcgisonline.com/ArcGIS/rest/services/SanFrancisco/311Incidents/FeatureServer/0".
                }
                return null;
            }
            if (dataConnection is CIMAGSServiceConnection)
            {
                // Imager service
                if ((dataConnection as CIMAGSServiceConnection)?.ObjectType == "ImageServer")
                {
                    string url = (dataConnection as CIMAGSServiceConnection)?.URL; // e.g. "http://services.arcgisonline.com/arcgis/services/World_Imagery/MapServer".
                    if (url.Contains("services"))
                    {
                        string[] splitStr = url.Split(new string[] { "services" }, StringSplitOptions.None);
                        if (splitStr?.Length >= 2 && splitStr.FirstOrDefault() != null)
                        {
                            return splitStr[0] + "rest/" + "services" + splitStr[1]; // e.g. "http://services.arcgisonline.com/arcgis/rest/services/World_Imagery/MapServer".
                        }
                    }
                    return null;
                }
                // Other services
                return (dataConnection as CIMAGSServiceConnection).URL;
            }
            return null;
        }

        private static string GetWmsUrl(CIMWMSServiceConnection dataConnection)
        {
            if (dataConnection?.ServerConnection is CIMProjectServerConnection)
            {
                return (dataConnection?.ServerConnection as CIMProjectServerConnection).URL; // e.g. "http://mesonet.agron.iastate.edu/cgi-bin/wms/nexrad/n0r.cgi"
            }
            if (dataConnection?.ServerConnection is CIMInternetServerConnection)
            {
                return (dataConnection?.ServerConnection as CIMInternetServerConnection).URL;
            }
            return null;
        }
    }
}
