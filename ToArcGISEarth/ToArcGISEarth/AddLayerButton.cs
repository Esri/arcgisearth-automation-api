using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ToArcGISEarth
{
    public class AddLayerButton : Button
    {
        private const string MESSAGE_TIPS = "Failed to add layer to ArcGIS Earth.";

        #region  ElevationSource variable and property

        private Timer timer;
        private event PropertyChangedEventHandler ElevationSourceAddedChanged;
        private Dictionary<int, string> layerSource = new Dictionary<int, string>(); // layerSource[0]: id  layerSource[1]: url
        private bool? IsAddedElevationSource = null;
        private CIMMap _myIMMap = new CIMMap();
        public CIMMap MyCIMMap
        {
            get { return _myIMMap; }
            set
            {
                layerSource = ToolHelper.AddedElevationSource(_myIMMap.ElevationSurfaces, value.ElevationSurfaces, ref IsAddedElevationSource);
                if (this.IsElevationSourceAddedChanged())
                {
                    _myIMMap = value;
                    ElevationSourceAddedChanged?.Invoke(this, new PropertyChangedEventArgs("MyCIMMap"));
                }
            }
        }

        #endregion ElevationSource variable and property

        public AddLayerButton()
        {
            this.Enabled = false;
            timer = new Timer
            {
                Enabled = true,
                Interval = 2000
            };
            timer.Elapsed += (s, e) =>
            {
                QueuedTask.Run(() =>
                {
                    MyCIMMap = MapView.Active?.Map?.GetDefinition();
                });
            };         
        }

        protected override void OnClick()
        {
            if (this.IsChecked)
            {
                ElevationSourceAddedChanged -= ElevationSourceAddedEvent;
                LayersAddedEvent.Unsubscribe(this.AddLayer);
                this.timer.Stop();
                this.timer.Enabled = false;
                this.IsChecked = false;
            }
            else
            {
                ElevationSourceAddedChanged += ElevationSourceAddedEvent;
                LayersAddedEvent.Subscribe(this.AddLayer, false);
                QueuedTask.Run(() =>
                {
                    _myIMMap = MapView.Active.Map.GetDefinition();
                });
                this.timer.Enabled = true;
                this.timer.Start();
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
                ElevationSourceAddedChanged -= ElevationSourceAddedEvent;
                LayersAddedEvent.Unsubscribe(this.AddLayer);
                this.Enabled = false;
                this.IsChecked = false;
            }
        }

        private void AddLayer(LayerEventsArgs args)
        {
            try
            {
                List<Layer> layerList = args.Layers as List<Layer>;
                if (layerList?.Count != 0 && !this.IsCreateNewGroupLayer(layerList))
                {
                    foreach (var layer in layerList)
                    {
                        QueuedTask.Run(() =>
                        {
                            // This method or property must be called within the lambda passed to QueuedTask.Run. 
                            CIMObject dataConnection = layer.GetDataConnection();
                            string url = ToolHelper.GetDataSource(dataConnection, false);
                            if (!String.IsNullOrWhiteSpace(url))
                            {
                                JObject addLayerJson = new JObject
                                {
                                    ["URI"] = url
                                };
                                if (dataConnection is CIMWMSServiceConnection)
                                {
                                    addLayerJson["type"] = "OGCWMS"; // ArcGIS Earth Auotmation API can't autoatic recognize wms service.
                                }
                                if (layer.MapLayerType == ArcGIS.Core.CIM.MapLayerType.Operational)
                                {
                                    addLayerJson["target"] = "OperationalLayers";
                                }
                                if (layer.MapLayerType == ArcGIS.Core.CIM.MapLayerType.BasemapBackground)
                                {
                                    addLayerJson["target"] = "BasemapLayers";
                                }
                                string currentJson = addLayerJson.ToString();
                                string[] nameAndType = new string[2]
                                {
                                    layer.Name,
                                    layer.MapLayerType.ToString()
                                };
                                string id = ToolHelper.Utils.AddLayer(currentJson);
                                if (!ToolHelper.IdNameDic.Keys.Contains(id))
                                {
                                    ToolHelper.IdNameDic.Add(id, nameAndType);
                                }
                            }
                            else
                            {
                                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(MESSAGE_TIPS.Remove(MESSAGE_TIPS.Length - 1, 1) + " : " + layer.Name);
                            }
                        });
                    }
                }
            }
            catch
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(MESSAGE_TIPS);
            }
        }

        private bool IsCreateNewGroupLayer(List<Layer> layerList)
        {
            return layerList?.Count == 1 && layerList[0]?.Name == "New Group Layer" && (layerList[0].GetType()?.GetProperty("Layers")?.GetValue(layerList[0]) as List<Layer>) == null;
        }

        private bool IsElevationSourceAddedChanged()
        {
            if (layerSource != null && layerSource.Count != 0)
            {
                // added elevation source
                if (IsAddedElevationSource == true)
                {
                    return layerSource.Values.FirstOrDefault() != null;
                }
                return false;
            }
            return false;
        }

        private void ElevationSourceAddedEvent(object sender, PropertyChangedEventArgs args)
        {
            if (IsAddedElevationSource == true && layerSource?.Count != 0)
            {
                string url = layerSource.Values.FirstOrDefault();
                JObject addLayerJson = new JObject
                {
                    ["URI"] = url,
                    ["target"] = "ElevationLayers"
                };
                string currentJson = addLayerJson.ToString();
                string[] nameAndType = new string[2]
                {
                         url,
                        "ElevationLayers"
                };
                if (IsAddedElevationSource == true)
                {
                    try
                    {
                        string id = ToolHelper.Utils.AddLayer(currentJson);
                        if (!ToolHelper.IdNameDic.Keys.Contains(id))
                        {
                            ToolHelper.IdNameDic.Add(id, nameAndType);
                        }
                    }
                    catch
                    {
                        ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(MESSAGE_TIPS);
                    }
                }
            }
        }
    }
}
