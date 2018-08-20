using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using System.Collections.Generic;

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
                LayersRemovedEvent.Unsubscribe(this.RemoveLayer);
                this.IsChecked = false;
            }
            else
            {
                LayersRemovedEvent.Subscribe(this.RemoveLayer, false);
                this.IsChecked = true;
            }
        }

        protected override void OnUpdate()
        {
            if (ToolHelper.IsConnectSuccessfully)
            {
                this.Enabled = true;
            }
            else
            {
                LayersRemovedEvent.Unsubscribe(this.RemoveLayer);
                this.Enabled = false;
                this.IsChecked = false;
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
