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

using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using System.Collections.Generic;

namespace ToArcGISEarth
{
    public class RemoveLayerSyncCheckBox : Button
    {
        public RemoveLayerSyncCheckBox()
        {
            Enabled = false;
        }

        protected override void OnClick()
        {
            if (IsChecked)
            {
                // Unsubscribe RemoveLayerFromEarth and set button status.
                LayersRemovedEvent.Unsubscribe(RemoveLayerFromEarth);
                IsChecked = false;
            }
            else
            {
                // Subscribe RemoveLayerFromEarth and set button status.
                LayersRemovedEvent.Subscribe(RemoveLayerFromEarth, false);
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
                LayersRemovedEvent.Unsubscribe(RemoveLayerFromEarth);
                Enabled = false;
                IsChecked = false;
            }
        }

        private void RemoveLayerFromEarth(LayerEventsArgs args)
        {
            try
            {
                // Get removed layer list.
                List<Layer> layerList = args.Layers as List<Layer>;
                if (layerList?.Count != 0)
                {
                    // Use temp list to log the id of removed elevation sources in ArcGIS Pro. 
                    List<string> idList = new List<string>();
                    foreach (var layer in layerList)
                    {
                        foreach (var item in ToolHelper.IdInfoDictionary)
                        {
                            // Find and save removed id.
                            if (item.Value?.Length == 3 && item.Value[0] == layer.Name && item.Value[1] == layer.MapLayerType.ToString() && item.Value[2] == null)
                            {
                                idList.Add(item.Key);
                            }
                        }
                    }
                    // Remove elevation sources in ArcGIS Earth and removed id of these sources.
                    foreach (var id in idList)
                    {
                        ToolHelper.Utils.RemoveLayer(id);
                        ToolHelper.IdInfoDictionary.Remove(id);
                    }
                }
            }
            catch
            {
            }
        }
    }
}

