using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;

namespace ToArcGISEarth
{
    public class ClearEarthWorkspaceButton : Button
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
                    ClearAll();
                    MessageBox.Show("ArcGIS Earth workspace has been cleared");
                }
            }
            else
            {
                return;
            }
        }

        public void ClearAll()
        {
            ConnectToArcGISEarthButton.Utils.ClearLayers("{\"target\":\"OperationalLayers\"}");
            ConnectToArcGISEarthButton.Utils.ClearLayers("{\"target\":\"BasemapLayers\"}");
            ConnectToArcGISEarthButton.Utils.ClearLayers("{\"target\":\"ElevationLayers\"}");
        }
    }
}
