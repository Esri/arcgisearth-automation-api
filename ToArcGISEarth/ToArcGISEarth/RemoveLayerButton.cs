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
                LayersRemovedEvent.Unsubscribe(RemoveLayer);
                IsChecked = false;
                HasChecked = false;
            }
            else
            {
                LayersRemovedEvent.Subscribe(RemoveLayer, false);
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
                LayersRemovedEvent.Unsubscribe(RemoveLayer);
                Enabled = false;
                IsChecked = false;
                HasChecked = false;
            }
        }

        private void RemoveLayer(LayerEventsArgs args)
        {
            try
            {
                List<Layer> layerList = args.Layers as List<Layer>;
                if (layerList?.Count != 0)
                {
                    foreach (var layer in layerList)
                    {
                        string id = "";
                        foreach (var item in ToolHelper.IdNameDic)
                        {
                            if (item.Value?.Length == 2 && item.Value[0] == layer.Name && item.Value[1] == layer.MapLayerType.ToString())
                            {
                                id = item.Key;
                                break;
                            }
                        }
                        ToolHelper.Utils.RemoveLayer(id);
                        ToolHelper.IdNameDic.Remove(id);
                        return;
                    }
                }
            }
            catch
            {
                return;
            }
        }
    }
}

