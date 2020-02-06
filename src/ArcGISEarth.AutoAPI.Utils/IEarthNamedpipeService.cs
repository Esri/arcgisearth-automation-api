﻿// Copyright 2018 Esri
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

using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace Esri.ArcGISEarth.WCFNamedPipeIPC
{
    /// <summary>
    /// Fault contract of ArcGIS Earth Automation API.
    /// </summary>
    /// <remarks>
    /// The ArcGIS Earth Automation API will throw exceptions as EarthNamedpipeFault, and clients side need catch this kind of exception.
    /// </remarks>
    [DataContract]
    public class EarthNamedpipeFault
    {
        /// <summary>
        /// The fault message.
        /// </summary>
        [DataMember]
        public string Message { get; set; }
    }

    /// <summary>
    /// The public interface for communicating with ArcGIS Earth Automation API.
    /// </summary>
    [ServiceContract(Namespace = "ArcGISEarth/2018/07")]
    public interface IEarthNamedpipeService
    {
        /// <summary>
        /// Get current viewing perspective of the scene.
        /// </summary>
        /// <returns>A string in JSON format contains camera information
        /// <remarks>
        /// The JSON string of camera contains fields of "mapPoint"(which contains "x", "y", "z", "spatialReference"), "heading", "pitch" of camera.
        /// The value of x, y, z and spatialReference indicate the position of camera, and spatialReference is specified by WKID.
        /// The heading value ranges from 0 to 360 degrees, starting from north in ENU.
        /// The pitch value ranges from 0 to 180. 0 is looking straight down and 180 looking straight up(towards outer space).
        /// </remarks>
        /// </returns>
        [FaultContract(typeof(EarthNamedpipeFault))]
        [OperationContract]
        string GetCameraJson();

        /// <summary>
        /// Set or update the viewing perspective of the scene to a specified position.
        /// </summary>
        /// <param name="json">is a string in JSON format contains camera information
        /// <remarks>
        /// The JSON string parameter of camera requires fields of "mapPoint"(which contains "x", "y", "z", "spatialReference"), "heading", "pitch" of camera.
        /// x, y, z and spatialReference indicate the position of camera, and spatialReference is specified by WKID.
        /// The heading value ranges from 0 to 360 degrees, starting from north in ENU.
        /// The pitch value ranges from 0 to 180. 0 is looking straight down and 180 looking straight up(towards outer space).
        /// </remarks>
        /// </param>
        /// <returns>Boolean indicating if successfully setting camera</returns>
        [FaultContract(typeof(EarthNamedpipeFault))]
        [OperationContract]
        bool SetCamera(string json);

        /// <summary>
        /// Set or update the camera animation when fly to a target position.
        /// </summary>
        /// <param name="json">is a string in JSON format contains camera and duration information
        /// <remarks>
        /// The JSON string parameter of camera requires fields of "mapPoint"(which contains "x", "y", "z", "spatialReference"), "heading", "pitch" of camera and "duration" for flying time.
        /// x, y, z and spatialReference indicate the position of camera, and spatialReference is specified by WKID.
        /// The heading value ranges from 0 to 360 degrees, starting from north in ENU.
        /// The pitch value ranges from 0 to 180. 0 is looking straight down and 180 looking straight up(towards outer space).
        /// The duration value sets the flying time counts in seconds. If not specified, the default value is 2.  
        /// </remarks>
        /// </param>
        /// <returns>Boolean indicating if successfully flying to the target position</returns>
        [FaultContract(typeof(EarthNamedpipeFault))]
        [OperationContract]
        bool FlyTo(string json);

        /// <summary>
        /// Add layer to the table of contents, basemap or terrain of ArcGIS Earth.
        /// </summary>
        /// <param name="json">is a string in JSON format contains layer type, URI and target of layer
        /// <para>
        /// The JSON string parameter can have fields: "type"(optional), "target"(optional) and URI(or URIs).
        /// </para>
        /// <para>
        /// The type value specifies type supported in ArcGIS Earth. The API will automatically set an appropriate type according to URI if the JSON string doesn't contain this key.
        /// </para>
        /// <para>And the value can be one of these: </para>
        /// <para>"FeatureService" – ArcGIS feature service.</para>
        /// <para>"MapService" – ArcGIS map service.</para>
        /// <para>"ImageService" – ArcGIS image service.</para>
        /// <para>"Shapefile" – Local shapefile.</para>
        /// <para>"OGCWMS" – OGC Web Map Service.</para>
        /// <para>"KML" – Local KML and KMZ.</para>
        /// <para>"SceneLayerPackage" – ArcGIS scene layer package.</para>
        /// <para>"SceneService" – ArcGIS scene service.</para>
        /// <para>"Raster" – Local elevation raster formats.</para>
        /// <para>"TileLayerPackage" – ArcGIS tile layer package.</para>
        /// <para>
        /// The URI(URIs) value specifies the URL or path of a layer. Use URIs if the source files are multiple elevation source and used as elevation.
        /// </para>
        /// <para>
        /// The target value specifies the target place where the layers added to. If not specified, the default value is "OperationalLayers".
        /// <para>The value can one of these:</para>
        /// <para>"OperationalLayers" - layers added into the table of contents of ArcGIS Earth.</para>
        /// <para>"BasemapLayers" - layers added into Basemap pane of ArcGIS Earth as basemap.</para>
        /// <para>"ElevationLayers" – layers added into Terrain pane of ArcGIS Earth as elevation.</para>
        /// </para>
        /// </param>
        /// <returns>String indicating the id of layer.</returns>
        [FaultContract(typeof(EarthNamedpipeFault))]
        [OperationContract]
        string AddLayer(string json);

        /// <summary>
        /// Get load status of layer.
        /// </summary>
        /// <param name="json">is a string in JSON format contains id of layer 
        /// <remarks>
        /// The JSON string requires fields of "id" which value is the string type id of layer. 
        /// </remarks>
        /// </param>
        /// <returns>String indicating the load status of layer.</returns>
        [FaultContract(typeof(EarthNamedpipeFault))]
        [OperationContract]
        string GetLayerLoadStatus(string json);

        /// <summary>
        /// Remove a layer.
        /// </summary>
        /// <param name="json">is a string in JSON format contains id of layer 
        /// <remarks>
        /// The JSON string requires fields of "id" which value is the string type id of layer. 
        /// </remarks>
        /// </param>
        [FaultContract(typeof(EarthNamedpipeFault))]
        [OperationContract]
        void RemoveLayer(string json);

        /// <summary>
        /// Get the JSON description of current workspace.
        /// </summary>
        /// <returns>String indicating the JSON description of current workspace.</returns>
        [FaultContract(typeof(EarthNamedpipeFault))]
        [OperationContract]
        string GetWorkspace();

        /// <summary>
        /// Import workspace to ArcGIS Earth.
        /// </summary>
        /// <param name="json">is a string in JSON format of ArcGIS Earth workspace 
        /// <remarks>
        /// <para>The JSON string parameters can have fields as "operationalLayers", "elevationLayers", "basemapLayers" and "bookmarks"</para>
        /// <para>Each value indicating the JSON desciption of each component of workspace"</para>
        /// </remarks>
        /// </param>
        [FaultContract(typeof(EarthNamedpipeFault))]
        [OperationContract]
        void ImportWorkspace(string json);

        /// <summary>
        /// Remove layers from ArcGIS Earth workspace.
        /// </summary>
        /// <param name="json">is a string in JSON format contains target place where the layers exist
        /// <remarks>
        /// <para>The JSON string parameters can have an field of "target", the target value specify the target place of the layer exist.</para>
        /// <para>If not specified, the default value is "OpertaionalLayers". And the value can be one of these: "OperationalLayers", "BasemapLayers", "ElevationLayers" and "AllLayrs".</para>
        /// </remarks>
        /// </param>
        /// <returns>Boolean indicating if successfully clearing layers</returns>
        [FaultContract(typeof(EarthNamedpipeFault))]
        [OperationContract]
        bool ClearLayers(string json);

        /// <summary>
        /// Take screen snapshot of the current view of ArcGIS Earth.
        /// </summary>
        /// <returns>A System.Drawing.Bitmap of screen snapshot</returns>
        [FaultContract(typeof(EarthNamedpipeFault))]
        [OperationContract]
        Task<System.Drawing.Bitmap> GetSnapshotAsync();
    }
}
