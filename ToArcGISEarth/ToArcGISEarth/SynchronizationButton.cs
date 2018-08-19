using ArcGIS.Desktop.Framework.Contracts;

namespace ToArcGISEarth
{
    public class SynchronizationButton : Button
    {
        public SynchronizationButton()
        {
            this.Enabled = false;
        }

        protected override void OnUpdate()
        {
            if (ToolHelper.IsConnectSuccessfully)
            {
                this.Enabled = true;
                if (ToolHelper.IsSynchronizationButtonChecked)
                {
                    this.IsChecked = true;
                }
                else
                {
                    this.IsChecked = false;
                }
            }
            else
            {
                this.Enabled = false;
                this.IsChecked = false;
            }
        }
    }
}
