using System;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using Microsoft.Win32;

namespace ToArcGISEarth
{
    public class SaveImageFromArcGISEarthButton : Button
    {
        protected override async void OnClick()
        {
            this.IsChecked = true;
            if (!ConnectToArcGISEarthButton.IsConnectSuccessful)
            {
                ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Please connect to ArcGIS Earth");
                this.IsChecked = false;
                return;
            }
            else
            {
                bool result = await SaveImage();
                if (result)
                {
                    MessageBox.Show("Save screenshot from ArcGIS Earth Successfully");
                }
                this.IsChecked = false;
                return;
            }
        }

        private async Task<bool> SaveImage()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {            
                Filter = "Png file|*.png|Jpeg file|*.jpg|Tiff file|*.tif",
                DefaultExt = "png",
                OverwritePrompt = true,
                RestoreDirectory = true
            };
            if (dialog.ShowDialog() == true)
            {
                if (!String.IsNullOrWhiteSpace(dialog.FileName))
                {
                    await ConnectToArcGISEarthButton.Utils.GetSnapshot(dialog.FileName);
                    return true;
                }
            }
            return false;
        }
    }
}
