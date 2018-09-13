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
