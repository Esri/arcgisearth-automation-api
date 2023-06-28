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
    public class AddLayerSyncCheckBox : Button
    {
        public AddLayerSyncCheckBox()
        {
            Enabled = false;
        }

        protected override void OnClick()
        {
            if (IsChecked)
            {
                // Unsubscribe events of adding layer and elevation source.
                LayersAddedEvent.Unsubscribe(AddLayerToEarth);
                IsChecked = false;
            }
            else
            {
                // Subscribe events of adding layer and elevation source.
                LayersAddedEvent.Subscribe(AddLayerToEarth, false);
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
                // Unsubscribe events of adding layer and elevation source.
                LayersAddedEvent.Unsubscribe(AddLayerToEarth);
                Enabled = false;
                IsChecked = false;
            }
        }

        private void AddLayerToEarth(LayerEventsArgs args)
        {
            try
            {
                // Get list of added layer.
                List<Layer> layerList = args.Layers as List<Layer>;
                if (layerList?.Count != 0 && !IsCreateNewGroupLayer(layerList))
                {
                    foreach (var layer in layerList)
                    {
                        QueuedTask.Run(async () =>
                        {
                            // GetDataConnection method must be called within the lambda passed to QueuedTask.Run. 
                            CIMDataConnection dataConnection = layer.GetDataConnection();
                            // Get layer url.
                            string url = ToolHelper.GetDataSource(dataConnection);
                            if (!string.IsNullOrWhiteSpace(url))
                            {
                                JsonObject addLayerJson = new JsonObject
                                {
                                    ["URI"] = url
                                };
                                if (dataConnection is CIMWMSServiceConnection)
                                {
                                    // Specify layer type for wms service. API is limited to automatically recognized wms service.
                                    addLayerJson["type"] = "WMS";
                                }
                                if (layer.MapLayerType == MapLayerType.Operational)
                                {
                                    addLayerJson["target"] = "operationalLayers";
                                }
                                if (layer.MapLayerType == MapLayerType.BasemapBackground)
                                {
                                    addLayerJson["target"] = "baseMaps";
                                }
                                string currentJson = addLayerJson.ToString();
                                string[] nameAndType = new string[3]
                                {
                                    layer.Name,
                                    layer.MapLayerType.ToString(),
                                    null
                                };
                                // Add layer to ArcGIS Earth.
                                // Return layer id when use adding layer, whether it's succeed or failed.
                                string id = await AutomationAPIHelper.AddLayer(currentJson);
                                if (!ToolHelper.IdInfoDictionary.Keys.Contains(id))
                                {
                                    // Use IdInfoDictionary to save layer id and layer information.
                                    ToolHelper.IdInfoDictionary.Add(id, nameAndType);
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
            // Determine if group layer is created. 
            return layerList?.Count == 1 && layerList[0]?.Name == "New Group Layer" && (layerList[0].GetType()?.GetProperty("Layers")?.GetValue(layerList[0]) as List<Layer>) == null;
        }
    }
}
