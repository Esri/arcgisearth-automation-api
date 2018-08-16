using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            List<Layer> layerList = args.Layers as List<Layer>;
            if (layerList != null && layerList.Count != 0)
            {
                Layer layer = layerList[0] as Layer;
                JObject addLayerJson = new JObject();
                addLayerJson["URI"] = GetLayerUrl(layer);
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
                string[] nameAndType = new string[2];
                nameAndType[0] = (layer.Name);
                nameAndType[1] = (layer.MapLayerType.ToString());
                string id = ConnectToArcGISEarthButton.Utils.AddLayer(currentJson);
                if (!ConnectToArcGISEarthButton.IdNameDic.Keys.Contains(id))
                {
                    ConnectToArcGISEarthButton.IdNameDic.Add(id, nameAndType);
                }
            }
        }

        private string GetLayerUrl(Layer layer)
        {
            if (layer is ServiceLayer)
            {
                return (layer as ServiceLayer).URL;
            }
            if (layer is RasterLayer)
            {

            }
            if (layer is FeatureLayer)
            {

            }
            return "";
        }
    }
}
