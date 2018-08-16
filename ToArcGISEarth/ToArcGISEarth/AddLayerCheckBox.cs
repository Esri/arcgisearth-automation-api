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
    public class AddLayerCheckBox : CheckBox
    {
        protected override void OnClick()
        {
            if ((bool)IsChecked)
            {
                if (!ConnectToArcGISEarthButton.IsConnectSuccessful)
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please connect to ArcGIS Earth");
                    this.IsChecked = false;
                    return;
                }
                else
                {
                    LayersAddedEvent.Subscribe(AddLayer, false);
                }
            }
            else
            {
                LayersAddedEvent.Unsubscribe(AddLayer);
            }
        }

        private void AddLayer(LayerEventsArgs args)
        {
            try
            {
                List<Layer> layerList = args.Layers as List<Layer>;
                if (layerList != null && layerList.Count != 0)
                {
                    Layer layer = layerList[0] as Layer;
                    string url = GetLayerUrl(layer);
                    if (url != null)
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
                        }
                    }
                    else
                    {
                        ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Add layer into ArcGIS Earth unsuccessfully, because ArcGIS Earth can not get layer url");
                    }
                }
            }
            catch (Exception ex)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ex.Message, "Error message", MessageBoxButton.OK);
            }
        }

        private string GetLayerUrl(Layer layer)
        {
            return layer?.GetType()?.GetProperty("URL")?.GetValue(layer) as string;
        }
    }
}
