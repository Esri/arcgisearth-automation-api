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
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using static ToArcGISEarth.ToolHelper;

namespace ToArcGISEarth
{
    public class AddLayerSyncCheckBox : Button
    {
        #region  ElevationSource variable and property

        private Timer _timer;
        private event PropertyChangedEventHandler _elevationSourceAddedChanged;
        private event PropertyChangedEventHandler _elevationSourceRemovedChanged;
        private List<string[]> _elevationSources = new List<string[]>();
        private ElevationSourcesOperation _sourcesOperation = ElevationSourcesOperation.None;
        private CIMMap currentCIMMap = new CIMMap();
        public CIMMap CurrentCIMMap
        {
            get { return currentCIMMap; }
            set
            {
                // If elevation sources changed, get changed source list.
                _elevationSources = ToolHelper.AddedOrRemovedElevationSources(currentCIMMap.ElevationSurfaces, value?.ElevationSurfaces, ref _sourcesOperation);
                // If added source, trigger _elevationSourceAddedChanged.
                if (Is_elevationSourceAddedChanged() && IsChecked)
                {
                    currentCIMMap = value;
                    _elevationSourceAddedChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCIMMap)));
                }
                // If removed source, trigger _elevationSourceRemovedChanged.
                if (Is_elevationSourceRemovedChanged() && RemoveLayerSyncCheckBox.HasChecked)
                {
                    currentCIMMap = value;
                    _elevationSourceRemovedChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCIMMap)));
                }
            }
        }

        #endregion ElevationSource variable and property

        public AddLayerSyncCheckBox()
        {
            Enabled = false;
            _timer = new Timer
            {
                Enabled = true,
                Interval = 1000
            };
            // Update CurrentCIMMap every 1 second to get the lasted elevation source list.
            _timer.Elapsed += (s, e) =>
            {
                // Must use QueuedTask to get avtive map definition. Then you can get elevation sources.
                QueuedTask.Run(() =>
                {
                    CurrentCIMMap = MapView.Active?.Map?.GetDefinition();
                });
            };
            _timer.Start();
        }

        protected override void OnClick()
        {
            if (IsChecked)
            {
                // Unsubscribe events of adding layer and elevation source.
                LayersAddedEvent.Unsubscribe(AddLayerToEarth);
                _elevationSourceAddedChanged -= AddElevationSource;
                IsChecked = false;
            }
            else
            {
                // Subscribe events of adding layer and elevation source.
                LayersAddedEvent.Subscribe(AddLayerToEarth, false);
                _elevationSourceAddedChanged += AddElevationSource;
                _timer.Enabled = true;
                _timer.Start();
                IsChecked = true;
            }
        }

        protected override void OnUpdate()
        {
            // Set button status when status of connecting to ArcGIS Earth changed.
            if (ToolHelper.IsConnectSuccessfully)
            {
                Enabled = true;
            }
            else
            {
                // Unsubscribe events of adding layer and elevation source.
                LayersAddedEvent.Unsubscribe(AddLayerToEarth);
                Enabled = false;
                IsChecked = false;
            }
            // Update _elevationSourceRemovedChanged status.
            Set_elevationSourceRemovedChangedStatus();
        }

        private void AddLayerToEarth(LayerEventsArgs args)
        {
            try
            {
                // Get added layer list.
                List<Layer> layerList = args.Layers as List<Layer>;
                if (layerList?.Count != 0 && !IsCreateNewGroupLayer(layerList))
                {
                    foreach (var layer in layerList)
                    {
                        QueuedTask.Run(() =>
                        {
                            // GetDataConnection method must be called within the lambda passed to QueuedTask.Run. 
                            CIMDataConnection dataConnection = layer.GetDataConnection();
                            // Get layer url.
                            string url = ToolHelper.GetDataSource(dataConnection);
                            if (!string.IsNullOrWhiteSpace(url))
                            {
                                JObject addLayerJson = new JObject
                                {
                                    ["URI"] = url
                                };
                                if (dataConnection is CIMWMSServiceConnection)
                                {
                                    // ArcGIS Earth Auotmation API can't autoatic recognize wms service.
                                    addLayerJson["type"] = "OGCWMS";
                                }
                                if (layer.MapLayerType == MapLayerType.Operational)
                                {
                                    addLayerJson["target"] = "OperationalLayers";
                                }
                                if (layer.MapLayerType == MapLayerType.BasemapBackground)
                                {
                                    addLayerJson["target"] = "BasemapLayers";
                                }
                                string currentJson = addLayerJson.ToString();
                                string[] nameAndType = new string[3]
                                {
                                    layer.Name,
                                    layer.MapLayerType.ToString(),
                                    null
                                };
                                // Add layer to Earth.
                                // When using ArcGIS Earth Automation API adding layer, whether success or failure, there will return layer id.
                                string id = ToolHelper.Utils.AddLayer(currentJson);
                                if (!ToolHelper.IDandInfoDic.Keys.Contains(id))
                                {
                                    // Use IDandInfoDic to save id and layer information.
                                    ToolHelper.IDandInfoDic.Add(id, nameAndType);
                                }
                            }
                            else
                            {
                                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Failed to add layer to ArcGIS Earth" + " : " + layer.Name);
                            }
                        });
                    }
                }
            }
            catch
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Failed to add layer to ArcGIS Earth.");
            }
        }

        private bool IsCreateNewGroupLayer(List<Layer> layerList)
        {
            // Determine whether created new group layer. 
            return layerList?.Count == 1 && layerList[0]?.Name == "New Group Layer" && (layerList[0].GetType()?.GetProperty("Layers")?.GetValue(layerList[0]) as List<Layer>) == null;
        }

        private bool Is_elevationSourceAddedChanged()
        {
            // Determine whether added layer. 
            return _elevationSources != null && _elevationSources?.Count > 0 && _sourcesOperation == ElevationSourcesOperation.Add;
        }

        private bool Is_elevationSourceRemovedChanged()
        {
            // Determine whether removed layer. 
            return _elevationSources != null && _elevationSources?.Count > 0 && _sourcesOperation == ElevationSourcesOperation.Remove;
        }

        private void Set_elevationSourceRemovedChangedStatus()
        {
            // When RemoveLayerButton is checked, _elevationSourceRemovedChanged event subscribe RemoveElevationSource; Otherwise, unsubscribe RemoveElevationSource.
            if (RemoveLayerSyncCheckBox.HasChecked)
            {
                _elevationSourceRemovedChanged += RemoveElevationSource;
            }
            else
            {
                _elevationSourceRemovedChanged -= RemoveElevationSource;
            }
        }

        private void AddElevationSource(object sender, PropertyChangedEventArgs args)
        {
            if (_sourcesOperation == ElevationSourcesOperation.Add && _elevationSources?.Count > 0)
            {
                try
                {
                    if (_elevationSources.FirstOrDefault()?.Length >= 3)
                    {
                        // Elevation source information.
                        // In fact, only can add one elevation source every time in ArcGIS Pro, so the added source is the first one.
                        string surfaceName = _elevationSources.FirstOrDefault()[0];
                        string sourceName = _elevationSources.FirstOrDefault()[1];
                        string sourceUrl = _elevationSources.FirstOrDefault()[2];
                        JObject addLayerJson = new JObject
                        {
                            ["URI"] = sourceUrl,
                            ["target"] = "ElevationLayers"
                        };
                        string currentJson = addLayerJson.ToString();
                        string[] nameAndType = new string[3]
                        {
                           surfaceName,
                           sourceName,
                           sourceUrl
                        };
                        // Add elevation source to ArcGIS Earth.
                        string id = ToolHelper.Utils.AddLayer(currentJson);
                        if (!ToolHelper.IDandInfoDic.Keys.Contains(id))
                        {
                            // Use IDandInfoDic to save id and elevation source information.
                            ToolHelper.IDandInfoDic.Add(id, nameAndType);
                        }
                    }
                }
                catch
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Failed to add source to ArcGIS Earth.");
                }
            }
        }

        private void RemoveElevationSource(object sender, PropertyChangedEventArgs args)
        {
            if (_sourcesOperation == ElevationSourcesOperation.Remove && _elevationSources?.Count > 0)
            {
                try
                {
                    // Use temp list to log the id of removed elevation sources in ArcGIS Pro. 
                    List<string> idList = new List<string>();
                    foreach (var source in _elevationSources)
                    {
                        foreach (var item in ToolHelper.IDandInfoDic)
                        {
                            // Find and save removed id.
                            if (item.Value?.Length == 3 && source?.Length >= 3 && item.Value[0] == source[0] && item.Value[1] == source[1] && item.Value[2] == source[2])
                            {
                                idList.Add(item.Key);
                            }
                        }
                    }
                    // Remove elevation sources in ArcGIS Earth and removed id of these sources.
                    // Can remove several elevation sources at one time in ArcGIS Pro.
                    foreach (var id in idList)
                    {
                        ToolHelper.Utils.RemoveLayer(id);
                        ToolHelper.IDandInfoDic.Remove(id);
                    }
                }
                catch
                {
                }
            }
        }
    }
}
