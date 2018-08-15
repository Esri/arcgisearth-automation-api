using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            List<Layer> layerList = args.Layers as List<Layer>;
            if (layerList != null && layerList.Count != 0)
            {
                Layer layer = layerList[0];
                string id = "";
                // If there have some layers that they have same name, just remove one of them 
                foreach (var item in ConnectToArcGISEarthButton.IdNameDic)
                {
                    if (item.Value?.Count == 2 && item.Value[0] == layer.Name && item.Value[1] == layer.MapLayerType.ToString())
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
}
