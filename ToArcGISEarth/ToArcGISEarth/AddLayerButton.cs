using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ToArcGISEarth
{
    public class AddLayerButton : Button
    {
        public AddLayerButton()
        {
            this.Enabled = false;
        }

        protected override void OnClick()
        {
            if (this.IsChecked)
            {
                LayersAddedEvent.Unsubscribe(AddLayer);
                this.IsChecked = false;
            }
            else
            {
                LayersAddedEvent.Subscribe(AddLayer, false);
                this.IsChecked = true;
            }
        }

        protected override void OnUpdate()
        {
            if (ConnectToArcGISEarthButton.IsConnectSuccessfully)
            {
                this.Enabled = true;
            }
            else
            {
                LayersAddedEvent.Unsubscribe(AddLayer);           
                this.IsChecked = false;
                this.Enabled = false;
            }
        }

        private void AddLayer(LayerEventsArgs args)
        {
            try
            {
                List<Layer> layerList = args.Layers as List<Layer>;
                if (layerList != null && layerList.Count != 0)
                {
                    if (!IsCreateNewGroupLayer(layerList))
                    {
                        List<string> unsuccessLayerNames = new List<string>();
                        foreach (var layer in layerList)
                        {
                            string url = GetLayerUrl(layer);
                            if (!String.IsNullOrWhiteSpace(url))
                            {
                                JObject addLayerJson = new JObject
                                {
                                    ["URI"] = url
                                };
                                if (layer.MapLayerType == ArcGIS.Core.CIM.MapLayerType.Operational)
                                {
                                    addLayerJson["target"] = "OperationalLayers";
                                }
                                if (layer.MapLayerType == ArcGIS.Core.CIM.MapLayerType.BasemapBackground)
                                {
                                    addLayerJson["target"] = "BasemapLayers";
                                }
                                string currentJson = addLayerJson.ToString();
                                if (currentJson != null)
                                {
                                    currentJson = currentJson.Replace("\n", "");
                                    currentJson = currentJson.Replace("\r", "");
                                }
                                string[] nameAndType = new string[2]
                                {
                                    layer.Name,
                                    layer.MapLayerType.ToString()
                                };
                                string id = ConnectToArcGISEarthButton.Utils.AddLayer(currentJson);
                                if (!ConnectToArcGISEarthButton.IdNameDic.Keys.Contains(id))
                                {
                                    ConnectToArcGISEarthButton.IdNameDic.Add(id, nameAndType);
                                    return;
                                }
                            }
                            else
                            {
                                unsuccessLayerNames.Add(layer.Name);
                                continue;
                            }
                        }
                        if (unsuccessLayerNames.Count != 0)
                        {
                            string result = "";
                            foreach (var layerName in unsuccessLayerNames)
                            {
                                result += "\n" + layerName;
                            }
                            ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show($"Failed to add layer to ArcGIS Earth:{result}");
                        }
                    }
                }
            }
            catch
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Failed to add layer to ArcGIS Earth.");
            }
        }

        private string GetLayerUrl(Layer layer)
        {
            // Determine whether the layer has URL property, if has, get URL value.
            return layer?.GetType()?.GetProperty("URL")?.GetValue(layer) as string;
        }

        private bool IsCreateNewGroupLayer(List<Layer> layerList)
        {
            if (layerList.Count == 1 && layerList[0].Name == "New Group Layer" && (layerList[0].GetType()?.GetProperty("Layers")?.GetValue(layerList[0]) as List<Layer>) == null)
            {
                return true;
            }
            return false;
        }
    }
}
