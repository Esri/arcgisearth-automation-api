// Copyright 2023 Esri
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
using System;
using System.Diagnostics;

namespace ArcGISProToArcGISEarth
{
    public class HelpButton : Button
    {
        protected override void OnClick()
        {
            try
            {
                // Open the help page. 
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://doc.arcgis.com/en/arcgis-earth/automation-api/samples.htm",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show(ex.Message);
            }
        }
    }
}
