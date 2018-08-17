using System.Windows;
using ArcGIS.Desktop.Framework.Contracts;

namespace ToArcGISEarth
{
    public class ClearArcGISEarthWorkspaceButton : Button
    {
        public ClearArcGISEarthWorkspaceButton()
        {
            this.Enabled = false;
        }

        protected override void OnClick()
        {
            this.IsChecked = true;
            MessageBoxResult result = ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Are you sure you want to remove all items from current workspace?", "", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                ClearAll();
                this.IsChecked = false;
            }
            else
            {
                this.IsChecked = false;
                return;
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
                this.Enabled = false;
                this.IsChecked = false;
            }
        }

        private void ClearAll()
        {
            ConnectToArcGISEarthButton.Utils.ClearLayers("{\"target\":\"AllLayers\"}");
        }
    }
}
