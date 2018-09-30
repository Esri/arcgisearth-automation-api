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
        protected override async void OnClick()
        {
            // Determine whether ArcGIS Earth running.
            if (ToolHelper.IsArcGISEarthRunning)
            {
                if (IsChecked)
                {
                    // Close connecting to ArcGIS Earth and set button status.
                    ToolHelper.Utils.CloseConnect();
                    IsChecked = false;
                    Caption = "Connect";
                    ToolHelper.IsConnectSuccessfully = false;
                }
                else
                {
                    // try to connect to ArcGIS Earth.
                    string result = await ToolHelper.Utils.Connect();
                    if (result != "Success")
                    {
                        ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please make sure ArcGIS Earth is running and the Automation API is configured as enabled. Then try to connect again.");
                    }
                    else
                    {
                        // Connecting to ArcGIS successfully and set button status. 
                        IsChecked = true;
                        Caption = "Disconnect";
                        ToolHelper.IsConnectSuccessfully = true;
                    }
                }
            }
            else
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please make sure ArcGIS Earth is running and the Automation API is configured as enabled. Then try to connect again.");
            }
        }

        protected override void OnUpdate()
        {
            // Determine whether ArcGIS Pro global scene is opening. 
            if (ToolHelper.IsArcGISProGlobalSceneOpening)
            {
                Enabled = true;
                // Set button status when ArcGIS Earth is not running.
                if (!ToolHelper.IsArcGISEarthRunning)
                {
                    ToolHelper.IsConnectSuccessfully = false;
                    IsChecked = false;
                    Caption = "Connect";
                }
            }
            else
            {
                // Set button status when ArcGIS Pro global scene is not opening.
                ToolHelper.IsConnectSuccessfully = false;
                IsChecked = false;
                Caption = "Connect";
                Enabled = false;
            }
        }
    }
}
