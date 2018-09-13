using ArcGIS.Desktop.Framework.Contracts;

namespace ToArcGISEarth
{
    public class ConnectButton : Button
    {
        private const string CONNECTION_SUCCESS = "Success";
        private const string CAPTION_CONNECT = "Connect";
        private const string CAPTION_DISCONNECT = "Disconnect";
        private const string MESSAGE_TIPS = "Please make sure ArcGIS Earth is running and the Automation API is configured as enabled. Then try to connect again.";

        protected override async void OnClick()
        {
            if (ToolHelper.IsArcGISProSceneOpening)
            {
                if (ToolHelper.IsArcGISEarthRunning)
                {
                    this.Enabled = true;
                    if (this.IsChecked)
                    {
                        ToolHelper.Utils.CloseConnect();
                        this.IsChecked = false;
                        this.Caption = CAPTION_CONNECT;
                        ToolHelper.IsConnectSuccessfully = false;
                    }
                    else
                    {
                        string result = await ToolHelper.Utils.Connect();
                        if (result != CONNECTION_SUCCESS)
                        {
                            ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(MESSAGE_TIPS);
                            return;
                        }
                        else
                        {
                            this.IsChecked = true;
                            this.Caption = CAPTION_DISCONNECT;
                            ToolHelper.IsConnectSuccessfully = true;
                        }
                    }
                }
                else
                {
                    ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(MESSAGE_TIPS);
                }
            }
            else
            {
                ToolHelper.IsConnectSuccessfully = false;
                this.IsChecked = false;
                this.Caption = CAPTION_CONNECT;
                this.Enabled = false;
            }
        }

        protected override void OnUpdate()
        {
            if (ToolHelper.IsArcGISProSceneOpening)
            {
                this.Enabled = true;
                if (!ToolHelper.IsArcGISEarthRunning)
                {
                    ToolHelper.IsConnectSuccessfully = false;
                    this.IsChecked = false;
                    this.Caption = CAPTION_CONNECT;
                }
            }
            else
            {
                ToolHelper.IsConnectSuccessfully = false;
                this.IsChecked = false;
                this.Caption = CAPTION_CONNECT;
                this.Enabled = false;
            }
        }
    }
}
