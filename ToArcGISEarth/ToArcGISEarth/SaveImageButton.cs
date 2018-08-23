using System.Threading.Tasks;
using ArcGIS.Desktop.Framework.Contracts;
using Microsoft.Win32;

namespace ToArcGISEarth
{
    public class SaveImageButton : Button
    {
        public SaveImageButton()
        {
            this.Enabled = false;
        }

        protected override async void OnClick()
        {
            this.IsChecked = true;
            await this.SaveImage();
            this.IsChecked = false;
            return;
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

        private async Task SaveImage()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Png file|*.png|Jpeg file|*.jpg|Tiff file|*.tif",
                FileName = "ArcGIS Earth.png",
                DefaultExt = "png",
                OverwritePrompt = true,
                RestoreDirectory = true
            };
            if (dialog.ShowDialog() == true)
            {
                await ToolHelper.Utils.GetSnapshot(dialog.FileName);
            }
        }
    }
}
