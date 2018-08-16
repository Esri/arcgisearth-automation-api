using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ToArcGISEarth
{
    public class RemoveLayerCheckBox : CheckBox
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
                    LayersRemovedEvent.Subscribe(RemoveLayer, false);
                }
            }
            else
            {
                LayersRemovedEvent.Unsubscribe(RemoveLayer);
            }
        }

        private void RemoveLayer(LayerEventsArgs args)
        {
            try
            {
                List<Layer> layerList = args.Layers as List<Layer>;
                if (layerList != null && layerList.Count != 0)
                {
                    Layer layer = layerList[0];
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
            catch (Exception ex)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ex.Message, "Error message", MessageBoxButton.OK);
            }
        }
    }
}
