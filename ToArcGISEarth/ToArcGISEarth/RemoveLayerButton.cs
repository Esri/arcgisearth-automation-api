using ArcGIS.Core.CIM;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;

namespace ToArcGISEarth
{
    public class RemoveLayerButton : Button
    {
        #region ElevationSource variable and property

        private Timer timer;
        private event PropertyChangedEventHandler ElevationSourceRemovedChanged;
        private Dictionary<int, string> layerSource = new Dictionary<int, string>(); // layerSource[0]: id  layerSource[1]: url
        private bool? IsRemovedElevationSource = null;
        private CIMMap _myIMMap = new CIMMap();
        public CIMMap MyCIMMap
        {
            get { return _myIMMap; }
            set
            {
                layerSource = ToolHelper.RemovedElevationSource(_myIMMap.ElevationSurfaces, value.ElevationSurfaces, ref IsRemovedElevationSource);
                if (this.IsElevationSourceRemovedChanged())
                {
                    _myIMMap = value;
                    ElevationSourceRemovedChanged?.Invoke(this, new PropertyChangedEventArgs("MyCIMMap"));
                }
            }
        }

        #endregion ElevationSource variable and property

        public RemoveLayerButton()
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
                ElevationSourceRemovedChanged -= ElevationSourceRemovedEvent;
                LayersRemovedEvent.Unsubscribe(this.RemoveLayer);
                this.timer.Stop();
                this.timer.Enabled = false;
                this.IsChecked = false;
            }
            else
            {
                ElevationSourceRemovedChanged += ElevationSourceRemovedEvent;
                LayersRemovedEvent.Subscribe(this.RemoveLayer, false);
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

        private bool IsElevationSourceRemovedChanged()
        {
            if (layerSource != null && layerSource.Count != 0)
            {
                // added elevation source
                if (IsRemovedElevationSource == true)
                {
                    return layerSource.Values.FirstOrDefault() != null;
                }
                return false;
            }
            return false;
        }

        private void ElevationSourceRemovedEvent(object sender, PropertyChangedEventArgs args)
        {
            if (IsRemovedElevationSource == true && layerSource?.Count != 0)
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

