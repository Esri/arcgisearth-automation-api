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

using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using System.Collections.Generic;

namespace ToArcGISEarth
{
    public class RemoveLayerButton : Button
    {
        public static bool HasChecked { get; set; }

        public RemoveLayerButton()
        {
            Enabled = false;
            HasChecked = false;
        }

        protected override void OnClick()
        {
            if (IsChecked)
            {
                LayersRemovedEvent.Unsubscribe(RemoveLayerFromEarthEvent);
                IsChecked = false;
                HasChecked = false;
            }
            else
            {
                LayersRemovedEvent.Subscribe(RemoveLayerFromEarthEvent, false);
                IsChecked = true;
                HasChecked = true;
            }
        }

        protected override void OnUpdate()
        {
            if (ToolHelper.IsConnectSuccessfully)
            {
                Enabled = true;
            }
            else
            {
                LayersRemovedEvent.Unsubscribe(RemoveLayerFromEarthEvent);
                Enabled = false;
                IsChecked = false;
                HasChecked = false;
            }
        }

        private void RemoveLayerFromEarthEvent(LayerEventsArgs args)
        {
            try
            {
                List<Layer> layerList = args.Layers as List<Layer>;
                if (layerList?.Count != 0)
                {
                    List<string> idList = new List<string>();
                    foreach (var layer in layerList)
                    {
                        foreach (var item in ToolHelper.IdNameDic)
                        {
                            if (item.Value?.Length == 2 && item.Value[0] == layer.Name && item.Value[1] == layer.MapLayerType.ToString())
                            {
                                idList.Add(item.Key);
                            }
                        }
                    }
                    foreach (var id in idList)
                    {
                        ToolHelper.Utils.RemoveLayer(id);
                        ToolHelper.IdNameDic.Remove(id);
                    }
                }
            }
            catch
            {
            }
        }
    }
}

