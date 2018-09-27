// Copyright 2018 Esri
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

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
                    Enabled = true;
                    if (IsChecked)
                    {
                        ToolHelper.Utils.CloseConnect();
                        IsChecked = false;
                        Caption = CAPTION_CONNECT;
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
                            IsChecked = true;
                            Caption = CAPTION_DISCONNECT;
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
                IsChecked = false;
                Caption = CAPTION_CONNECT;
                Enabled = false;
            }
        }

        protected override void OnUpdate()
        {
            if (ToolHelper.IsArcGISProSceneOpening)
            {
                Enabled = true;
                if (!ToolHelper.IsArcGISEarthRunning)
                {
                    ToolHelper.IsConnectSuccessfully = false;
                    IsChecked = false;
                    Caption = CAPTION_CONNECT;
                }
            }
            else
            {
                ToolHelper.IsConnectSuccessfully = false;
                IsChecked = false;
                Caption = CAPTION_CONNECT;
                Enabled = false;
            }
        }
    }
}
