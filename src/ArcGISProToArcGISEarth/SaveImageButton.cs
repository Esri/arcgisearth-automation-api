// Copyright 2020 Esri
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
using Microsoft.Win32;

namespace ToArcGISEarth
{
    public class SaveImageButton : Button
    {
        public SaveImageButton()
        {
            Enabled = false;
        }

        protected override void OnClick()
        {
            IsChecked = true;
            SaveImage();
            IsChecked = false;
            return;
        }

        protected override void OnUpdate()
        {
            // Set button status when status of ArcGIS Earth or ArcGIS Pro changed.
            if (ToolHelper.IsArcGISEarthRunning && ToolHelper.IsArcGISProGlobalSceneOpening)
            {
                Enabled = true;
            }
            else
            {
                Enabled = false;
                IsChecked = false;
            }
        }

        private void SaveImage()
        {
            // Set save file options.
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Jpeg Files|*.jpg|Png Files|*.png|Tiff Files|*.tif",
                FileName = "ArcGIS Earth.jpg",
                DefaultExt = "jpg",
                OverwritePrompt = true,
                RestoreDirectory = true
            };
            if (dialog.ShowDialog() == true)
            {
                // Get screenshot from ArcGIS Earth.
                ToolHelper.Utils.GetSnapshot(dialog.FileName);
            }
        }
    }
}
