using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using ArcGIS.Desktop.Mapping;



namespace ToArcGISEarth
{
    internal class CustomControl1ViewModel : CustomControl
    {
        /// <summary>
        /// Text shown in the control.
        /// </summary>
        private string _text = "Custom Control";
        public string Text
        {
            get { return _text; }
            set
            {
                SetProperty(ref _text, value, () => Text);
            }
        }
    }
}
