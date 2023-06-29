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
using ArcGISEarth.AutoAPI.Utils;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace ArcGISProToArcGISEarth
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

        private async void SaveImage()
        {
            try
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
                    var bitmapImage = await AutomationAPIHelper.TakeSnapshot() as BitmapImage;
                    if (bitmapImage == null)
                    {
                        ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Failed to save image.");
                        return;
                    }
                    var encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                    using (FileStream fs = new FileStream(dialog.FileName, FileMode.Create))
                    {
                        encoder.Save(fs);
                    }
                }
            }
            catch (Exception)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Failed to save image.");
            }
        }
    }
}
