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
using System.Linq;
using System.Timers;

namespace ToArcGISEarth
{
    public class AddLayerButton : Button
    {
        private const string MESSAGE_TIPS = "Failed to add layer to ArcGIS Earth.";

        #region  ElevationSource variable and property

        private Timer timer;
        private event PropertyChangedEventHandler ElevationSourceAddedChanged;
        private event PropertyChangedEventHandler ElevationSourceRemovedChanged;
        private Dictionary<int, string> layerSource = new Dictionary<int, string>();
        private bool? addOrRemove = null; // true: added elevation source, false: removed elevation source, null: do nothing
        private CIMMap _myIMMap = new CIMMap();
        public CIMMap MyCIMMap
        {
            get { return _myIMMap; }
            set
            {
                layerSource = ToolHelper.AddedOrRemovedElevationSource(_myIMMap.ElevationSurfaces, value.ElevationSurfaces, ref addOrRemove);
                if (this.IsElevationSourceAddedChanged())
                {
                    _myIMMap = value;
                    ElevationSourceAddedChanged?.Invoke(this, new PropertyChangedEventArgs("MyCIMMap"));
                }
                if (this.IsElevationSourceRemovedChanged() && RemoveLayerButton.HasChecked)
                {
                    _myIMMap = value;
                    ElevationSourceRemovedChanged?.Invoke(this, new PropertyChangedEventArgs("MyCIMMap"));
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
                Interval = 1000
            };
            // Update current MyCIMMap every 1 second
            timer.Elapsed += (s, e) =>
            {
                QueuedTask.Run(() =>
                {
                    MyCIMMap = MapView.Active?.Map?.GetDefinition();
                });
            };
            ElevationSourceAddedChanged += ElevationSourceAddedEvent;
            ElevationSourceRemovedChanged += ElevationSourceRemovedEvent;
        }

        protected override void OnClick()
        {
            if (this.IsChecked)
            {
                LayersAddedEvent.Unsubscribe(this.AddLayer);
                this.timer.Stop();
                this.IsChecked = false;
                if (RemoveLayerButton.HasChecked)
                {
                    ElevationSourceRemovedChanged += ElevationSourceRemovedEvent;
                }
            }
            else
            {
                if (RemoveLayerButton.HasChecked)
                {
                    ElevationSourceRemovedChanged += ElevationSourceRemovedEvent;
                }
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
                if (addOrRemove == true)
                {
                    return layerSource.Values.FirstOrDefault() != null;
                }
                return false;
            }
            return false;
        }

        private bool IsElevationSourceRemovedChanged()
        {
            if (layerSource != null && layerSource.Count != 0)
            {
                // added elevation source
                if (addOrRemove == false)
                {
                    return layerSource.Values.FirstOrDefault() != null;
                }
                return false;
            }
            return false;
        }

        private void ElevationSourceAddedEvent(object sender, PropertyChangedEventArgs args)
        {
            if (addOrRemove == true && layerSource?.Count != 0)
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

        private void ElevationSourceRemovedEvent(object sender, PropertyChangedEventArgs args)
        {
            if (addOrRemove == false && layerSource?.Count != 0)
            {
                string url = layerSource.Values.FirstOrDefault();
                string id = "";
                foreach (var item in ToolHelper.IdNameDic)
                {
                    if (item.Value?.Length == 2 && item.Value[0] == url && item.Value[1] == "ElevationLayers")
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
}
