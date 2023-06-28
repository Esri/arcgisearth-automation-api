using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using ArcGISEarth.AutoAPI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ProAppModule1
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
            // Set button status when status of ArcGIS Earth or ArcGIS Pro changed.
            if (ToolHelper.IsArcGISEarthRunning && ToolHelper.IsArcGISProGlobalSceneOpening)
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

        private async void RemoveLayerFromEarth(LayerEventsArgs args)
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
                        JsonObject idJson = JsonNode.Parse(id).AsObject();
                        string idString = idJson["id"].ToString();
                        await AutomationAPIHelper.RemoveLayer(idString);
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
