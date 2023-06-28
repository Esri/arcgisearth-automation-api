using ArcGIS.Core.CIM;
using ArcGIS.Core.Data;
using ArcGIS.Core.Geometry;
using ArcGIS.Desktop.Catalog;
using ArcGIS.Desktop.Core;
using ArcGIS.Desktop.Editing;
using ArcGIS.Desktop.Extensions;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using ArcGIS.Desktop.Mapping;
using ArcGISEarth.AutoAPI.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ProAppModule1
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
                    using (var outStream = new MemoryStream())
                    {
                        var encoder = new BmpBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                        encoder.Save(outStream);
                        var bitmap = new Bitmap(outStream);
                        bitmap.Save(dialog.FileName);
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
