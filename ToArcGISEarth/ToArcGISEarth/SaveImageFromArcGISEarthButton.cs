using System;
using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using Microsoft.Win32;

namespace ToArcGISEarth
{
    public class SaveImageFromArcGISEarthButton : Button
    {
        public SaveImageFromArcGISEarthButton()
        {
            this.Enabled = false;
        }

        protected override async void OnClick()
        {
            this.IsChecked = true;
            await SaveImage();
            this.IsChecked = false;
            return;
        }

        protected override void OnUpdate()
        {
            if (ConnectToArcGISEarthButton.IsConnectSuccessfully)
            {
                this.Enabled = true;
            }
            else
            {
                this.Enabled = false;
                this.IsChecked = false;
            }
        }

        private async Task SaveImage()
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
                }
            }
        }
    }
}
