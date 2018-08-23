using System.Diagnostics;
using ArcGIS.Desktop.Framework.Contracts;

namespace ToArcGISEarth
{
    public class HelpButton : Button
    {
        protected override void OnClick()
        {
            Process.Start("http://doc.arcgis.com/en/arcgis-earth/automation-api/samples.htm");               
        }
    }
}
