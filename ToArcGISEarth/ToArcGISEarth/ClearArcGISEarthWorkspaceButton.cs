using System.Windows;
using ArcGIS.Desktop.Framework.Contracts;

namespace ToArcGISEarth
{
    public class ClearArcGISEarthWorkspaceButton : Button
    {
        protected override void OnClick()
        {
            this.IsChecked = true;
            if (!ConnectToArcGISEarthButton.IsConnectSuccessful)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please connect to ArcGIS Earth");
                this.IsChecked = false;
                return;
            }
            MessageBoxResult result = ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Are you sure to clear ArcGIS Earth workspace", "Tip", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                ClearAll();
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("ArcGIS Earth workspace has been cleared");
                this.IsChecked = false;
            }
            else
            {
                this.IsChecked = false;
                return;
            }
        }

        private void ClearAll()
        {
            ConnectToArcGISEarthButton.Utils.ClearLayers("{\"target\":\"OperationalLayers\"}");
            ConnectToArcGISEarthButton.Utils.ClearLayers("{\"target\":\"BasemapLayers\"}");
            ConnectToArcGISEarthButton.Utils.ClearLayers("{\"target\":\"ElevationLayers\"}");
        }
    }
}
