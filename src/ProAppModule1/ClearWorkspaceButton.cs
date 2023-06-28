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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProAppModule1
{
    public class ClearWorkspaceButton : Button
    {
        public ClearWorkspaceButton()
        {
            Enabled = false;
        }

        protected override void OnClick()
        {
            IsChecked = true;
            MessageBoxResult result = ArcGIS.Desktop.Framework.Dialogs.MessageBox.Show("Are you sure you want to remove all items from current workspace of ArcGIS Earth?", null, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                ClearAll();
            }
            IsChecked = false;
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

        private async void ClearAll()
        {
            // Clear ArcGIS Earth workspace.
            await AutomationAPIHelper.ClearWorkspace();
        }
    }
}
