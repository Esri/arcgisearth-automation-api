using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ToArcGISEarth
{
    public class RemoveLayerButton : Button
    {
        public RemoveLayerButton()
        {
            this.Enabled = false;
        }

        protected override void OnClick()
        {
            if (this.IsChecked)
            {
                LayersRemovedEvent.Unsubscribe(RemoveLayer);
                this.IsChecked = false;
            }
            else
            {
                LayersRemovedEvent.Subscribe(RemoveLayer, false);
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
                LayersRemovedEvent.Unsubscribe(RemoveLayer);
                this.IsChecked = false;
                this.Enabled = false;
            }
        }

        private void RemoveLayer(LayerEventsArgs args)
        {
            try
            {
                List<Layer> layerList = args.Layers as List<Layer>;
                if (layerList != null && layerList.Count != 0)
                {
                    foreach (var layer in layerList)
                    {
                        string id = "";
                        foreach (var item in ConnectToArcGISEarthButton.IdNameDic)
                        {
                            if (item.Value?.Length == 2 && item.Value[0] == layer.Name && item.Value[1] == layer.MapLayerType.ToString())
                            {
                                id = item.Key;
                                break;
                            }
                        }
                        ConnectToArcGISEarthButton.Utils.RemoveLayer(id);
                        ConnectToArcGISEarthButton.IdNameDic.Remove(id);
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
