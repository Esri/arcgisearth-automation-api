using System.Windows;
using ArcGIS.Desktop.Framework.Contracts;

namespace ToArcGISEarth
{
    public class ClearWorkspaceButton : Button
    {
        private const string MESSAGE_TIPS = "Are you sure you want to remove all items from current workspace?";

        public ClearWorkspaceButton()
        {
            this.Enabled = false;
        }

        protected override void OnClick()
        {
            this.IsChecked = true;
            MessageBoxResult result = ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(MESSAGE_TIPS, null, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                this.ClearAll();
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
            if (ToolHelper.IsConnectSuccessfully)
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
            ToolHelper.Utils.ClearLayers("{\"target\":\"AllLayers\"}");
        }
    }
}
